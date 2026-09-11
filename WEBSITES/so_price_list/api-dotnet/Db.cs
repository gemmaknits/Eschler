using System.Data;
using Microsoft.Data.SqlClient;

namespace SoPriceListApi;

/// <summary>
/// Thin wrapper over the SO_PRICE_LIST_PKG procedures. Nothing in this app
/// writes SQL - every call is a stored procedure, so the database keeps control
/// of the rules and the API stays a transport.
/// </summary>
public sealed class Db
{
    private readonly string _cs;
    private readonly string _schema;

    public Db(IConfiguration cfg)
    {
        _cs = cfg.GetConnectionString("Gemmasoft")
              ?? throw new InvalidOperationException("ConnectionStrings:Gemmasoft is not configured.");
        _schema = cfg["PriceList:ProcSchema"] ?? "SO";
    }

    /// <summary>A parameter to send. Value null means SQL NULL.</summary>
    public readonly record struct P(string Name, SqlDbType Type, object? Value, int Size = 0);

    public static P Text(string name, string? v, int size)   => new(name, SqlDbType.NVarChar, v, size);
    public static P Chr(string name, string? v, int size)    => new(name, SqlDbType.Char, v, size);
    public static P Num(string name, long? v)                => new(name, SqlDbType.BigInt, v);
    public static P Int32P(string name, int? v)              => new(name, SqlDbType.Int, v);
    public static P Dec(string name, decimal? v)             => new(name, SqlDbType.Decimal, v);
    public static P Dt(string name, DateTime? v)             => new(name, SqlDbType.Date, v);
    public static P Flag(string name, bool v)                => new(name, SqlDbType.Bit, v);

    /// <summary>
    /// Run a procedure and return its rows as dictionaries, ready to serialise.
    /// </summary>
    public async Task<List<Dictionary<string, object?>>> QueryAsync(
        string proc, IEnumerable<P> parameters, CancellationToken ct = default)
    {
        var rows = new List<Dictionary<string, object?>>();

        await using var cn = new SqlConnection(_cs);
        await cn.OpenAsync(ct);

        // A name may carry its own schema ("LOV.P_LOV_PKG_..."), because lists
        // of values live in the LOV schema by house convention, not in SO.
        var idx = proc.IndexOf('.');
        var qualified = idx > 0
            ? $"[{proc[..idx]}].[{proc[(idx + 1)..]}]"
            : $"[{_schema}].[{proc}]";

        await using var cmd = new SqlCommand(qualified, cn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 120
        };

        foreach (var p in parameters)
        {
            var sp = new SqlParameter(p.Name, p.Type);
            if (p.Size > 0) sp.Size = p.Size;
            if (p.Type == SqlDbType.Decimal) { sp.Precision = 18; sp.Scale = 4; }
            sp.Value = p.Value ?? DBNull.Value;
            cmd.Parameters.Add(sp);
        }

        await using var rd = await cmd.ExecuteReaderAsync(ct);
        while (await rd.ReadAsync(ct))
        {
            var row = new Dictionary<string, object?>(rd.FieldCount, StringComparer.Ordinal);
            for (var i = 0; i < rd.FieldCount; i++)
            {
                var value = await rd.IsDBNullAsync(i, ct) ? null : rd.GetValue(i);
                // char(2)/char(3) columns come back padded; the client compares
                // them as 'M', 'USD' etc, so trim at the boundary.
                if (value is string s) value = s.TrimEnd();
                row[rd.GetName(i)] = value;
            }
            rows.Add(row);
        }
        return rows;
    }

    public async Task<Dictionary<string, object?>> SingleAsync(
        string proc, IEnumerable<P> parameters, CancellationToken ct = default)
        => (await QueryAsync(proc, parameters, ct)).FirstOrDefault() ?? new();

    public async Task<bool> CanConnectAsync(CancellationToken ct = default)
    {
        await using var cn = new SqlConnection(_cs);
        await cn.OpenAsync(ct);
        return true;
    }
}
