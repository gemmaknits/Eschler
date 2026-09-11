import { useState, useEffect, useCallback, useRef } from 'react';
import {
  api, pivotToGrid, gridShape, orderTiers, bizKeyOf,
  getEmpCd, setEmpCd, isUserFromUrl, launch
} from './api';
import PriceGrid from './PriceGrid.jsx';
import ColumnsMenu from './ColumnsMenu.jsx';
import PriceListNav from './PriceListNav.jsx';
import HeaderForm from './HeaderForm.jsx';
import CustomerField from './CustomerField.jsx';
import DesignLov from './DesignLov.jsx';
import UomLov from './UomLov.jsx';

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
  // hidden by default; the header carries each list's own choice
  const [hideInactive, setHideInactive] = useState(true);
  const [showCols, setShowCols] = useState(false);
  const [navSearch, setNavSearch] = useState('');
  const [editHeader, setEditHeader] = useState(null);  // {mode:'new'} | {mode:'edit'}
  const [confirm, setConfirm]   = useState(null);      // {title, body, cta, onYes}
  const [flash, setFlash]       = useState('');
  /* The right-click clipboard: the row Copy was used on. It holds set_no, not
     a snapshot of the values - the copy happens on the server from whatever
     the row says at Insert time, so an edit in between is not lost. */
  const [copied, setCopied]     = useState(null);
  /* {row, filter} while the design picker is open. */
  const [designLov, setDesignLov] = useState(null);
  /* {row, typed} while the unit picker is open, and the units themselves -
     loaded once, so a typed unit can be checked without a round trip. */
  const [uomLov, setUomLov] = useState(null);
  const [uoms, setUoms] = useState([]);
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
    if (header) {
      setShape(gridShape(header));
      setHideInactive(header.hide_inactive !== 'N');
    }
  }, [header]);

  /* silent = refresh the data underneath without flipping to the loading
     state. The loading message replaces the whole table, which loses scroll
     position and focus - fine when switching lists, jarring after an edit. */
  const reload = useCallback((silent = false) => {
    if (headerId == null) return;
    if (!silent) setLoading(true);
    setError(null);
    api.listDetail(headerId, {
        search: filter || undefined,
        conflicts_only: conflictsOnly ? 1 : undefined
      })
      .then(rows => { setGrid(pivotToGrid(rows)); setDrafts([]); if (!silent) setLoading(false); })
      .catch(err => { setError(err.message); if (!silent) setLoading(false); });
  }, [headerId, filter, conflictsOnly]);

  /* Patch the loaded rows in place. A field edit changes values we already
     know, so there is nothing to fetch - and not fetching keeps the row where
     it is under the cursor. */
  const patchRowLocally = useCallback((rowKey, patch, cellPatch) => {
    setGrid(g => ({
      ...g,
      rows: g.rows.map(r => {
        if (r.key !== rowKey) return r;
        const next = { ...r, ...patch };
        if (cellPatch) {
          next.cells = Object.fromEntries(
            Object.entries(r.cells).map(([k, d]) => [k, { ...d, ...cellPatch }]));
        }
        return next;
      })
    }));
  }, []);

  useEffect(() => {
    const t = setTimeout(reload, filter ? 250 : 0);
    return () => clearTimeout(t);
  }, [reload, filter]);

  const refreshLists = () => api.listPriceLists().then(setLists).catch(() => {});

  /* The units of measure, once. The grid checks a typed unit against these
     before saving, so an invalid one opens the picker instead of coming back
     as an error from the database. */
  useEffect(() => {
    api.lovUom().then(rows => setUoms(rows.map(u => u.uom))).catch(() => {});
  }, []);


  /* A tier or currency with prices but no column would hide money, so union
     what the header declares with what the data actually holds. */
  const tiers = orderTiers([...shape.tiers, ...(grid.tiersInData || [])]);
  const currencies = ['USD', 'THB'].filter(c =>
    (shape.currencies.length ? shape.currencies : ['USD']).includes(c) ||
    (grid.currenciesInData || []).includes(c));
  const visibleRows = hideInactive
    ? grid.rows.filter(r => r.active !== 'N')
    : grid.rows;
  /* A draft opened with "Insert here" shows where it was asked for, directly
     below that row - not at the bottom of the list. Seeing it appear somewhere
     else is how you lose track of what you were inserting. Drafts from
     "+ Add line" have no position and still go at the end. */
  const rows = (() => {
    const placed = drafts.filter(d => d.after_set_no != null);
    const appended = drafts.filter(d => d.after_set_no == null);
    if (!placed.length) return [...visibleRows, ...appended];

    const out = [];
    for (const r of visibleRows) {
      out.push(r);
      for (const d of placed) if (d.after_set_no === r.set_no) out.push(d);
    }
    /* A draft whose anchor is hidden (a withdrawn row, with Hidden on) would
       vanish silently, so it falls to the end rather than being dropped. */
    for (const d of placed) if (!out.includes(d)) out.push(d);
    return [...out, ...appended];
  })();
  const inactiveCount = grid.rows.filter(r => r.active === 'N').length;

  /* ---- right-click: copy / insert / delete a whole line ------------------
     A grid row is a whole set - every tier, both currencies - so all three act
     on the set, never on the single cell that was clicked. */
  const copyRow = useCallback(row => {
    setCopied({ set_no: row.set_no, design_no: row.design_no });
    say(`Copied ${row.design_no} — right-click another line to insert it`);
  }, [say]);

  const insertCopied = useCallback(async target => {
    /* Nothing copied: open a blank line in that position instead. It is a
       draft, so it costs nothing until a design and a price are typed - but it
       remembers where it was asked for, and saves into that slot rather than
       at the end of the list. */
    if (!copied) {
      setDrafts(d => [...d, {
        key: `draft-${Date.now()}-${d.length}`,
        isDraft: true,
        after_set_no: target.set_no,
        design_no: '',
        article_variant: '',
        qty_min: 0,
        qty_max: null,
        qty_unit: target.qty_unit || 'MTS',
        fabric_name: '', composition: '',
        full_width_cm: '', usable_width_cm: '', weight_gsm: '', moq: '',
        source_row: null,
        cells: {}
      }]);
      say(`New line below ${target.design_no} — fill the design no and a price`);
      return;
    }

    setBusy(true);
    try {
      /* after_set_no is the row that was right-clicked, so the copy lands
         directly below it. The procedure shifts everything under that point
         down, and carries line_no over unchanged so the tiers inside the
         copied row keep the order they were put in. */
      const res = await api.copySet(headerId, {
        source_set_no: copied.set_no,
        after_set_no: target.set_no
      });
      say(`Inserted ${copied.design_no} below ${target.design_no} — ${res?.rows_created ?? 0} prices`);
      reload(true); refreshLists();
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [copied, headerId, reload, say]);

  const deleteRow = useCallback(row => {
    setConfirm({
      title: `Delete ${row.design_no}?`,
      body: `${row.qty_min}–${row.qty_max === null ? '∞' : row.qty_max} ${row.qty_unit}. `
          + 'Every price on this line goes with it. This is a soft delete — the '
          + 'rows stay in the table and can be brought back.',
      cta: 'Delete line',
      onYes: async () => {
        setBusy(true);
        try {
          const res = await api.deleteSet(headerId, { set_no: row.set_no });
          if (copied?.set_no === row.set_no) setCopied(null);
          say(`${row.design_no} deleted — ${res?.lines_deleted ?? 0} prices`);
          reload(true); refreshLists();
        } catch (err) { setError(err.message); } finally { setBusy(false); }
      }
    });
  }, [headerId, copied, reload, say]);

  /* ---- add a line ---------------------------------------------------------
     A draft lives only in the browser until a price is typed. A detail row
     cannot exist without a currency, tier and price, so there is nothing to
     insert until then - and an abandoned draft leaves no empty row behind. */
  const addRow = useCallback(() => {
    const last = rows[rows.length - 1];
    setDrafts(d => [...d, {
      key: `draft-${Date.now()}-${d.length}`,
      isDraft: true,
      design_no: last?.design_no || '',
      article_variant: '',
      qty_min: last?.qty_max != null ? Number(last.qty_max) + 1 : 0,
      qty_max: null,
      qty_unit: last?.qty_unit || 'MTS',
      fabric_name: last?.fabric_name || '',
      composition: last?.composition || '',
      full_width_cm: last?.full_width_cm || '',
      usable_width_cm: last?.usable_width_cm || '',
      weight_gsm: last?.weight_gsm || '',
      moq: last?.moq || '',
      source_row: null,
      cells: {}
    }]);
    say('New line — fill the design no and a price, then it saves');
  }, [rows, say]);

  const patchDraft = (key, patch) =>
    setDrafts(d => d.map(r => (r.key === key ? { ...r, ...patch } : r)));

  /* ---- edit a row-level field ---- */
  /* Typing a design number fills in what the company already knows about it:
     the fabric name from dm.refdesno, the spec from designs. Only EMPTY fields
     are filled - a value already on the line is the price list's own, possibly
     corrected, and is never overwritten by the master.

     An unknown design is not an error. 31 of the 261 designs in use are not in
     the ERP masters at all, so the fields are simply left to be typed. */
  const fillFromDesign = useCallback(async (row, designNo) => {
    const key = (designNo || '').trim();
    if (!key) return;
    try {
      const d = await api.lookupDesign(key);

      /* No exact match. A price list often carries the STEM of a number the ERP
         holds in full - 255699AA against 255699AA/14, /15, /16 - so offer those
         rather than declaring the design unknown. The picker decides; if it
         finds nothing either, it says so and the line is typed by hand. */
      if (!d?.found) { setDesignLov({ row, filter: key }); return; }

      const patch = {};
      const take = (field, incoming) => {
        const current = (row[field] ?? '').toString().trim();
        if (!current && incoming) patch[field] = incoming;
      };
      take('fabric_name',     d.fabric_name);
      take('composition',     d.composition);
      take('weight_gsm',      d.weight_gsm);
      take('usable_width_cm', d.usable_width_cm);

      if (!Object.keys(patch).length) { say(`${key} — ${d.fabric_name || 'found'}`); return; }

      if (row.isDraft) { patchDraft(row.key, patch); }
      else {
        for (const line of Object.values(row.cells || {})) {
          await api.saveDetail({ detail_id: line.so_price_list_detail_id, ...patch });
        }
        patchRowLocally(row.key, patch, patch);
      }
      say(`${key} — ${d.fabric_name || 'spec'} filled in`);
    } catch (err) {
      /* A lookup failure must not lose the design number the user just typed. */
      setError(`Design lookup failed: ${err.message}`);
    }
  }, [say, patchRowLocally]);

  /* A design picked from the list of values. It replaces what was typed - the
     stem 255699AA becomes the real 255699AA/14 - and brings the spec with it,
     so the line ends up carrying a number the ERP actually knows. */
  const pickDesign = useCallback(async picked => {
    const target = designLov?.row;
    setDesignLov(null);
    if (!target || !picked?.design_no) return;

    const patch = {
      design_no: picked.design_no,
      ...(picked.fabric_name     && !((target.fabric_name     ?? '').trim()) ? { fabric_name:     picked.fabric_name } : {}),
      ...(picked.composition     && !((target.composition     ?? '').trim()) ? { composition:     picked.composition } : {}),
      ...(picked.weight_gsm      && !((target.weight_gsm      ?? '').trim()) ? { weight_gsm:      picked.weight_gsm } : {}),
      ...(picked.usable_width_cm && !((target.usable_width_cm ?? '').trim()) ? { usable_width_cm: picked.usable_width_cm } : {})
    };

    if (target.isDraft) { patchDraft(target.key, patch); say(`${picked.design_no} selected`); return; }

    setBusy(true);
    try {
      for (const line of Object.values(target.cells || {})) {
        await api.saveDetail({ detail_id: line.so_price_list_detail_id, ...patch });
      }
      patchRowLocally(target.key, patch, patch);
      say(`${picked.design_no} — ${picked.fabric_name || 'selected'}`);
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [designLov, patchRowLocally, say]);

  /* A unit chosen from the picker, applied to the row it was opened for. */
  const pickUom = useCallback(async uom => {
    const target = uomLov?.row;
    setUomLov(null);
    if (!target || !uom) return;

    if (target.isDraft) { patchDraft(target.key, { qty_unit: uom }); say(`Unit ${uom}`); return; }
    setBusy(true);
    try {
      for (const line of Object.values(target.cells || {})) {
        await api.saveDetail({ detail_id: line.so_price_list_detail_id, qty_unit: uom });
      }
      patchRowLocally(target.key, { qty_unit: uom }, { qty_unit: uom });
      say(`Unit set to ${uom}`);
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [uomLov, patchRowLocally, say]);

  const editRow = useCallback(async (row, col, raw) => {
    /* The unit has to be one the system knows. The database refuses anything
       else, so rather than let the save fail and report it, check here and
       open the picker - the typed value is shown so it is clear what was
       rejected. Case is not the user's problem: "mts" becomes "MTS".

       Checked BEFORE the value is read, so the corrected case is what gets
       saved rather than what was typed. */
    if (col.key === 'qty_unit' && uoms.length) {
      const typed = (raw || '').trim();
      const match = uoms.find(u => u.toUpperCase() === typed.toUpperCase());
      if (!match) { setUomLov({ row, typed }); return; }
      raw = match;
    }

    const value = col.num ? (raw === '' ? null : parseInt(raw, 10)) : raw;
    if (col.num && raw !== '' && Number.isNaN(value)) return;

    if (row.isDraft) {
      patchDraft(row.key, { [col.key]: value });
      if (col.key === 'design_no' && value) {
        fillFromDesign({ ...row, [col.key]: value }, value);
      }
      return;
    }

    /* The active flag is set on THIS row's own price lines, by id.
       It cannot go through update_price_list_row: that proc matches on
       (design, variant, qty band) without tier or currency, so on a list where
       several rows share a quantity band - which is most of them - one click
       would silently withdraw all of its siblings. */
    if (col.flag) {
      const lines = Object.values(row.cells || {});
      if (!lines.length) return;

      const apply = async () => {
        setBusy(true);
        try {
          for (const d of lines) {
            await api.saveDetail({ detail_id: d.so_price_list_detail_id, active: value });
          }
          patchRowLocally(row.key, { active: value }, { active: value });
          say(value === 'N'
            ? `${row.design_no} withdrawn — order entry will not offer it`
            : `${row.design_no} set active`);
        } catch (err) { setError(err.message); }
        finally { setBusy(false); }
      };

      // Withdrawing removes a price from order entry, so ask. Re-enabling
      // restores it and is harmless, so it just happens.
      if (value === 'N') {
        setConfirm({
          title: 'Withdraw this price line?',
          body: `${row.design_no} · ${row.qty_min}–${row.qty_max === null ? '∞' : row.qty_max} ${row.qty_unit}`,
          detail: lines.length === 1
            ? 'Order entry will stop offering this price.'
            : `Order entry will stop offering all ${lines.length} prices on this row.`,
          cta: 'Withdraw',
          onYes: apply
        });
        return;
      }
      await apply();
      return;
    }

    /* Every row-level edit is applied to THIS row's own price lines, by id.
       update_price_list_row matches on (design, variant, qty band) without
       tier or currency, so on a list where several rows share a quantity band
       - most of them - editing one row would quietly rewrite its siblings.
       The proc is still there for a deliberate bulk change; the grid does not
       use it. */
    const lines = Object.values(row.cells || {});
    if (!lines.length) return;

    setBusy(true);
    try {
      for (const d of lines) {
        await api.saveDetail({
          detail_id: d.so_price_list_detail_id,
          [col.key]: value,
          // an open upper bound is a real value, not "leave alone"
          ...(col.key === 'qty_max' && value === null ? { clear_qty_max: true } : {})
        });
      }
      patchRowLocally(row.key, { [col.key]: value }, { [col.key]: value });
      say(lines.length === 1
        ? `${col.label} updated`
        : `${col.label} updated on ${lines.length} prices in this row`);

      /* A design number changed, so the spec on the line may now belong to a
         different fabric. Fill anything blank from the master - what is
         already there stays, since it may be a correction being made now. */
      if (col.key === 'design_no' && value) {
        await fillFromDesign({ ...row, design_no: value }, value);
      }
    } catch (err) {
      setError(err.message);
    } finally { setBusy(false); }
  }, [headerId, reload, say, fillFromDesign, uoms]);

  /* ---- edit or create a price ---- */
  const editPrice = useCallback(async (row, currency, tier, value) => {
    const w = row.cells?.[`${currency}|${tier}`] || null;

    if (!row.isDraft && !w) {
      // no line under this cell yet - create one on this row's key
      if (!row.design_no) { setError('Give the line a design no first.'); return; }
      setBusy(true);
      try {
        /* The row is identified by its design no - that is what names a line
           in the workbook, so it is what the save sends.

           set_no rides along only as a tiebreaker: two rows CAN share a design
           and quantity band and still hold different prices (the grid brackets
           them together), and the design alone cannot say which of those the
           user clicked.

           after_line_no keeps the new tier where it was typed. Without it the
           line is appended to the end of the row's lines instead of sitting
           next to the tier above it. */
        const lineNos = Object.values(row.cells || {})
          .map(d => d.line_no).filter(n => n != null);

        await api.saveDetail({
          header_id: headerId, set_no: row.set_no,
          design_no: row.design_no,
          after_line_no: lineNos.length ? Math.max(...lineNos) : null,
          article_variant: row.article_variant || null,
          qty_min: row.qty_min, qty_max: row.qty_max, qty_unit: row.qty_unit,
          color_tier: tier, currency, price: value,
          fabric_name: row.fabric_name, composition: row.composition,
          full_width_cm: row.full_width_cm, usable_width_cm: row.usable_width_cm,
          weight_gsm: row.weight_gsm, moq: row.moq
        });
        say(`Added ${row.design_no} · ${tier} · ${currency}`);
        reload(true); refreshLists();
      } catch (err) { setError(err.message); } finally { setBusy(false); }
      return;
    }

    if (row.isDraft) {
      if (!row.design_no) { setError('Give the line a design no first.'); return; }
      setBusy(true);
      try {
        const body = {
          header_id: headerId, design_no: row.design_no,
          article_variant: row.article_variant || null,
          qty_min: row.qty_min ?? 0, qty_max: row.qty_max, qty_unit: row.qty_unit || 'MTS',
          color_tier: tier, currency, price: value,
          fabric_name: row.fabric_name, composition: row.composition,
          full_width_cm: row.full_width_cm, usable_width_cm: row.usable_width_cm,
          weight_gsm: row.weight_gsm, moq: row.moq
        };

        /* A draft opened with "Insert here" knows where it belongs, and goes
           through the set insert so the rows below shift down to make room.
           A draft from "+ Add line" has no position and just appends. */
        if (row.after_set_no != null) {
          await api.insertSet(headerId, { ...body, after_set_no: row.after_set_no });
        } else {
          await api.saveDetail(body);
        }
        say(`Line saved — ${row.design_no} · ${tier} · ${currency}`);
        reload(true); refreshLists();
      } catch (err) { setError(err.message); } finally { setBusy(false); }
      return;
    }

    setBusy(true);
    try {
      await api.saveDetail({ detail_id: w.so_price_list_detail_id, price: value });
      /* One detail per cell since alternatives were split into their own rows.
         This used to treat cells as arrays and threw on every price edit. */
      setGrid(g => ({
        ...g,
        rows: g.rows.map(r => r.key !== row.key ? r : {
          ...r,
          cells: Object.fromEntries(Object.entries(r.cells).map(([k, d]) => [
            k, d.so_price_list_detail_id === w.so_price_list_detail_id
                 ? { ...d, price: value } : d
          ]))
        })
      }));
      say(`${row.design_no} · ${tier} · ${currency} → ${value}`);
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [headerId, reload, say]);


  /* A per-list view preference, stored on the header so the next person to open
     this list sees it the way it was left. */
  const setHideInactivePref = useCallback(async (on) => {
    setHideInactive(on);
    if (!headerId) return;
    try {
      await api.saveGridShape(headerId, { hide_inactive: on ? 'Y' : 'N' });
      setLists(ls => ls.map(l => l.so_price_list_header_id === headerId
        ? { ...l, hide_inactive: on ? 'Y' : 'N' } : l));
    } catch (err) { setError(err.message); }
  }, [headerId]);

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

  /* Delete a whole price list from the nav. It was only reachable from inside
     Edit header before, which is not where anyone looks for it.

     Soft, like every delete here: the header and every one of its lines go to
     delete_mark 'Y' in one transaction, and nothing reads them afterwards. */
  const deleteList = useCallback(list => {
    const lines = list.line_count || 0;
    setConfirm({
      title: `Delete "${list.list_name}"?`,
      body: lines > 0
        ? `${lines} price line${lines === 1 ? '' : 's'} go with it.`
        : 'This list has no price lines.',
      detail: 'A soft delete — the rows stay in the table and can be brought back.',
      cta: 'Delete price list',
      onYes: async () => {
        setBusy(true);
        try {
          const res = await api.deleteList(list.so_price_list_header_id);
          await headerDeleted(list.so_price_list_header_id, res?.deleted_line_count ?? lines);
        } catch (err) { setError(err.message); } finally { setBusy(false); }
      }
    });
  }, [headerDeleted]);


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

  /* rows that are one of several alternatives for the same design and band */
  const multiPriceRows = grid.rows.filter(r => r.groupSize > 1).length;
  const conflictTotal = header?.conflict_count ?? 0;

  return (
    <>
      <header className="topbar">
        <div className="brand">
          <h1>Eschler Price Book</h1>
          <span className="tag">header · detail</span>
        </div>

        <input type="search" placeholder="Filter design…"
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
          onSelect={id => { setHeaderId(id); setDrafts([]); }}
          onNew={() => setEditHeader({ mode: 'new' })}
          onDeleteList={deleteList}
          busy={busy}
        />

        <div className="main">
          {header && (
            <MetaStrip
              header={header}
              conflictTotal={conflictTotal}
              onEdit={() => setEditHeader({ mode: 'edit' })}
              onSetCustomer={setCustomer}
              hideInactive={hideInactive}
              inactiveCount={inactiveCount}
              onHideInactive={setHideInactivePref}
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
                    grid={{ rows }} tiers={tiers} currencies={currencies}
                    onEditPrice={editPrice} onEditRow={editRow} onAddRow={addRow}
                    onInvalid={say}
                    copied={copied}
                    onCopyRow={copyRow}
                    onInsertCopied={insertCopied}
                    onDeleteRow={deleteRow}
                  />}
          </div>

          <footer className="status">
            <span><b>{rows.length}</b> grid rows{drafts.length ? ` · ${drafts.length} unsaved` : ''}</span>
            {hideInactive && inactiveCount > 0 && (
              <span><b>{inactiveCount}</b> inactive hidden</span>
            )}
            <span><b>{header?.line_count ?? 0}</b> detail lines in this list</span>
            {multiPriceRows > 0 && (
              <span><b>{multiPriceRows}</b> rows are alternatives</span>
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


      {editHeader && (
        <HeaderForm
          header={editHeader.mode === 'edit' ? header : null}
          onSaved={headerSaved}
          onDeleted={headerDeleted}
          onClose={() => setEditHeader(null)}
        />
      )}

      {uomLov && (
        <UomLov
          invalid={uomLov.typed}
          onPick={pickUom}
          onClose={() => setUomLov(null)}
        />
      )}

      {designLov && (
        <DesignLov
          initialFilter={designLov.filter}
          onPick={pickDesign}
          /* Closing without picking leaves the typed number exactly as it was.
             It may be a design the ERP has never carried, and this app is not
             the place to refuse it. */
          onClose={() => setDesignLov(null)}
        />
      )}

      {confirm && (
        <div className="modalwrap" onMouseDown={e => { if (e.target === e.currentTarget) setConfirm(null); }}>
          <div className="modal confirm" role="alertdialog" aria-label={confirm.title}
               onKeyDown={e => { if (e.key === 'Escape') setConfirm(null); }}>
            <h3>{confirm.title}</h3>
            <p className="confirmbody mono">{confirm.body}</p>
            <p className="modalsub">{confirm.detail}</p>
            <div className="modalactions">
              <button className="danger" autoFocus
                      onClick={() => { const f = confirm.onYes; setConfirm(null); f(); }}>
                {confirm.cta}
              </button>
              <button className="ghost" onClick={() => setConfirm(null)}>Cancel</button>
            </div>
          </div>
        </div>
      )}

      <div className={`flash${flash ? ' on' : ''}`}>{flash}</div>
    </>
  );
}

function MetaStrip({ header: h, conflictTotal, onEdit, onSetCustomer,
                     hideInactive, inactiveCount, onHideInactive }) {
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
      <div className="mf">
        <span className="k">Withdrawn lines</span>
        <label className="chk metachk"
               title="Remembered for this price list. Withdrawn lines are not offered by order entry either way.">
          <input type="checkbox" checked={hideInactive}
                 onChange={e => onHideInactive(e.target.checked)} />
          <span>Hidden</span>
          {inactiveCount > 0 && <span className="metacount">{inactiveCount}</span>}
        </label>
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
