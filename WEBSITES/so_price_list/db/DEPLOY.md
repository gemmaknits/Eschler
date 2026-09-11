# Eschler Price Book — deployment

Three pieces. Only the middle one needs a manual step in IIS.

## 1. Database — done

`SO_PRICE_LIST_PKG`, 13 procedures in schema `SO` of `gemmasoft`.
Tables `dbo.so_price_list_header` / `dbo.so_price_list_detail`.
Re-deploy with the scripts in this folder, in order:

    P_SO_PRICE_LIST_PKG_01_select.sql
    P_SO_PRICE_LIST_PKG_02_update.sql
    P_SO_PRICE_LIST_PKG_03_delete_copy.sql
    P_SO_PRICE_LIST_PKG_04_grid_shape.sql

## 2. API — deployed to `V:\ESHWeb\SoPriceListApi`, needs one IIS step

ASP.NET Core 8, published **self-contained win-x64**, so the server's installed
.NET runtimes do not matter and the existing net7.0 `V:\WebAPI` is untouched.
Hosted by IIS through `AspNetCoreModuleV2`, exactly like `WebAPI` is.

**The manual step:** in IIS Manager, under the site that serves `ESHWeb`,
right-click → *Add Application*:

    Alias            SoPriceListApi
    Physical path    V:\ESHWeb\SoPriceListApi
    Application pool  a No Managed Code pool (or any; the runtime is bundled)

Give the pool identity read access to the folder. Then check:

    http://<host>/SoPriceListApi/health     ->  {"ok":true,"db":"connected"}

The SQL connection string lives in `appsettings.json` in that folder.

## 3. Web app — deployed to `V:\ESHWeb\SoPriceList`

Create-React-App-shaped build (`static/`, `index.html`, `web.config`), same as
its siblings `SoStatus` and `CmrStatus`.

`config.js` sets the API base and is deliberately **not** fingerprinted, so the
API can be moved by editing that one line — no rebuild:

    window.__PRICE_LIST_API__ = '/SoPriceListApi';

A relative path means same-origin, so there is no CORS and no hard-coded host.
If the API ever moves off this site, put a full URL there instead.

## Launching from VB.NET

    http://<host>/SoPriceList/?user_id=SURES

| Parameter   | Effect                                                        |
|-------------|---------------------------------------------------------------|
| `user_id`   | Shown read-only; becomes `created_by` / `updated_by` on edits  |
| `header_id` | Opens straight to that price list                              |
| `list_name` | Same, by name instead of id                                    |
| `article`   | Pre-fills the article filter                                   |

Example for order entry:

    http://<host>/SoPriceList/?user_id=SURES&list_name=CENTER&article=255484
