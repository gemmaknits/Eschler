import { useState, useEffect, useCallback, useRef } from 'react';
import {
  api, pivotToGrid, gridShape, orderTiers,
  getEmpCd, setEmpCd, isUserFromUrl, launch
} from './api';
import PriceGrid from './PriceGrid.jsx';
import ConflictPicker from './ConflictPicker.jsx';
import ColumnsMenu from './ColumnsMenu.jsx';
import PriceListNav from './PriceListNav.jsx';
import HeaderForm from './HeaderForm.jsx';
import CustomerField from './CustomerField.jsx';

export default function App() {
  const [lists, setLists]       = useState([]);
  const [headerId, setHeaderId] = useState(null);
  const [grid, setGrid]         = useState({ rows: [], tiersInData: [] });
  const [drafts, setDrafts]     = useState([]);      // rows not yet in the DB
  const [shape, setShape]       = useState({ tiers: [], currencies: [] });
  const [allTiers, setAllTiers] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [busy, setBusy]         = useState(false);
  const [error, setError]       = useState(null);
  const [filter, setFilter]     = useState('');
  const [conflictsOnly, setConflictsOnly] = useState(false);
  const [picker, setPicker]     = useState(null);
  const [showCols, setShowCols] = useState(false);
  const [navSearch, setNavSearch] = useState('');
  const [editHeader, setEditHeader] = useState(null);  // {mode:'new'} | {mode:'edit'}
  const [picked, setPicked]     = useState({});
  const [flash, setFlash]       = useState('');
  const [emp, setEmp]           = useState(getEmpCd());
  const flashTimer = useRef(null);

  const say = useCallback(msg => {
    setFlash(msg);
    clearTimeout(flashTimer.current);
    flashTimer.current = setTimeout(() => setFlash(''), 1800);
  }, []);

  /* ---- lists + the global tier vocabulary ---- */
  useEffect(() => {
    let alive = true;
    Promise.all([api.listPriceLists(), api.listColorTiers()])
      .then(([rows, tierRows]) => {
        if (!alive) return;
        setLists(rows);
        setAllTiers(tierRows.map(t => t.color_tier));
        if (!rows.length) { setLoading(false); return; }
        const wanted =
          (launch.headerId && rows.find(r =>
             r.so_price_list_header_id === Number(launch.headerId))) ||
          (launch.listName && rows.find(r =>
             r.list_name?.toLowerCase() === launch.listName.toLowerCase())) ||
          rows[0];
        setHeaderId(wanted.so_price_list_header_id);
        if (launch.article) setFilter(launch.article);
      })
      .catch(err => { if (alive) { setError(err.message); setLoading(false); } });
    return () => { alive = false; };
  }, []);

  const header = lists.find(l => l.so_price_list_header_id === headerId);

  /* Columns come from the header, so an empty list still has somewhere to type. */
  useEffect(() => {
    if (header) setShape(gridShape(header));
  }, [header]);

  const reload = useCallback(() => {
    if (headerId == null) return;
    setLoading(true);
    setError(null);
    api.listDetail(headerId, {
        search: filter || undefined,
        conflicts_only: conflictsOnly ? 1 : undefined
      })
      .then(rows => { setGrid(pivotToGrid(rows)); setDrafts([]); setLoading(false); })
      .catch(err => { setError(err.message); setLoading(false); });
  }, [headerId, filter, conflictsOnly]);

  useEffect(() => {
    const t = setTimeout(reload, filter ? 250 : 0);
    return () => clearTimeout(t);
  }, [reload, filter]);

  const refreshLists = () => api.listPriceLists().then(setLists).catch(() => {});

  /* A tier with prices but no column would hide money, so union them. */
  const tiers = orderTiers([...shape.tiers, ...(grid.tiersInData || [])]);
  const currencies = shape.currencies.length ? shape.currencies : ['USD'];
  const rows = [...grid.rows, ...drafts];

  /* ---- add a line ---------------------------------------------------------
     A draft lives only in the browser until a price is typed. A detail row
     cannot exist without a currency, tier and price, so there is nothing to
     insert until then - and an abandoned draft leaves no empty row behind. */
  const addRow = useCallback(() => {
    const last = rows[rows.length - 1];
    setDrafts(d => [...d, {
      key: `draft-${Date.now()}-${d.length}`,
      isDraft: true,
      article: last?.article || '',
      article_variant: '',
      qty_min: last?.qty_max != null ? Number(last.qty_max) + 1 : 0,
      qty_max: null,
      qty_unit: last?.qty_unit || 'M',
      fabric_name: last?.fabric_name || '',
      composition: last?.composition || '',
      full_width_cm: last?.full_width_cm || '',
      usable_width_cm: last?.usable_width_cm || '',
      weight_gsm: last?.weight_gsm || '',
      moq: last?.moq || '',
      source_row: null,
      cells: {}
    }]);
    say('New line — fill the article and a price, then it saves');
  }, [rows, say]);

  const patchDraft = (key, patch) =>
    setDrafts(d => d.map(r => (r.key === key ? { ...r, ...patch } : r)));

  /* ---- edit a row-level field ---- */
  const editRow = useCallback(async (row, col, raw) => {
    const value = col.num ? (raw === '' ? null : parseInt(raw, 10)) : raw;
    if (col.num && raw !== '' && Number.isNaN(value)) return;

    if (row.isDraft) { patchDraft(row.key, { [col.key]: value }); return; }

    setBusy(true);
    try {
      await api.saveRow(headerId, {
        article: row.article,
        article_variant: row.article_variant,
        qty_min: row.qty_min,
        qty_max: row.qty_max,
        qty_unit: row.qty_unit,
        [`new_${col.key}`]: col.key.startsWith('qty') || col.key.startsWith('article')
          ? value : undefined,
        // qty_max empty means "open upper bound", which is a real value, not a skip
        clear_qty_max: col.key === 'qty_max' && value === null,
        ...(['fabric_name','composition','full_width_cm','usable_width_cm',
             'weight_gsm','moq'].includes(col.key) ? { [col.key]: value } : {})
      });
      say(`${col.label} updated across the line`);
      reload();
    } catch (err) {
      setError(err.message);
    } finally { setBusy(false); }
  }, [headerId, reload, say]);

  /* ---- edit or create a price ---- */
  const editPrice = useCallback(async (row, currency, tier, value) => {
    const existing = row.cells?.[`${currency}|${tier}`] || [];
    const w = existing.length === 1
      ? existing[0]
      : existing.find(d => d.so_price_list_detail_id === picked[`${row.key}|${currency}|${tier}`])
        || existing[0];

    if (!row.isDraft && !w) {
      // no line under this cell yet - create one on this row's key
      if (!row.article) { setError('Give the line an article first.'); return; }
      setBusy(true);
      try {
        await api.saveDetail({
          header_id: headerId, article: row.article,
          article_variant: row.article_variant || null,
          qty_min: row.qty_min, qty_max: row.qty_max, qty_unit: row.qty_unit,
          color_tier: tier, currency, price: value,
          fabric_name: row.fabric_name, composition: row.composition,
          full_width_cm: row.full_width_cm, usable_width_cm: row.usable_width_cm,
          weight_gsm: row.weight_gsm, moq: row.moq
        });
        say(`Added ${row.article} · ${tier} · ${currency}`);
        reload(); refreshLists();
      } catch (err) { setError(err.message); } finally { setBusy(false); }
      return;
    }

    if (row.isDraft) {
      if (!row.article) { setError('Give the line an article first.'); return; }
      setBusy(true);
      try {
        await api.saveDetail({
          header_id: headerId, article: row.article,
          article_variant: row.article_variant || null,
          qty_min: row.qty_min ?? 0, qty_max: row.qty_max, qty_unit: row.qty_unit || 'M',
          color_tier: tier, currency, price: value,
          fabric_name: row.fabric_name, composition: row.composition,
          full_width_cm: row.full_width_cm, usable_width_cm: row.usable_width_cm,
          weight_gsm: row.weight_gsm, moq: row.moq
        });
        say(`Line saved — ${row.article} · ${tier} · ${currency}`);
        reload(); refreshLists();
      } catch (err) { setError(err.message); } finally { setBusy(false); }
      return;
    }

    setBusy(true);
    try {
      await api.saveDetail({ detail_id: w.so_price_list_detail_id, price: value });
      setGrid(g => ({
        ...g,
        rows: g.rows.map(r => r.key !== row.key ? r : {
          ...r,
          cells: Object.fromEntries(Object.entries(r.cells).map(([k, list]) => [
            k, list.map(d => d.so_price_list_detail_id === w.so_price_list_detail_id
                              ? { ...d, price: value } : d)
          ]))
        })
      }));
      say(`${row.article} · ${tier} · ${currency} → ${value}`);
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [headerId, picked, reload, say]);

  const retireLine = useCallback(async (detail) => {
    setBusy(true);
    try {
      const res = await api.deleteDetail(detail.so_price_list_detail_id);
      say(res.remaining_count === 1
        ? 'Conflict resolved — one price line remains'
        : `Line retired — ${res.remaining_count} still share this key`);
      setPicker(null); reload(); refreshLists();
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [reload, say]);

  const applyShape = useCallback(async (newTiers, newCurrencies) => {
    setShowCols(false);
    setShape({ tiers: newTiers, currencies: newCurrencies });
    try {
      await api.saveGridShape(headerId, {
        tier_set: newTiers.join(','),
        currency_set: newCurrencies.join(',')
      });
      say('Columns saved for this list');
      refreshLists();
    } catch (err) { setError(err.message); }
  }, [headerId, say]);

  /* ---- header created or edited -------------------------------------------
     Refresh the nav, then open the list. A brand-new list has no lines, so the
     grid falls back to the header's stored columns and offers the first row. */
  const headerSaved = useCallback(async (id, wasNew) => {
    setEditHeader(null);
    try {
      const rows = await api.listPriceLists();
      setLists(rows);
      setHeaderId(id);
      setNavSearch('');
      setDrafts([]);
      setPicked({});
      say(wasNew ? 'Price list created' : 'Price list saved');
    } catch (err) { setError(err.message); }
  }, [say]);

  const headerDeleted = useCallback(async (id, lineCount) => {
    setEditHeader(null);
    try {
      const rows = await api.listPriceLists();
      setLists(rows);
      // fall back to the first remaining list rather than an empty screen
      if (headerId === id) setHeaderId(rows[0]?.so_price_list_header_id ?? null);
      say(lineCount > 0
        ? `Price list deleted, with ${lineCount} lines`
        : 'Price list deleted');
    } catch (err) { setError(err.message); }
  }, [headerId, say]);

  /* Set (or clear) the customer straight from the meta strip, without opening
     the whole header dialog - 53 imported lists still need mapping and that is
     the only field most of them are missing.

     update_price_list is a full upsert, so every other field has to be sent
     back as it stands or it would be nulled out. */
  const setCustomer = useCallback(async (customerId) => {
    if (!header) return;
    setBusy(true);
    try {
      await api.saveHeader({
        header_id: header.so_price_list_header_id,
        list_name: header.list_name,
        list_desc: header.list_desc || null,
        customer_id: customerId,
        customer_excel: header.customer_excel || null,
        list_date: header.list_date || null,
        valid_from: header.valid_from || null,
        valid_to: header.valid_to || null,
        terms: header.terms || null,
        quote_ref: header.quote_ref || null,
        sonoid: header.sonoid || null,
        so_line_id: header.so_line_id ?? null,
        notes: header.notes || null
      });
      const rows = await api.listPriceLists();
      setLists(rows);
      say(customerId ? 'Customer set on this price list' : 'Customer cleared');
    } catch (err) { setError(err.message); }
    finally { setBusy(false); }
  }, [header, say]);

  const onEmp = e => { const v = e.target.value.toUpperCase(); setEmp(v); setEmpCd(v); };

  const conflictTotal = grid.rows.reduce(
    (a, r) => a + Object.values(r.cells).filter(c => c.length > 1).length, 0);

  return (
    <>
      <header className="topbar">
        <div className="brand">
          <h1>Eschler Price Book</h1>
          <span className="tag">header · detail</span>
        </div>

        <input type="search" placeholder="Filter article…"
               value={filter} onChange={e => setFilter(e.target.value)} />

        <button className={conflictsOnly ? 'on' : ''}
                onClick={() => setConflictsOnly(v => !v)}>
          Conflicts only
        </button>

        <button onClick={() => setShowCols(v => !v)}>
          Columns <span className="dimcount">{tiers.length}×{currencies.length}</span>
        </button>

        <div className="spacer" />
        {busy && <span className="saving" title="Saving…" />}
        <div className="pick">
          <label htmlFor="empcd">User</label>
          {isUserFromUrl()
            ? <span className="mono userchip" title="Passed as user_id on the URL">{emp}</span>
            : <input id="empcd" className="emp" value={emp} onChange={onEmp}
                     maxLength={15} placeholder="user_id" />}
        </div>
      </header>

      <div className="shell">
        <PriceListNav
          lists={lists}
          selectedId={headerId}
          search={navSearch}
          onSearch={setNavSearch}
          onSelect={id => { setHeaderId(id); setPicked({}); setDrafts([]); }}
          onNew={() => setEditHeader({ mode: 'new' })}
          busy={busy}
        />

        <div className="main">
          {header && (
            <MetaStrip
              header={header}
              conflictTotal={conflictTotal}
              onEdit={() => setEditHeader({ mode: 'edit' })}
              onSetCustomer={setCustomer}
            />
          )}

          {error && (
            <div className="errbox" role="alert">
              <span><b>Could not complete that</b>{error}</span>
              <button className="ghost" onClick={() => setError(null)}>Dismiss</button>
            </div>
          )}

          <div className="gridwrap">
            {!header
              ? <div className="empty">
                  <p className="big">No price list selected.</p>
                  <p>Pick one from the left, or create a new one.</p>
                  <button className="primary" onClick={() => setEditHeader({ mode: 'new' })}>
                    ＋ New price list
                  </button>
                </div>
              : loading
                ? <div className="loading">Loading price lines…</div>
                : <PriceGrid
                    grid={{ rows }} tiers={tiers} currencies={currencies} picked={picked}
                    onEditPrice={editPrice} onEditRow={editRow} onAddRow={addRow}
                    onPick={(row, currency, tier, anchor) =>
                      setPicker({ row, currency, tier, anchor })}
                  />}
          </div>

          <footer className="status">
            <span><b>{rows.length}</b> grid rows{drafts.length ? ` · ${drafts.length} unsaved` : ''}</span>
            <span><b>{header?.line_count ?? 0}</b> detail lines in this list</span>
            {conflictTotal > 0 && (
              <span style={{ color: 'var(--warn)' }}>
                <b style={{ color: 'var(--warn)' }}>{conflictTotal}</b> cells need a pick
              </span>
            )}
            <span className="spacer" />
            <span><kbd>↑↓←→</kbd> move · <kbd>Enter</kbd> edit · <kbd>Space</kbd> pick price · <kbd>Esc</kbd> cancel</span>
          </footer>
        </div>
      </div>

      {showCols && (
        <ColumnsMenu
          tiers={tiers} currencies={currencies} allTiers={allTiers}
          onApply={applyShape} onClose={() => setShowCols(false)}
        />
      )}

      {picker && (
        <ConflictPicker
          {...picker} picked={picked}
          onChoose={(cellKey, detailId) => {
            setPicked(p => ({ ...p, [cellKey]: detailId }));
            setPicker(null);
            say('Price selected for this line');
          }}
          onRetire={retireLine}
          onClose={() => setPicker(null)}
        />
      )}

      {editHeader && (
        <HeaderForm
          header={editHeader.mode === 'edit' ? header : null}
          onSaved={headerSaved}
          onDeleted={headerDeleted}
          onClose={() => setEditHeader(null)}
        />
      )}

      <div className={`flash${flash ? ' on' : ''}`}>{flash}</div>
    </>
  );
}

function MetaStrip({ header: h, conflictTotal, onEdit, onSetCustomer }) {
  const F = ({ k, v, dim }) => (
    <div className="mf">
      <span className="k">{k}</span>
      <span className={`v${dim ? ' dim' : ''}`}>{v}</span>
    </div>
  );
  return (
    <section className="meta">
      <F k="Header id" v={<span className="mono">{h.so_price_list_header_id}</span>} />
      {/* editable in place: mapping a customer is the one field most of the
          imported lists are still missing */}
      <div className="mf mfcust">
        <span className="k">Customer</span>
        <CustomerField
          key={h.so_price_list_header_id}
          compact
          customerId={h.customer_id}
          customerName={h.customer_name}
          onPick={c => onSetCustomer(c.customer_id)}
          onClear={() => onSetCustomer(null)}
        />
      </div>
      <F k="Workbook name" v={h.customer_excel || '—'} dim={!h.customer_excel} />
      <F k="Terms" v={h.terms || 'not stated'} dim={!h.terms} />
      <F k="Valid"
         v={h.valid_from || h.valid_to
             ? `${fmtDate(h.valid_from) || '—'} → ${fmtDate(h.valid_to) || 'open'}`
             : 'no dates — always current'}
         dim={!(h.valid_from || h.valid_to)} />
      <div className="mf">
        <span className="k">Detail lines</span>
        <span className="pillrow">
          <span className="pill open">{h.line_count} in DB</span>
          {conflictTotal > 0 && <span className="pill warnp">{conflictTotal} conflicts</span>}
        </span>
      </div>
      <div className="mf" style={{ marginLeft: 'auto', marginRight: 0, borderRight: 'none' }}>
        <span className="k">&nbsp;</span>
        <button className="ghost" onClick={onEdit} title="Edit this price list's header">
          Edit header…
        </button>
      </div>
    </section>
  );
}

const fmtDate = v => (v ? String(v).slice(0, 10) : '');
