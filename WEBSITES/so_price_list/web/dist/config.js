/* Runtime configuration for the Eschler Price Book.
   Edit this file on the server to move the API; no rebuild is needed.
   It is loaded before the app bundle and is deliberately not fingerprinted.

   Relative base = same IIS site as this app, so there is no CORS and no
   hard-coded host or port. */
window.__PRICE_LIST_API__ = '/SoPriceListApi';
