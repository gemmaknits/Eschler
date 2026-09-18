# SoPriceListApi

ASP.NET Core (net8.0) API over the `SO_PRICE_LIST_PKG` stored procedures in
`gemmasoft`. The browser never sees a table or a query — every route calls one
procedure.

This replaced an earlier Node/Express API that served the same purpose. The move
was about hosting, not language: the published `web.config` uses
`AspNetCoreModuleV2` with `hostingModel="inprocess"`, so IIS owns the process —
it starts it, recycles it and stops it, and the `app_offline.htm` deployment
handshake works. Node on IIS needs either iisnode or an ARR reverse proxy to a
process supervised separately, which is a second lifecycle to maintain on a box
where everything else is an app pool.

## Build and deploy

    dotnet publish -c Release

`RuntimeIdentifier` and `SelfContained` are declared **in the csproj**, not on
the command line — a plain `dotnet publish` that loses them produces an app the
server cannot start (HTTP 500.31). Self-contained matters here because the
existing ESH WebAPI on the same box targets net7.0 and must not be disturbed.

Deploy by dropping `app_offline.htm` into the target, robocopying, then removing
it. Robocopy exit code 3 means success.

Configuration lives in `appsettings.json`: the connection string, and
`PriceList:ProcSchema` (default `SO`).

## Routes

Procedures are resolved against `PriceList:ProcSchema` unless the name carries
its own schema — lists of values live in `LOV` by house convention.

| Method | Path | Procedure |
|--------|------|-----------|
| GET | `/health` | connection check |
| GET | `/price_list` | `select_price_list` |
| GET | `/price_list/price` | `get_price` |
| GET | `/price_list/design` | `select_design` |
| GET | `/price_list/design_list` | `select_design_list` |
| GET | `/price_list/color_tier` | `select_color_tier` |
| GET | `/price_list/{id}/detail` | `select_price_list_detail` |
| GET | `/price_list/{id}/customers` | `select_price_list_customer` |
| GET | `/uom` | `select_uom` |
| GET | `/lov/customer` | `LOV.P_LOV_PKG_select_customer_list` |
| POST | `/price_list` | `update_price_list` (omit `header_id` to insert) |
| POST | `/price_list/detail` | `update_price_list_detail` |
| POST | `/price_list/validate_name` | `validate_price_list_name` |
| POST | `/price_list/{id}/row` | `update_price_list_row` |
| POST | `/price_list/{id}/set/copy` | `copy_price_list_set` |
| POST | `/price_list/{id}/set/insert` | `insert_price_list_set` |
| POST | `/price_list/{id}/set/delete` | `delete_price_list_set` |
| POST | `/price_list/{id}/customers` | `assign_price_list_customer` |
| POST | `/price_list/{id}/verified` | `set_price_list_verified` |
| POST | `/price_list/{id}/grid_shape` | `update_price_list_grid_shape` |
| POST | `/price_list/{id}/copy` | `copy_price_list` |
| DELETE | `/price_list/{id}` | `delete_price_list` (soft) |
| DELETE | `/price_list/detail/{detailId}` | `delete_price_list_detail` (soft) |
| DELETE | `/price_list/{id}/customers/{customerId}` | `unassign_price_list_customer` |

Unqualified names above are prefixed `P_SO_PRICE_LIST_PKG_`.

## The one route that matters

`GET /price_list/price` is the order-entry lookup, called by the VB.NET client.
It returns:

    { match_count, resolved, needs_selection, matches[] }

- `match_count: 1` → `resolved` holds the line; the price applies automatically.
- `match_count: 2+` → `needs_selection: true`; show `matches` and let the user pick.

That is deliberate. The business key is not unique in the imported data, and the
decision was to keep every line and ask rather than silently choose one.

## Who did it

Every procedure takes `@logempcd` last. The API reads it from the `X-Emp-Cd`
header, which the web app sets from the `user_id` URL parameter passed by the
VB.NET client. It lands in `created_by` / `updated_by`.

## design_no is text

Design numbers carry codes like `255484AA/11`, so `@design_no` is bound as
`NVarChar` everywhere. Binding it as an integer rejects those rows. `article` is
kept as a mirror of `design_no` because `get_price` is called by the VB.NET
order-entry client with `@article`, and that contract must not break.

## Things that bite

- **`QUOTED_IDENTIFIER` must be ON** for anything writing to
  `so_price_list_detail` — it has filtered indexes, and SQL Server refuses the
  write with `Msg 1934` otherwise. sqlcmd defaults it OFF where SSMS defaults it
  ON, so it is set explicitly at the top of every `.sql` file.
- **Collation.** The ERP tables (`dbo.dm`, `dbo.designs`, `dbo.customers`,
  `dbo.uom`) are `Thai_CI_AI`. Comparing them against the price-list tables
  needs `COLLATE DATABASE_DEFAULT` or it fails with `Msg 468`.
- `dbo.so`, `dbo.customers`, `dbo.dm`, `dbo.designs` and `dbo.uom` belong to
  other systems: **read only, never write**. Note that `dbo.so` is a string
  prefix of `dbo.so_price_list` — a find-and-replace must match the longer name
  first.
