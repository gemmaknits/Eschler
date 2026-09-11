// Every call goes through the SO_PRICE_LIST_PKG procedures - the browser never
// sees a table or a query. In dev, vite proxies /price_list to the API host.
//
// In production the base comes from public/config.js, which is deployed
// alongside the bundle and NOT fingerprinted - so the API can be moved by
// editing one file on the server, with no rebuild.
const BASE = import.meta.env.DEV
  ? ''
  : (window.__PRICE_LIST_API__ || 'http://172.16.3.2:3001');

// The page is opened directly from the VB.NET WinForms client, which passes the
// signed-in user on the URL:  .../SoPriceList/?user_id=SURES
// That value rides along as X-Emp-Cd and lands in created_by / updated_by, so
// edits are attributed to the person in the desktop app - there is no login here.
const urlParams = new URLSearchParams(window.location.search);
const fromUrl = (urlParams.get('user_id') || urlParams.get('userid') || '').trim();

// A user_id on the URL always wins; it is the caller's assertion of who this is.
let empCd = (fromUrl || localStorage.getItem('eschler_empcd') || '').slice(0, 15);

// Whether the identity was supplied by the caller, so the UI can show it as
// given rather than offering it as a free-text field.
export const isUserFromUrl = () => Boolean(fromUrl);
export const getEmpCd = () => empCd;
export const setEmpCd = v => {
  empCd = (v || '').slice(0, 15);
  localStorage.setItem('eschler_empcd', empCd);
};
if (fromUrl) localStorage.setItem('eschler_empcd', empCd);

/** Other launch parameters the WinForms caller may pass. */
export const launch = {
  headerId: urlParams.get('header_id') || urlParams.get('list_id') || null,
  listName: urlParams.get('list_name') || null,
  article:  urlParams.get('article') || null
};

async function call(path, options = {}) {
  const res = await fetch(BASE + path, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      'X-Emp-Cd': empCd,
      ...(options.headers || {})
    }
  });

  const text = await res.text();
  let body = null;
  try { body = text ? JSON.parse(text) : null; } catch { /* non-JSON error page */ }

  if (!res.ok) {
    // The procs RAISERROR in plain language; the API forwards that as .error.
    throw new Error(body?.error || `Request failed (${res.status}).`);
  }
  return body;
}

const qs = params => {
  const p = new URLSearchParams();
  Object.entries(params).forEach(([k, v]) => {
    if (v !== null && v !== undefined && v !== '') p.append(k, v);
  });
  const s = p.toString();
  return s ? `?${s}` : '';
};

export const api = {
  listPriceLists: (params = {}) => call(`/price_list${qs(params)}`),

  listDetail: (headerId, params = {}) =>
    call(`/price_list/${headerId}/detail${qs(params)}`),

  // 1 match resolves itself; 2+ come back as needs_selection for the user
  getPrice: (params) => call(`/price_list/price${qs(params)}`),

  // Customer list of values — LOV.P_LOV_PKG_select_customer_list
  lovCustomer: (params = {}) => call(`/lov/customer${qs(params)}`),

  listColorTiers: (params = {}) => call(`/price_list/color_tier${qs(params)}`),

  validateName: (body) =>
    call('/price_list/validate_name', { method: 'POST', body: JSON.stringify(body) }),

  saveHeader: (body) =>
    call('/price_list', { method: 'POST', body: JSON.stringify(body) }),

  saveDetail: (body) =>
    call('/price_list/detail', { method: 'POST', body: JSON.stringify(body) }),

  // row-level edit: moves every price line sitting under one grid row
  saveRow: (headerId, body) =>
    call(`/price_list/${headerId}/row`, { method: 'POST', body: JSON.stringify(body) }),

  saveGridShape: (headerId, body) =>
    call(`/price_list/${headerId}/grid_shape`, { method: 'POST', body: JSON.stringify(body) }),

  copyList: (headerId, body) =>
    call(`/price_list/${headerId}/copy`, { method: 'POST', body: JSON.stringify(body) }),

  deleteList: (headerId) =>
    call(`/price_list/${headerId}`, { method: 'DELETE' }),

  deleteDetail: (detailId) =>
    call(`/price_list/detail/${detailId}`, { method: 'DELETE' })
};

/**
 * The API returns TALL rows (one per currency); the grid is WIDE (colour tiers
 * as columns under USD and again under THB). This pivots one list's detail
 * rows into grid rows, keeping EVERY line behind a cell - a cell backed by two
 * lines is the conflict the user has to resolve, not something to collapse.
 */
export const DEFAULT_TIERS = ['PFE/PFD', 'All_colors'];
export const DEFAULT_CURRENCIES = ['USD'];

const splitSet = s =>
  (s || '').split(',').map(x => x.trim()).filter(Boolean);

/**
 * Columns a list shows. Derived from the header, NOT from the rows - an empty
 * list has no rows to derive from, and a tier the user added but has not priced
 * yet must still get a column to type into.
 */
export function gridShape(header) {
  const tiers = splitSet(header?.tier_set);
  const currencies = splitSet(header?.currency_set);
  return {
    tiers: tiers.length ? tiers : DEFAULT_TIERS,
    currencies: currencies.length ? currencies : DEFAULT_CURRENCIES
  };
}

export function pivotToGrid(rows) {
  const byKey = new Map();
  const tiers = new Set();

  for (const d of rows) {
    tiers.add(d.color_tier);
    const key = [d.article, d.article_variant || '', d.qty_min,
                 d.qty_max ?? '', (d.qty_unit || '').trim()].join('');

    if (!byKey.has(key)) {
      byKey.set(key, {
        key,
        article: d.article,
        article_variant: d.article_variant || '',
        qty_min: d.qty_min,
        qty_max: d.qty_max,
        qty_unit: (d.qty_unit || '').trim(),
        fabric_name: d.fabric_name, composition: d.composition,
        full_width_cm: d.full_width_cm, usable_width_cm: d.usable_width_cm,
        weight_gsm: d.weight_gsm, moq: d.moq,
        source_row: d.source_row,
        cells: {}
      });
    }
    const row = byKey.get(key);
    const cellKey = `${(d.currency || '').trim()}|${d.color_tier}`;
    (row.cells[cellKey] ||= []).push(d);
  }

  const gridRows = [...byKey.values()].sort(
    (a, b) => a.article.localeCompare(b.article) || a.qty_min - b.qty_min
  );

  // tiersInData is only a safety net: if a line exists under a tier the header
  // forgot to declare, its column still appears rather than the price vanishing.
  return { rows: gridRows, tiersInData: [...tiers] };
}

export const TIER_ORDER =
  ['PFE/PFD', 'PFE', 'PFD', 'Greige', 'White', 'Light', 'Medium', 'Dark', 'All_colors'];

export const orderTiers = list => {
  const seen = [...new Set(list)];
  return [
    ...TIER_ORDER.filter(t => seen.includes(t)),
    ...seen.filter(t => !TIER_ORDER.includes(t)).sort()
  ];
};
