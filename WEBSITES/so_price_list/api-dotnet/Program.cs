using System.Data;
using Microsoft.Data.SqlClient;
using SoPriceListApi;
using static SoPriceListApi.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Db>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

/* Every procedure takes @logempcd last. It arrives as X-Emp-Cd, which the web
   app sets from the user_id URL parameter the VB.NET client passes, and lands
   in created_by / updated_by. */
static string Who(HttpRequest r)
{
    var v = r.Headers["X-Emp-Cd"].FirstOrDefault()
            ?? r.Query["logempcd"].FirstOrDefault()
            ?? "";
    return v.Length > 15 ? v[..15] : v;
}

static string? S(string? v) => string.IsNullOrWhiteSpace(v) ? null : v;
static long? L(string? v) => long.TryParse(v, out var n) ? n : null;
static int? I(string? v) => int.TryParse(v, out var n) ? n : null;
static DateTime? D(string? v) => DateTime.TryParse(v, out var d) ? d : null;

static long? LJ(System.Text.Json.JsonElement e, string k) =>
    e.TryGetProperty(k, out var v) && v.ValueKind is not System.Text.Json.JsonValueKind.Null
        ? (v.ValueKind == System.Text.Json.JsonValueKind.Number ? v.GetInt64() : L(v.GetString()))
        : null;
/* Present-but-unparseable is a caller error, not "leave it alone". Returning
   null for junk quietly wrote 0 into qty_min and NULL into price. */
static int? IJ(System.Text.Json.JsonElement e, string k)
{
    if (!e.TryGetProperty(k, out var v) || v.ValueKind is System.Text.Json.JsonValueKind.Null)
        return null;
    if (v.ValueKind == System.Text.Json.JsonValueKind.Number) return v.GetInt32();
    var s = v.GetString();
    if (string.IsNullOrWhiteSpace(s)) return null;
    if (int.TryParse(s, out var n)) return n;
    throw new BadInputException($"{k} must be a whole number; got \"{s}\".");
}
static string? SJ(System.Text.Json.JsonElement e, string k) =>
    e.TryGetProperty(k, out var v) && v.ValueKind is not System.Text.Json.JsonValueKind.Null
        ? S(v.ValueKind == System.Text.Json.JsonValueKind.String ? v.GetString() : v.ToString())
        : null;
static decimal? DecJ(System.Text.Json.JsonElement e, string k)
{
    if (!e.TryGetProperty(k, out var v) || v.ValueKind is System.Text.Json.JsonValueKind.Null)
        return null;
    if (v.ValueKind == System.Text.Json.JsonValueKind.Number) return v.GetDecimal();
    var s = v.GetString();
    if (string.IsNullOrWhiteSpace(s)) return null;
    if (decimal.TryParse(s, System.Globalization.NumberStyles.Number,
                         System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
    throw new BadInputException($"{k} must be a number; got \"{s}\".");
}
static DateTime? DJ(System.Text.Json.JsonElement e, string k) => D(SJ(e, k));
static bool BJ(System.Text.Json.JsonElement e, string k) =>
    e.TryGetProperty(k, out var v) && v.ValueKind == System.Text.Json.JsonValueKind.True;

/* The procedures RAISERROR with plain-language messages meant for the user
   ("Another price list already uses this name."). Return those as 400 rather
   than burying them in a 500. */
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (BadInputException ex)
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (SqlException ex) when (ex.Number is 50000 or 0)
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

app.MapGet("/health", async (Db db) =>
{
    try { await db.CanConnectAsync(); return Results.Ok(new { ok = true, db = "connected" }); }
    catch (Exception ex) { return Results.Json(new { ok = false, db = "unavailable", error = ex.Message }, statusCode: 503); }
});

// ---------------------------------------------------------------- reads ---

app.MapGet("/price_list", async (Db db, HttpRequest r) =>
    Results.Ok(await db.QueryAsync("P_SO_PRICE_LIST_PKG_select_price_list", new[]
    {
        Num("@so_price_list_header_id", L(r.Query["header_id"])),
        Num("@customer_id",             L(r.Query["customer_id"])),
        Text("@search",                 S(r.Query["search"]), 100),
        Dt("@as_of",                    D(r.Query["as_of"])),
        Text("@logempcd",               Who(r), 15)
    })));

// Declared before /{id} so "price" is not read as an id.
app.MapGet("/price_list/price", async (Db db, HttpRequest r) =>
{
    var headerId = L(r.Query["header_id"]);
    // design_no is the identifier now; article is its mirror column, which is
    // what get_price still filters on - so either query name works here.
    var article = S(r.Query["design_no"]) ?? S(r.Query["article"]);
    if (headerId is null || article is null)
        return Results.BadRequest(new { error = "header_id and design_no are required." });

    var rows = await db.QueryAsync("P_SO_PRICE_LIST_PKG_get_price", new[]
    {
        Num("@so_price_list_header_id", headerId),
        Text("@article",                article, 30),   // text: 487 lines look like '255484AA/11'
        Text("@color_tier",             S(r.Query["color_tier"]), 30),
        Int32P("@qty",                  I(r.Query["qty"])),
        Chr("@qty_unit",                S(r.Query["qty_unit"]) ?? "M", 2),
        Chr("@currency",                S(r.Query["currency"]), 3),
        Dt("@as_of",                    D(r.Query["as_of"])),
        Flag("@include_inactive",       r.Query["include_inactive"] == "1"),
        Text("@logempcd",               Who(r), 15)
    });

    // 1 line resolves itself; 2+ go back for the user to choose.
    return Results.Ok(new
    {
        match_count = rows.Count,
        resolved = rows.Count == 1 ? rows[0] : null,
        needs_selection = rows.Count > 1,
        matches = rows
    });
});

// Customer list of values. Lives in the LOV schema per house convention, so
// the name is schema-qualified rather than resolved against SO.
app.MapGet("/lov/customer", async (Db db, HttpRequest r) =>
    Results.Ok(await db.QueryAsync("LOV.P_LOV_PKG_select_customer_list", new[]
    {
        Text("@p_filter_text", S(r.Query["filter"]) ?? "", 100),
        Flag("@p_active_only", r.Query["active_only"] == "1"),
        Int32P("@p_top_n",     I(r.Query["top_n"]) ?? 200)
    })));

app.MapGet("/price_list/color_tier", async (Db db, HttpRequest r) =>
    Results.Ok(await db.QueryAsync("P_SO_PRICE_LIST_PKG_select_color_tier", new[]
    {
        Num("@so_price_list_header_id", L(r.Query["header_id"])),
        Text("@logempcd",               Who(r), 15)
    })));

app.MapGet("/price_list/{id:long}/detail", async (Db db, HttpRequest r, long id) =>
    Results.Ok(await db.QueryAsync("P_SO_PRICE_LIST_PKG_select_price_list_detail", new[]
    {
        Num("@so_price_list_header_id", id),
        Text("@design_no",              S(r.Query["design_no"]), 60),
        Text("@article",                S(r.Query["article"]), 30),
        Text("@search",                 S(r.Query["search"]), 100),
        Flag("@conflicts_only",         r.Query["conflicts_only"] == "1"),
        Text("@logempcd",               Who(r), 15)
    })));

// --------------------------------------------------------------- writes ---

app.MapPost("/price_list/validate_name", async (Db db, HttpRequest r, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_validate_price_list_name", new[]
    {
        Text("@list_name",              SJ(b, "list_name"), 60),
        Num("@so_price_list_header_id", LJ(b, "header_id")),
        Text("@logempcd",               Who(r), 15)
    })));

app.MapPost("/price_list", async (Db db, HttpRequest r, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_update_price_list", new[]
    {
        Num("@so_price_list_header_id", LJ(b, "header_id")),
        Text("@list_name",              SJ(b, "list_name"), 60),
        Text("@list_desc",              SJ(b, "list_desc"), 400),
        Num("@customer_id",             LJ(b, "customer_id")),
        Text("@customer_excel",         SJ(b, "customer_excel"), 120),
        Dt("@list_date",                DJ(b, "list_date")),
        Dt("@valid_from",               DJ(b, "valid_from")),
        Dt("@valid_to",                 DJ(b, "valid_to")),
        Text("@terms",                  SJ(b, "terms"), 60),
        Text("@quote_ref",              SJ(b, "quote_ref"), 200),
        Text("@sonoid",                 SJ(b, "sonoid"), 30),
        Num("@so_line_id",              LJ(b, "so_line_id")),
        Text("@notes",                  SJ(b, "notes"), 500),
        Text("@logempcd",               Who(r), 15)
    })));

app.MapPost("/price_list/detail", async (Db db, HttpRequest r, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_update_price_list_detail", new[]
    {
        Num("@so_price_list_detail_id", LJ(b, "detail_id")),
        Num("@so_price_list_header_id", LJ(b, "header_id")),
        Int32P("@set_no",               IJ(b, "set_no")),
        Text("@article",                SJ(b, "article"), 30),
        Text("@design_no",              SJ(b, "design_no"), 60),
        Text("@article_variant",        SJ(b, "article_variant"), 20),
        Text("@fabric_name",            SJ(b, "fabric_name"), 120),
        Text("@composition",            SJ(b, "composition"), 200),
        Text("@full_width_cm",          SJ(b, "full_width_cm"), 30),
        Text("@usable_width_cm",        SJ(b, "usable_width_cm"), 30),
        Text("@weight_gsm",             SJ(b, "weight_gsm"), 30),
        Text("@moq",                    SJ(b, "moq"), 30),
        Int32P("@qty_min",              IJ(b, "qty_min")),
        Int32P("@qty_max",              IJ(b, "qty_max")),
        Chr("@qty_unit",                SJ(b, "qty_unit") ?? "M", 2),
        Text("@color_tier",             SJ(b, "color_tier"), 30),
        Chr("@currency",                SJ(b, "currency"), 3),
        Dec("@price",                   DecJ(b, "price")),
        Int32P("@line_no",              IJ(b, "line_no")),
        Int32P("@after_line_no",        IJ(b, "after_line_no")),
        Chr("@active",                  SJ(b, "active"), 1),
        Text("@notes",                  SJ(b, "notes"), 500),
        Text("@logempcd",               Who(r), 15)
    })));

// One grid row stands for up to 12 detail lines; moving its article or qty band
// has to move all of them together or the row splits in two.
app.MapPost("/price_list/{id:long}/row", async (Db db, HttpRequest r, long id, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_update_price_list_row", new[]
    {
        Num("@so_price_list_header_id", id),
        Text("@design_no",              SJ(b, "design_no"), 60),
        Text("@article",                SJ(b, "article"), 30),
        Text("@article_variant",        SJ(b, "article_variant"), 20),
        Int32P("@qty_min",              IJ(b, "qty_min")),
        Int32P("@qty_max",              IJ(b, "qty_max")),
        Chr("@qty_unit",                SJ(b, "qty_unit") ?? "M", 2),
        Text("@new_design_no",          SJ(b, "new_design_no"), 60),
        Text("@new_article",            SJ(b, "new_article"), 30),
        Text("@new_article_variant",    SJ(b, "new_article_variant"), 20),
        Int32P("@new_qty_min",          IJ(b, "new_qty_min")),
        Int32P("@new_qty_max",          IJ(b, "new_qty_max")),
        Flag("@clear_qty_max",          BJ(b, "clear_qty_max")),
        Chr("@new_qty_unit",            SJ(b, "new_qty_unit"), 2),
        Text("@fabric_name",            SJ(b, "fabric_name"), 120),
        Text("@composition",            SJ(b, "composition"), 200),
        Text("@full_width_cm",          SJ(b, "full_width_cm"), 30),
        Text("@usable_width_cm",        SJ(b, "usable_width_cm"), 30),
        Text("@weight_gsm",             SJ(b, "weight_gsm"), 30),
        Text("@moq",                    SJ(b, "moq"), 30),
        Chr("@active",                  SJ(b, "active"), 1),
        Text("@logempcd",               Who(r), 15)
    })));

// Right-click Copy then Insert. A grid row is a whole set, so this duplicates
// every line behind it - all tiers, both currencies - and lands it after the
// row that was right-clicked. after_set_no 0 puts it first.
app.MapPost("/price_list/{id:long}/set/copy", async (Db db, HttpRequest r, long id, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_copy_price_list_set", new[]
    {
        Num("@so_price_list_header_id", id),
        Int32P("@source_set_no",        IJ(b, "source_set_no")),
        Int32P("@after_set_no",         IJ(b, "after_set_no")),
        Text("@logempcd",               Who(r), 15)
    })));

// Right-click Delete. Soft, like every delete here: delete_mark goes to 'Y' and
// the rows stay in the table, out of sight of every select.
app.MapPost("/price_list/{id:long}/set/delete", async (Db db, HttpRequest r, long id, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_delete_price_list_set", new[]
    {
        Num("@so_price_list_header_id", id),
        Int32P("@set_no",               IJ(b, "set_no")),
        Text("@logempcd",               Who(r), 15)
    })));

app.MapPost("/price_list/{id:long}/grid_shape", async (Db db, HttpRequest r, long id, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_update_price_list_grid_shape", new[]
    {
        Num("@so_price_list_header_id", id),
        Text("@tier_set",               SJ(b, "tier_set"), 200),
        Text("@currency_set",           SJ(b, "currency_set"), 20),
        Chr("@hide_inactive",           SJ(b, "hide_inactive"), 1),
        Text("@logempcd",               Who(r), 15)
    })));

app.MapPost("/price_list/{id:long}/copy", async (Db db, HttpRequest r, long id, System.Text.Json.JsonElement b) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_copy_price_list", new[]
    {
        Num("@source_header_id", id),
        Text("@list_name",       SJ(b, "list_name"), 60),
        Num("@customer_id",      LJ(b, "customer_id")),
        Dt("@valid_from",        DJ(b, "valid_from")),
        Dt("@valid_to",          DJ(b, "valid_to")),
        Text("@logempcd",        Who(r), 15)
    })));

// before /{id} for the same reason as /price
app.MapDelete("/price_list/detail/{detailId:long}", async (Db db, HttpRequest r, long detailId) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_delete_price_list_detail", new[]
    {
        Num("@so_price_list_detail_id", detailId),
        Text("@logempcd",               Who(r), 15)
    })));

app.MapDelete("/price_list/{id:long}", async (Db db, HttpRequest r, long id) =>
    Results.Ok(await db.SingleAsync("P_SO_PRICE_LIST_PKG_delete_price_list", new[]
    {
        Num("@so_price_list_header_id", id),
        Text("@logempcd",               Who(r), 15)
    })));

app.Run();

/* Thrown when a request carries a value that cannot be parsed. Declared after
   the top-level statements, as C# requires. */
sealed class BadInputException : Exception
{
    public BadInputException(string message) : base(message) { }
}
