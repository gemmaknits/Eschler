import { useState, useRef, useEffect, useCallback, useMemo } from 'react';

/* Two decimals, both currencies, always - so a column of prices reads as a
   column of prices rather than 1.8 above 2.15 above 216.

   The column is decimal(18,4) and 14 lines in the book do use the third and
   fourth place (USD 6.152, THB 54.922). Those are SHOWN rounded and STORED
   whole: the editor is seeded from the stored number, not from this, so
   opening such a cell shows 6.152 and pressing Enter changes nothing. Nothing
   here can round anything away. */
const money = (v, ccy) => {
  if (v === null || v === undefined || v === '') return '';
  const n = Number(v);
  return Number.isFinite(n) ? n.toFixed(2) : '';
};

/* True when the stored price carries more than the two places shown, so the
   cell can say so on hover instead of quietly hiding it. */
const isRounded = v => {
  const n = Number(v);
  return Number.isFinite(n) && Number(n.toFixed(2)) !== n;
};

/* Row-level columns, editable. `num` writes an integer, `blank` means the empty
   string clears the value (qty_max empty = open upper bound). */
/* Variant is hidden, not dropped: it is still part of the business key that
   identifies a row, so it keeps travelling with every read and row update -
   it just has no column, because in the imported data it is empty on all
   4,468 lines. */
export const ROW_COLS = [
  { key: 'active',   label: 'Act',     width: 52, flag: true },
  { key: 'design_no', label: 'Design No', sticky: true, width: 118 },
  /* When this price was quoted. Next to the design number because that is
     where it is read - "this design, at this price, on this date" - rather
     than at the far end of the spec columns where it started. */
  { key: 'price_line_date', label: 'Price date', width: 132, date: true },
  { key: 'qty_min',  label: 'Min',     group: 'Quantity', num: true,  width: 72 },
  { key: 'qty_max',  label: 'Max',     group: 'Quantity', num: true,  width: 72, blank: true },
  { key: 'qty_unit', label: 'Unit',    group: 'Quantity', width: 56 }
];

export const INFO_COLS = [
  { key: 'fabric_name',     label: 'Fabric',     width: 210 },
  { key: 'composition',     label: 'Composition', width: 210 },
  { key: 'full_width_cm',   label: 'Full W',     width: 124 },
  { key: 'usable_width_cm', label: 'Usable W',   width: 124 },
  { key: 'weight_gsm',      label: 'g/m²',       width: 112 },
  { key: 'moq',             label: 'MOQ',        width: 104 },
  /* Only 3% of lines have a note, but they carry the quote's provenance -
     who quoted it, when, on what terms - so they are worth the widest column
     here. They run to 2,000 characters, so no width fits them all: what does
     not fit stays reachable by scrolling the cell sideways, and the whole
     text is on hover and in the editor. */
  { key: 'notes',           label: 'Notes',      width: 460, wide: true }
];

/* Every column has a fixed width, and the table is laid out fixed, so the grid
   has identical geometry on every price list. Sized to content it would shift
   column to column and list to list, and a long fabric name would squeeze the
   numbers - the thing you actually read. */
const GUTTER_W = 42;
const PRICE_W  = 96;

/* Widths the user drags are kept here, per column, and remembered across
   sessions. Bounds exist so a column cannot be dragged to nothing (invisible,
   and impossible to grab again) or to a width that pushes the rest off screen. */
/* Height of a wide cell's own scrollbar - the strip at the bottom where a
   click means "scroll", not "edit". */
const SCROLL_H = 7;
const MIN_W = 44;
const MAX_W = 900;
const WIDTH_KEY = 'priceGrid.colWidths.v1';

function loadWidths() {
  try {
    const raw = localStorage.getItem(WIDTH_KEY);
    if (!raw) return {};
    const o = JSON.parse(raw);
    if (!o || typeof o !== 'object') return {};
    /* A hand-edited or stale entry must not be able to break the grid. */
    const clean = {};
    for (const [k, v] of Object.entries(o))
      if (Number.isFinite(v) && v >= MIN_W && v <= MAX_W) clean[k] = Math.round(v);
    return clean;
  } catch { return {}; }
}

/* One detail per cell. Alternatives for the same article and quantity band are
   separate rows, bracketed together by a rail in the row-number gutter - so
   there is nothing hidden behind a badge and nothing to click to reveal. */
const cellAt = (row, ccy, tier) => row.cells?.[`${ccy}|${tier}`] || null;

/* PageUp/PageDown move by what is actually on screen, not a fixed guess, so
   the row under the cursor stays under the cursor. 29px is the row height set
   in the stylesheet; the fallback is a sensible screenful. */
/* ---------- column filters ----------

   A filter is a substring by default, because that is what people type. On a
   column of numbers an operator makes it a comparison instead, which is the
   thing you actually want of a price column: >5, <=2.5, =0.

   "=" on its own means blank, the one case that cannot be written as text -
   and finding the gaps is most of what this review is for. */
const OPS = [['>=', (a, b) => a >= b], ['<=', (a, b) => a <= b],
             ['<>', (a, b) => a !== b], ['>',  (a, b) => a >  b],
             ['<',  (a, b) => a <  b],  ['=',  (a, b) => a === b]];

function matches(text, q) {
  const needle = q.trim();
  if (!needle) return true;
  const have = (text ?? '').toString().trim();

  if (needle === '=') return have === '';

  for (const [sym, test] of OPS) {
    if (!needle.startsWith(sym)) continue;
    const n = Number(needle.slice(sym.length).trim());
    const v = Number(have);
    if (!Number.isFinite(n)) break;          // ">abc" - fall through to text
    if (have === '' || !Number.isFinite(v)) return false;
    return test(v, n);
  }
  return have.toLowerCase().includes(needle.toLowerCase());
}

const ROW_H = 29;
function pageRows() {
  const pane = document.querySelector('.gridwrap');
  const h = pane?.clientHeight;
  return h ? Math.max(1, Math.floor(h / ROW_H) - 1) : 20;
}

/* What is on the clipboard, in the few words the menu has room for.

   The design number alone is not enough to tell two copied rows apart - a
   design usually appears several times over, once per quantity band, and the
   thing that distinguishes them is the colour tier and what it costs. */
function describe(c) {
  if (!c) return null;
  if (c.count > 1) return `${c.count} lines · ${c.designs.slice(0, 3).join(', ')}` +
                          (c.designs.length > 3 ? ' …' : '');
  const band = c.qty_min !== undefined
    ? ` · ${c.qty_min}–${c.qty_max === null || c.qty_max === '' ? '∞' : c.qty_max}` : '';
  const priced = (c.tiers || []).filter(t => t.price !== null && t.price !== undefined);
  const shown = (priced.length ? priced : (c.tiers || [])).slice(0, 3);
  const tiers = shown.map(t =>
    t.price === null || t.price === undefined
      ? t.tier
      : `${t.tier} ${t.currency === 'THB' ? '฿' : '$'}${t.price}`).join(' · ');
  return `${c.design_no}${band}${tiers ? ' · ' + tiers : ''}`;
}

/* Whole string or nothing. parseFloat('3.5xyz') is 3.5 and parseInt('12abc')
   is 12, so a typo would be silently truncated and saved. */
const IS_QTY   = /^\d{1,9}$/;                  // non-negative whole number
const IS_PRICE = /^\d{1,12}(\.\d{1,4})?$/;     // decimal(18,4), non-negative
/* ISO only, and the day has to be real: 2025-02-31 parses to 3 March in JS,
   which would silently save a date nobody typed. */
const IS_DATE  = /^\d{4}-\d{2}-\d{2}$/;
const dateOk = v => {
  if (!IS_DATE.test(v)) return false;
  const [y, m, d] = v.split('-').map(Number);
  if (m < 1 || m > 12 || d < 1) return false;
  return d <= new Date(y, m, 0).getDate();
};

export default function PriceGrid({
  grid, tiers, currencies,
  onEditPrice, onEditRow, onAddRow, onInvalid,
  onCopyRow, onInsertCopied, onInsertBlank, onDeleteRow, copied
}) {
  const [focus, setFocus] = useState(null);      // {r, c} - c indexes ALL columns
  const [editing, setEditing] = useState(null);  // {r, c, value, mode}
  const tableRef = useRef(null);
  const lastCommit = useRef(null);
  const [invalid, setInvalid] = useState(false);
  /* colId -> pixels, for the columns the user has dragged. Absent means the
     column's own default from the tables above. */
  const [sized, setSized] = useState(loadWidths);
  /* {x, y, row} - where the menu sits and which row it acts on. Held here
     rather than per row so only one can ever be open. */
  const [menu, setMenu] = useState(null);

  /* Row selection, by row key. Click a row number to select it, shift-click
     another to take the block between them, ctrl-click to add or drop one -
     the same three gestures as a spreadsheet's row headings. What it is FOR is
     copying several rows in one go; everything else still works a row at a
     time. */
  const [selected, setSelected] = useState(() => new Set());
  /* colId -> what was typed under that column. */
  const [colFilter, setColFilter] = useState({});
  const [showFilters, setShowFilters] = useState(false);
  /* Where a shift-click measures from. */
  const anchor = useRef(null);

  /* Any click elsewhere, Escape, or a scroll closes it. Without the scroll
     listener the menu hangs in place while the rows move underneath it. */
  useEffect(() => {
    if (!menu) return;
    const close = () => setMenu(null);
    const onKey = e => { if (e.key === 'Escape') close(); };
    window.addEventListener('click', close);
    window.addEventListener('resize', close);
    window.addEventListener('scroll', close, true);
    window.addEventListener('keydown', onKey);
    return () => {
      window.removeEventListener('click', close);
      window.removeEventListener('resize', close);
      window.removeEventListener('scroll', close, true);
      window.removeEventListener('keydown', onKey);
    };
  }, [menu]);

  /* One flat column model, so arrow keys cross field columns and price columns
     alike instead of stopping at the boundary. */
  const columns = [
    ...ROW_COLS.map(c => ({ kind: 'row', ...c })),
    ...currencies.flatMap(ccy =>
      tiers.map(tier => ({ kind: 'price', currency: ccy, tier, label: tier }))),
    ...INFO_COLS.map(c => ({ kind: 'row', ...c }))
  ];
  const maxC = columns.length - 1;

  /* One stable id per column, so a remembered width survives a reload and a
     change of price list. Price columns are keyed by currency and tier rather
     than by position - the columns shift when a filter narrows them. */
  const colId = c => c.kind === 'price' ? `p:${c.currency}:${c.tier}` : `f:${c.key}`;

  /* Filtered at source, deliberately: every index in this component - the
     focused cell, the shift-anchor, what a right-click acts on - counts rows
     as they appear on screen. Filtering anywhere later would leave the
     keyboard walking over rows nobody can see. */
  const allRows = grid.rows;
  const activeFilters = Object.entries(colFilter).filter(([, v]) => v.trim());

  const rows = useMemo(() => {
    if (!activeFilters.length) return allRows;
    return allRows.filter(row =>
      /* A draft is a line being typed. It has nothing in it yet, so every
         filter would hide it the moment it was created. */
      row.isDraft ||
      activeFilters.every(([id, q]) => {
        const col = columns.find(c => colId(c) === id);
        if (!col) return true;
        if (col.kind === 'price') {
          const d = cellAt(row, col.currency, col.tier);
          return matches(d ? d.price : '', q);
        }
        if (col.key === 'qty_max' && (row.qty_max === null || row.qty_max === ''))
          return matches('', q);
        return matches(row[col.key], q);
      }));
  }, [allRows, colFilter, columns]);

  const hidden = allRows.length - rows.length;

  const setFilter = (id, v) => setColFilter(p => ({ ...p, [id]: v }));
  const clearFilters = () => { setColFilter({}); setShowFilters(false); };

  /* Opening a different price list drops the selection. Deliberately NOT keyed
     on the grid object: it is rebuilt on every render of the parent, so that
     cleared the selection every time anything saved - including the save you
     were selecting rows in order to make. The first row's key and the row
     count together change when the list changes and hold still when it does
     not. */
  const listSig = `${grid.rows[0]?.key ?? ''}|${grid.rows.length}`;
  useEffect(() => { setSelected(new Set()); anchor.current = null; }, [listSig]);

  /* Rows can disappear under a selection (a delete, a filter). Everything that
     acts on the selection reads it through here, so it can only ever contain
     rows that are actually on screen. */
  const selectedRows = rows.filter(r => selected.has(r.key));

  /* Click a row number. Plain click replaces the selection, shift extends from
     the anchor, ctrl/cmd toggles one. */
  const pickRow = useCallback((ri, e) => {
    const row = rows[ri];
    if (!row) return;
    setSelected(prev => {
      const next = new Set(prev);
      if (e.shiftKey && anchor.current !== null) {
        const [a, b] = [anchor.current, ri].sort((x, y) => x - y);
        next.clear();
        for (let i = a; i <= b; i++) if (rows[i]) next.add(rows[i].key);
        return next;
      }
      if (e.ctrlKey || e.metaKey) {
        next.has(row.key) ? next.delete(row.key) : next.add(row.key);
        anchor.current = ri;
        return next;
      }
      /* A plain click on the only selected row clears it, so there is a way
         out that does not need the keyboard. */
      const only = prev.size === 1 && prev.has(row.key);
      next.clear();
      if (!only) next.add(row.key);
      anchor.current = only ? null : ri;
      return next;
    });
  }, [rows]);

  /* Written on every drop rather than on every pixel - setSized already runs
     per mousemove, and localStorage is synchronous. */
  useEffect(() => {
    try { localStorage.setItem(WIDTH_KEY, JSON.stringify(sized)); } catch { /* private mode */ }
  }, [sized]);

  /* ------------------------------ column widths ------------------------------
     The defaults above are the starting point, not the law: a reviewer reading
     long notes or Thai fabric names needs to widen a column, and the width they
     choose should still be there tomorrow. */
  const colWidth = c => sized[colId(c)]
                     ?? (c.kind === 'price' ? PRICE_W : (c.width || 120));
  /* table-layout:fixed only honours <col> widths when the table has an explicit
     width; left to width:auto it stretches the columns to fill the pane, which
     is exactly the data-dependent sizing we are trying to avoid. */
  const tableWidth = GUTTER_W + columns.reduce((a, c) => a + colWidth(c), 0);

  /* Drag state lives in a ref, not in state: the mousemove handler fires on
     every pixel and re-rendering the whole grid to remember a cursor position
     would make the drag stutter on a 600-line list. */
  const drag = useRef(null);

  const startResize = (e, col) => {
    e.preventDefault();
    e.stopPropagation();     // the header must not take focus or sort
    const id = colId(col);
    const from = colWidth(col);
    drag.current = { id, x: e.clientX, from };
    document.body.classList.add('colresizing');

    const onMove = ev => {
      const d = drag.current;
      if (!d) return;
      const w = Math.round(Math.min(Math.max(d.from + (ev.clientX - d.x), MIN_W), MAX_W));
      setSized(prev => (prev[d.id] === w ? prev : { ...prev, [d.id]: w }));
    };
    const onUp = () => {
      drag.current = null;
      document.body.classList.remove('colresizing');
      window.removeEventListener('mousemove', onMove);
      window.removeEventListener('mouseup', onUp);
    };
    window.addEventListener('mousemove', onMove);
    window.addEventListener('mouseup', onUp);
  };

  /* Double-click the handle to put one column back to its default - quicker
     than dragging back to a number you no longer remember. */
  const resetWidth = col => setSized(prev => {
    const id = colId(col);
    if (!(id in prev)) return prev;
    const next = { ...prev };
    delete next[id];
    return next;
  });

  /* The grip sits inside the header cell it resizes, on its right edge, so the
     thing you drag is the boundary you are moving.

     A plain function, not a component: declared inside the render it would be a
     new component type on every pass, and React would throw away and rebuild
     all ten handles on every keystroke in the grid. */
  const grip = col => col && (
    <span
      className="rsz"
      title="Drag to resize · double-click to reset"
      onMouseDown={e => startResize(e, col)}
      onDoubleClick={e => { e.stopPropagation(); resetWidth(col); }}
    />
  );
  const byKey = k => columns.find(c => c.key === k);

  const move = useCallback((dr, dc) => {
    setFocus(f => {
      const base = f || { r: 0, c: 0 };
      return {
        r: Math.min(Math.max(base.r + dr, 0), rows.length - 1),
        c: Math.min(Math.max(base.c + dc, 0), maxC)
      };
    });
  }, [rows.length, maxC]);

  const valueAt = useCallback((row, col) => {
    if (col.kind === 'row') {
      const v = row[col.key];
      return v === null || v === undefined ? '' : String(v);
    }
    const d = cellAt(row, col.currency, col.tier);
    return d ? String(d.price ?? '') : '';
  }, []);

  /* Two modes, as in Excel:
       'enter' - you clicked in or started typing. Arrow keys commit and move,
                 so you can tab across a row without touching the mouse.
       'edit'  - F2 or Enter. Left/Right move the caret so a typo can be fixed
                 in place; only Up/Down leave the cell. */
  const beginEdit = useCallback((mode = 'edit') => {
    if (!focus) return;
    const row = rows[focus.r];
    const col = columns[focus.c];
    if (!row || !col || col.flag) return;
    setEditing({ r: focus.r, c: focus.c, value: valueAt(row, col), mode });
  }, [focus, rows, columns, valueAt]);

  /* Jump straight to a cell, rather than stepping. */
  const goto = useCallback((r, c) => setFocus({
    r: Math.min(Math.max(r, 0), rows.length - 1),
    c: Math.min(Math.max(c, 0), maxC)
  }), [rows.length, maxC]);

  /* Select from the shift-anchor to a row, the way shift-arrow does in a
     spreadsheet: the anchor stays put and the far end follows the cursor. */
  const extendTo = useCallback(ri => {
    if (anchor.current === null) anchor.current = focus?.r ?? ri;
    const [a, b] = [anchor.current, ri].sort((x, y) => x - y);
    const next = new Set();
    for (let i = a; i <= b; i++) if (rows[i]) next.add(rows[i].key);
    setSelected(next);
  }, [rows, focus]);

  useEffect(() => {
    const onKey = e => {
      if (editing) return;
      if (e.target.matches('input,select,textarea,button')) return;
      const jump = e.ctrlKey || e.metaKey;
      const r = focus?.r ?? 0, c = focus?.c ?? 0;
      const lastRow = rows.length - 1;

      switch (e.key) {
        /* Ctrl+arrow runs to the edge of the grid, as it does in a sheet where
           every row is filled. Shift+up/down grows the row selection. */
        case 'ArrowUp':
          e.preventDefault();
          if (e.shiftKey) { const t = Math.max(r - 1, 0); goto(t, c); extendTo(t); }
          else if (jump) goto(0, c);
          else move(-1, 0);
          break;
        case 'ArrowDown':
          e.preventDefault();
          if (e.shiftKey) { const t = Math.min(r + 1, lastRow); goto(t, c); extendTo(t); }
          else if (jump) goto(lastRow, c);
          else move(1, 0);
          break;
        case 'ArrowLeft':
          e.preventDefault();
          jump ? goto(r, 0) : move(0, -1);
          break;
        case 'ArrowRight':
          e.preventDefault();
          jump ? goto(r, maxC) : move(0, 1);
          break;

        /* Home/End are the row; with ctrl they are the whole grid. */
        case 'Home':
          e.preventDefault();
          jump ? goto(0, 0) : goto(r, 0);
          break;
        case 'End':
          e.preventDefault();
          jump ? goto(lastRow, maxC) : goto(r, maxC);
          break;

        case 'PageDown':
          e.preventDefault();
          goto(Math.min(r + pageRows(), lastRow), c);
          break;
        case 'PageUp':
          e.preventDefault();
          goto(Math.max(r - pageRows(), 0), c);
          break;

        case 'Tab':
          /* Tab crosses the row and wraps to the next one, rather than
             stopping dead at the last column. */
          e.preventDefault();
          if (e.shiftKey) c === 0 ? goto(r - 1, maxC) : move(0, -1);
          else            c === maxC ? goto(r + 1, 0) : move(0, 1);
          break;

        case 'Escape':
          /* Drop the selection; the focused cell stays where it is. */
          if (selected.size) { e.preventDefault(); setSelected(new Set()); anchor.current = null; }
          break;

        case 'Enter':
        case 'F2':         beginEdit('edit'); e.preventDefault(); break;

        case ' ': {
          e.preventDefault();
          const row = rows[r], col = columns[c];
          /* Ctrl+space takes the whole row, as in a sheet. */
          if (jump) {
            if (row && !row.isDraft) {
              setSelected(new Set([row.key]));
              anchor.current = r;
            }
            break;
          }
          if (col?.flag && row) onEditRow(row, col, row.active === 'N' ? 'Y' : 'N');
          break;
        }

        case 'l':
        case 'L':
          if (jump && e.shiftKey) {
            e.preventDefault();
            if (activeFilters.length) clearFilters();
            else setShowFilters(v => !v);
          }
          break;

        case 'a':
        case 'A':
          if (jump) {
            e.preventDefault();
            setSelected(new Set(rows.filter(x => !x.isDraft).map(x => x.key)));
            anchor.current = 0;
          }
          break;

        case 'c':
        case 'C':
          /* Ctrl+C copies the selected rows, or the focused one if nothing is
             selected - the same thing the right-click menu does. */
          if (jump) {
            e.preventDefault();
            const picked = selected.size ? rows.filter(x => selected.has(x.key))
                                         : (rows[r] ? [rows[r]] : []);
            if (picked.length) onCopyRow(picked);
          }
          break;

        default:
          // start typing to edit, the way a spreadsheet does
          if (e.key.length === 1 && !e.ctrlKey && !e.metaKey && !e.altKey && focus) {
            const col = columns[focus.c];
            if (!col?.flag) {
              setEditing({ r: focus.r, c: focus.c, value: e.key, mode: 'enter' });
              e.preventDefault();
            }
          }
      }
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [move, goto, extendTo, beginEdit, editing, focus, rows, columns, maxC,
      selected, onCopyRow, onEditRow, activeFilters.length]);

  /* Keep the focused cell on screen.

     scrollIntoView is no good here: the header and the three frozen columns
     are sticky, so "in view" by its reckoning still means underneath them.
     These insets are measured from the live elements rather than assumed, so
     they stay right when a frozen column is resized. */
  useEffect(() => {
    if (!focus) return;
    const table = tableRef.current;
    const pane  = table?.closest('.gridwrap');
    const cell  = table?.querySelector(`td[data-r="${focus.r}"][data-c="${focus.c}"]`);
    if (!pane || !cell) return;

    const head = table.querySelector('thead');
    const topInset  = head ? head.getBoundingClientRect().height : 0;
    /* everything frozen to the left: gutter + Act + Design No */
    const frozen = table.querySelector('tbody tr td.s1');
    const leftInset = frozen
      ? frozen.getBoundingClientRect().right - pane.getBoundingClientRect().left
      : 0;

    const p = pane.getBoundingClientRect();
    const b = cell.getBoundingClientRect();

    if (b.top    < p.top + topInset)  pane.scrollTop  -= (p.top + topInset) - b.top;
    else if (b.bottom > p.bottom)     pane.scrollTop  += b.bottom - p.bottom;

    /* A frozen cell is always on screen, so there is nothing to scroll it
       into - but there is nothing to its left either, so landing on one means
       going back to the start of the sheet. Without this, Home moves the
       cursor and the view stays where it was, which reads as nothing having
       happened. */
    if (cell.classList.contains('stk')) pane.scrollLeft = 0;
    else if (b.left  < p.left + leftInset) pane.scrollLeft -= (p.left + leftInset) - b.left;
    else if (b.right > p.right)           pane.scrollLeft += b.right - p.right;
  }, [focus]);

  /* Enter, Tab and the arrow keys all commit and then move, which unmounts the
     input - and the blur handler would commit the same edit a second time.
     Remember what was just written and ignore a repeat of it. */
  const commit = async () => {
    if (!editing) return;
    const row = rows[editing.r];
    const col = columns[editing.c];
    if (!row || !col) { setEditing(null); return; }

    const raw = (editing.value ?? '').trim();

    /* Validate BEFORE closing the editor, so a rejected value stays on screen
       with the cursor in it rather than silently reverting. */
    if (col.kind === 'price') {
      if (raw !== '' && !IS_PRICE.test(raw)) {
        onInvalid?.(`"${raw}" is not a price. Use digits, e.g. 3.25`);
        setInvalid(true);
        return;
      }
    } else if (col.num) {
      if (raw !== '' && !IS_QTY.test(raw)) {
        onInvalid?.(`"${raw}" is not a quantity. Use whole numbers, e.g. 600`);
        setInvalid(true);
        return;
      }
    } else if (col.date) {
      if (raw !== '' && !dateOk(raw)) {
        onInvalid?.(`"${raw}" is not a date. Use YYYY-MM-DD, e.g. 2025-07-25`);
        setInvalid(true);
        return;
      }
    }

    setInvalid(false);
    const stamp = `${editing.r}:${editing.c}:${raw}`;
    if (lastCommit.current === stamp) { setEditing(null); return; }
    lastCommit.current = stamp;
    setEditing(null);
    if (raw === valueAt(row, col)) return;          // nothing changed

    if (col.kind === 'price') {
      if (raw === '') return;                       // clearing a price is a delete, not an edit
      await onEditPrice(row, col.currency, col.tier, Number(raw));
    } else {
      await onEditRow(row, col, raw);
    }
  };

  if (!rows.length) {
    // Only claim the default is typical when it actually IS the default -
    // once the columns are customised that sentence is just wrong.
    const isDefault =
      currencies.length === 1 && currencies[0] === 'USD' &&
      tiers.length === 2 && tiers.includes('PFE/PFD') && tiers.includes('All_colors');

    return (
      <div className="empty">
        <p className="big">This price list has no lines yet.</p>
        <p>
          Columns: <b>{tiers.join(' + ')}</b> in <b>{currencies.join(' + ')}</b>
          {isDefault
            ? ' — the shape most Eschler lists use.'
            : ' — set for this list.'}
          {' '}Change them with <b>Columns</b> above.
        </p>
        <button onClick={onAddRow}>+ Add the first line</button>
      </div>
    );
  }

  let prevArticle = null;

  return (
    <>
    <table id="grid" ref={tableRef}
           style={{
             width: tableWidth,
             /* The frozen columns stack left to right by CSS offset. Deriving
                those offsets from the live widths is what lets Act and Design
                No be resized without the sticky columns sliding over each
                other. */
             '--w0':   `${GUTTER_W}px`,
             '--wact': `${colWidth(byKey('active'))}px`,
             '--w1':   `${colWidth(byKey('design_no'))}px`
           }}>
      {/* fixed geometry: one <col> per column, same on every list */}
      <colgroup>
        <col style={{ width: GUTTER_W }} />
        {columns.map((c, i) => (
          <col
            key={c.key ? `c-${c.key}` : `p-${c.currency}-${c.tier}-${i}`}
            style={{ width: colWidth(c) }}
          />
        ))}
      </colgroup>
      <thead>
        <tr>
          <th className="stk s0 fbtn" rowSpan={2}>
            <button
              className={activeFilters.length ? 'on' : ''}
              title={activeFilters.length
                ? `${activeFilters.length} column filter${activeFilters.length > 1 ? 's' : ''} · ${hidden} row${hidden === 1 ? '' : 's'} hidden — Ctrl+Shift+L to clear`
                : 'Filter columns (Ctrl+Shift+L)'}
              onClick={() => setShowFilters(v => !v)}
            >▼</button>
          </th>
          <th className="stk sAct" rowSpan={2} title="Active — N is a withdrawn price">
            Act{grip(byKey('active'))}
          </th>
          <th className="stk s1" rowSpan={2}>
            Design No{grip(byKey('design_no'))}
          </th>
          <th rowSpan={2}>Price date{grip(byKey('price_line_date'))}</th>
          <th className="grp" colSpan={3}>Quantity</th>
          {currencies.map(ccy => (
            <th key={ccy} className={`grp ${ccy === 'USD' ? 'usd' : 'thb'}`} colSpan={tiers.length}>
              {ccy} · {ccy === 'USD' ? '$' : '฿'} per unit
            </th>
          ))}
          <th className="grp" colSpan={INFO_COLS.length}>Design detail</th>
        </tr>
        <tr>
          <th className="num">Min{grip(byKey('qty_min'))}</th>
          <th className="num">Max{grip(byKey('qty_max'))}</th>
          <th>Unit{grip(byKey('qty_unit'))}</th>
          {currencies.flatMap(ccy =>
            tiers.map(t => {
              const col = columns.find(c => c.kind === 'price' &&
                                            c.currency === ccy && c.tier === t);
              return (
                <th key={`${ccy}${t}`} className={`num ${ccy === 'USD' ? 'usdc' : 'thbc'}`}>
                  {t}{grip(col)}
                </th>
              );
            }))}
          {INFO_COLS.map(c => (
            <th key={c.key}>{c.label}{grip(byKey(c.key))}</th>
          ))}
        </tr>

        {/* One box per column. Sticky like the two rows above it, so it stays
            put while the rows scroll under it. */}
        {showFilters && (
          <tr className="filterrow">
            <th className="stk s0 fx">
              {activeFilters.length > 0 && (
                <button className="fclear" title="Clear every filter"
                        onClick={clearFilters}>×</button>
              )}
            </th>
            {columns.map((c, i) => {
              const id = colId(c);
              const on = (colFilter[id] || '').trim();
              const sticky = c.flag ? 'stk sAct ' : c.sticky ? 'stk s1 ' : '';
              return (
                <th key={`f-${id}-${i}`} className={`${sticky}fx${on ? ' on' : ''}`}>
                  <input
                    className="fin"
                    value={colFilter[id] || ''}
                    placeholder={c.kind === 'price' || c.num ? '>0' : '…'}
                    title={c.kind === 'price' || c.num
                      ? 'Type to match, or compare: >5  <=2.5  =0.  "=" alone finds blanks.'
                      : 'Type to match. "=" alone finds blanks.'}
                    onChange={e => setFilter(id, e.target.value)}
                    onKeyDown={e => {
                      e.stopPropagation();
                      if (e.key === 'Escape') { e.preventDefault(); setFilter(id, ''); }
                    }}
                  />
                </th>
              );
            })}
          </tr>
        )}
      </thead>
      <tbody>
        {rows.map((row, ri) => {
          const newArt = row.design_no !== prevArticle;
          prevArticle = row.design_no;

          return (
            <tr key={row.key}
                className={`${newArt ? 'newart' : ''}${row.active === 'N' ? ' rowinactive' : ''}${selected.has(row.key) ? ' selrow' : ''}`}
                onContextMenu={e => {
                  /* A draft has no set_no yet - there is nothing on the server
                     to copy or delete - so it gets the browser's own menu. */
                  if (row.isDraft) return;
                  e.preventDefault();
                  /* Right-clicking outside the selection moves to that row, the
                     way a spreadsheet does; inside it, the selection stands and
                     the menu acts on all of it. */
                  if (!selected.has(row.key)) {
                    setSelected(new Set([row.key]));
                    anchor.current = ri;
                  }
                  setMenu({ x: e.clientX, y: e.clientY, row });
                }}>
              <td className={[
                    'stk', 's0', 'rn',
                    row.isDraft ? '' : 'pick',
                    selected.has(row.key) ? 'picked' : '',
                    row.groupSize > 1 ? 'grp' : '',
                    row.groupSize > 1 && row.groupPos === 0 ? 'grpfirst' : '',
                    row.groupSize > 1 && row.groupPos === row.groupSize - 1 ? 'grplast' : ''
                  ].filter(Boolean).join(' ')}
                  onClick={e => { if (!row.isDraft) pickRow(ri, e); }}
                  title={row.isDraft ? undefined
                    : (row.groupSize > 1
                        ? `${row.groupPos + 1} of ${row.groupSize} prices for ${row.design_no}, ${row.qty_min}–${row.qty_max === null ? '∞' : row.qty_max} ${row.qty_unit}\nClick to select · shift-click for a block`
                        : 'Click to select · shift-click for a block')}>
                {row.isDraft ? '＋' : ri + 1}
              </td>

              {columns.map((col, ci) => {
                const isFocus = focus?.r === ri && focus?.c === ci;
                const isEdit  = editing?.r === ri && editing?.c === ci;
                const d = col.kind === 'price'
                  ? cellAt(row, col.currency, col.tier) : null;

                const cls = ['cell'];
                if (col.kind === 'price') {
                  cls.push('num', col.currency === 'USD' ? 'usdc' : 'thbc');
                } else if (col.flag) {
                  cls.push('stk', 'sAct', 'flagcell');
                  if (row.active === 'N') cls.push('inactive');
                } else {
                  if (col.key === 'notes' && row.notes) cls.push('hasnote');
                  /* text too long for the column stays reachable: the cell
                     scrolls sideways rather than ending in an ellipsis */
                  if (col.wide) cls.push('wcell');
                  if (col.num) cls.push('num', 'mono');
                  if (col.sticky) cls.push('stk', 's1', 'mono');
                  /* The fabric and spec columns used to be greyed as secondary
                     detail. They are not: they are data being reviewed and
                     corrected, and grey text reads as disabled or less
                     trustworthy. Every column now carries the same weight. */
                }
                if (isFocus) cls.push('foc');

                const display = col.kind === 'price'
                  ? (d ? <span className="val mono">{money(d.price, col.currency)}</span>
                       : <span className="nil">·</span>)
                  : col.flag
                  ? <input
                      type="checkbox"
                      className="flagbox"
                      checked={row.active !== 'N'}
                      title={row.active === 'N'
                        ? 'Withdrawn — order entry will not offer this price'
                        : 'Active'}
                      onChange={e => onEditRow(row, col, e.target.checked ? 'Y' : 'N')}
                      onClick={e => e.stopPropagation()}
                    />
                  : (col.key === 'qty_max' && (row.qty_max === null || row.qty_max === '')
                      ? <span className="nil">∞</span>
                      : (row[col.key] || <span className="nil">·</span>));

                return (
                  <td
                    key={col.key || `${col.currency}${col.tier}`}
                    className={cls.join(' ')}
                    title={
                      col.kind === 'price' && d && isRounded(d.price)
                        ? `Stored as ${d.price} — shown to two places`
                        : col.wide && row[col.key] ? row[col.key] : undefined
                    }
                    data-r={ri} data-c={ci}
                    onClick={e => {
                      /* The bottom few pixels of a wide cell are its own
                         scrollbar. A click there is someone reading the rest
                         of a note - swapping the text for an input mid-drag
                         would take it away from them. */
                      if (col.wide) {
                        const box = e.currentTarget.getBoundingClientRect();
                        if (box.bottom - e.clientY <= SCROLL_H) return;
                      }
                      setFocus({ r: ri, c: ci });
                      // a single click opens the cell - the checkbox column
                      // handles its own toggle and must not become a text box
                      if (!col.flag && !(editing?.r === ri && editing?.c === ci)) {
                        setEditing({ r: ri, c: ci, value: valueAt(row, col), mode: 'enter' });
                      }
                    }}
                  >
                    {isEdit ? (
                      <input
                        /* A native picker on a date column: the calendar is
                           there for people who want it, and the field still
                           accepts a typed YYYY-MM-DD for people who would
                           rather not reach for the mouse. */
                        type={col.date ? 'date' : 'text'}
                        className={`edit${col.kind === 'price' || col.num ? ' mono' : ''}${col.date ? ' datein' : ''}${invalid ? ' bad' : ''}`}
                        style={col.kind === 'price' || col.num ? undefined : { textAlign: 'left' }}
                        autoFocus
                        value={editing.value}
                        onChange={e => { setInvalid(false); setEditing({ ...editing, value: e.target.value }); }}
                        onBlur={commit}
                        onKeyDown={e => {
                          e.stopPropagation();
                          const k = e.key;
                          const typing = editing.mode === 'enter';
                          const inp = e.currentTarget;
                          const atStart = inp.selectionStart === 0 && inp.selectionEnd === 0;
                          const atEnd   = inp.selectionStart === inp.value.length &&
                                          inp.selectionEnd === inp.value.length;

                          if (k === 'Enter')       { e.preventDefault(); commit(); move(1, 0); }
                          else if (k === 'Escape') { e.preventDefault(); setInvalid(false); setEditing(null); }
                          else if (k === 'Tab')    { e.preventDefault(); commit(); move(0, e.shiftKey ? -1 : 1); }
                          /* A date field spends its arrow keys on its own
                             segments - up and down step the day, left and
                             right move between day, month and year. Taking
                             them away to move the cursor would make the picker
                             useless, so on a date cell they stay with it and
                             Enter, Tab or Escape are the way out. */
                          else if (col.date) { /* leave the arrows to the picker */ }
                          // Up/Down always leave the cell - a single-line input
                          // has nowhere for them to go anyway
                          else if (k === 'ArrowUp')   { e.preventDefault(); commit(); move(-1, 0); }
                          else if (k === 'ArrowDown') { e.preventDefault(); commit(); move(1, 0); }
                          // Left/Right leave while typing; in edit mode they
                          // move the caret until it reaches the end of the text
                          else if (k === 'ArrowLeft'  && (typing || atStart)) {
                            e.preventDefault(); commit(); move(0, -1);
                          }
                          else if (k === 'ArrowRight' && (typing || atEnd)) {
                            e.preventDefault(); commit(); move(0, 1);
                          }
                        }}
                      />
                    ) : col.wide ? (
                      <div className="scrollcell">{display}</div>
                    ) : display}
                  </td>
                );
              })}
            </tr>
          );
        })}
        <tr className="addrow">
          <td className="stk s0" />
          <td colSpan={columns.length}>
            <button className="ghost" onClick={onAddRow}>+ Add line</button>
          </td>
        </tr>
      </tbody>
    </table>

    {menu && (() => {
      /* What the menu acts on: the selection if the clicked row is part of
         one, otherwise just that row. onContextMenu has already made sure the
         clicked row is in the selection, so this is only ever a widening. */
      const acting = selectedRows.length > 1 && selected.has(menu.row.key)
        ? selectedRows : [menu.row];
      const many = acting.length > 1;

      return (
      /* Positioned against the viewport, so it is not clipped by the grid's
         own scroll container. onClick stops here: the window listener that
         closes the menu would otherwise fire before the action runs. */
      <ul className="ctxmenu"
          style={{ left: menu.x, top: menu.y }}
          onClick={e => e.stopPropagation()}>
        <li onClick={() => { onCopyRow(acting); setMenu(null); }}>
          {many ? `Copy ${acting.length} lines` : 'Copy line'}
          {many && <span className="ctxnote">
            {acting.map(r => r.design_no).join(', ')}
          </span>}
        </li>

        {/* Always available. With something copied it inserts that; with
            nothing copied it opens a blank line in the same place, so this is
            the one way to add a line anywhere but the end. */}
        <li onClick={() => { onInsertCopied(menu.row); setMenu(null); }}>
          Insert here
          {copied && <span className="ctxnote ctxwhat">{describe(copied)}</span>}
        </li>

        {/* With a row on the clipboard "Insert here" means that row, so there
            has to be a second way to say "a new empty one". Without a
            clipboard the item above already does this, and a second entry
            saying the same thing would just be noise. */}
        {copied && (
          <li onClick={() => { onInsertBlank(menu.row); setMenu(null); }}>
            Insert here blank
            <span className="ctxnote">an empty line</span>
          </li>
        )}

        <li className="danger"
            onClick={() => { onDeleteRow(menu.row); setMenu(null); }}>
          Delete line
        </li>
      </ul>
      );
    })()}
    </>
  );
}
