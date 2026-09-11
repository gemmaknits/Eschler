# so-price-list-api

Node/Express API over the `SO_PRICE_LIST_PKG` stored procedures in `gemmasoft`.
The browser never sees a table or a query - every route calls one procedure.

## Run

    npm install
    npm start            # listens on 3001, routes under /price_list

Overridable via environment: `PORT`, `DB_SERVER`, `DB_NAME`, `DB_USER`,
`DB_PASSWORD`, `DB_PORT`, `DB_PROC_SCHEMA` (default `SO`).

## Routes

| Method | Path                          | Procedure                                        |
|--------|-------------------------------|--------------------------------------------------|
| GET    | `/health`                     | connection check                                  |
| GET    | `/price_list`                 | `select_price_list`                               |
| GET    | `/price_list/price`           | `get_price`                                       |
| GET    | `/price_list/customer`        | `select_customer`                                 |
| GET    | `/price_list/color_tier`      | `select_color_tier`                               |
| GET    | `/price_list/:id/detail`      | `select_price_list_detail`                        |
| POST   | `/price_list`                 | `update_price_list` (omit `header_id` to insert)  |
| POST   | `/price_list/detail`          | `update_price_list_detail`                        |
| POST   | `/price_list/validate_name`   | `validate_price_list_name`                        |
| POST   | `/price_list/:id/copy`        | `copy_price_list`                                 |
| DELETE | `/price_list/:id`             | `delete_price_list`   (soft)                      |
| DELETE | `/price_list/detail/:id`      | `delete_price_list_detail` (soft)                 |

## The one route that matters

`GET /price_list/price` is the order-entry lookup. It returns:

    { match_count, resolved, needs_selection, matches[] }

- `match_count: 1` -> `resolved` holds the line; price applies automatically.
- `match_count: 2+` -> `needs_selection: true`; show `matches` and let the user pick.

That is deliberate. The business key is not unique in the imported data, and the
decision was to keep every line and ask rather than silently choose one.

## Who did it

Every procedure takes `@logempcd`. The API reads it from the `X-Emp-Cd` header,
which the web app sets from the `user_id` URL parameter passed by the VB.NET
client. It lands in `created_by` / `updated_by`.

## article is text

487 of the 4,468 price lines carry codes like `255484AA/11`. `article` is bound
as `NVarChar` everywhere. Binding it as an integer rejects those rows.
