import { useState, useRef, useEffect, useCallback } from 'react';

const money = (v, ccy) => {
  if (v === null || v === undefined || v === '') return '';
  const n = Number(v);
  return ccy === 'THB'
    ? n.toFixed(2).replace(/\.00$/, '')
    : n.toFixed(4).replace(/0+$/, '').replace(/\.$/, '');
};

/* Row-level columns, editable. `num` writes an integer, `blank` means the empty
   string clears the value (qty_max empty = open upper bound). */
/* Variant is hidden, not dropped: it is still part of the business key that
   identifies a row, so it keeps travelling with every read and row update -
   it just has no column, because in the imported data it is empty on all
   4,468 lines. */
export const ROW_COLS = [
  { key: 'article',  label: 'Article', sticky: true, width: 118 },
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
  { key: 'moq',             label: 'MOQ',        width: 104 }
];

/* Every column has a fixed width, and the table is laid out fixed, so the grid
   has identical geometry on every price list. Sized to content it would shift
   column to column and list to list, and a long fabric name would squeeze the
   numbers - the thing you actually read. */
const GUTTER_W = 42;
const PRICE_W  = 96;

const cellKeyOf = (row, ccy, tier) => `${row.key}|${ccy}|${tier}`;

function winner(row, ccy, tier, picked) {
  const list = row.cells?.[`${ccy}|${tier}`] || [];
  if (list.length === 0) return null;
  if (list.length === 1) return list[0];
  const id = picked[cellKeyOf(row, ccy, tier)];
  return list.find(d => d.so_price_list_detail_id === id) || list[0];
}

export default function PriceGrid({
  grid, tiers, currencies, picked,
  onEditPrice, onEditRow, onPick, onAddRow
}) {
  const [focus, setFocus] = useState(null);      // {r, c} - c indexes ALL columns
  const [editing, setEditing] = useState(null);  // {r, c, value}
  const tableRef = useRef(null);

  const { rows } = grid;

  /* One flat column model, so arrow keys cross field columns and price columns
     alike instead of stopping at the boundary. */
  const columns = [
    ...ROW_COLS.map(c => ({ kind: 'row', ...c })),
    ...currencies.flatMap(ccy =>
      tiers.map(tier => ({ kind: 'price', currency: ccy, tier, label: tier }))),
    ...INFO_COLS.map(c => ({ kind: 'row', ...c }))
  ];
  const maxC = columns.length - 1;

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
    const w = winner(row, col.currency, col.tier, picked);
    return w ? String(w.price ?? '') : '';
  }, [picked]);

  const beginEdit = useCallback(() => {
    if (!focus) return;
    const row = rows[focus.r];
    const col = columns[focus.c];
    if (!row || !col) return;
    setEditing({ r: focus.r, c: focus.c, value: valueAt(row, col) });
  }, [focus, rows, columns, valueAt]);

  useEffect(() => {
    const onKey = e => {
      if (editing) return;
      if (e.target.matches('input,select,textarea,button')) return;
      switch (e.key) {
        case 'ArrowUp':    move(-1, 0); e.preventDefault(); break;
        case 'ArrowDown':  move(1, 0);  e.preventDefault(); break;
        case 'ArrowLeft':  move(0, -1); e.preventDefault(); break;
        case 'ArrowRight': move(0, 1);  e.preventDefault(); break;
        case 'Enter':
        case 'F2':         beginEdit(); e.preventDefault(); break;
        case ' ': {
          if (!focus) break;
          const row = rows[focus.r], col = columns[focus.c];
          if (col?.kind === 'price') {
            const list = row?.cells?.[`${col.currency}|${col.tier}`] || [];
            if (list.length > 1) {
              const td = tableRef.current?.querySelector(
                `td[data-r="${focus.r}"][data-c="${focus.c}"]`);
              onPick(row, col.currency, col.tier, td?.getBoundingClientRect());
            }
          }
          e.preventDefault();
          break;
        }
        default:
          // start typing to edit, the way a spreadsheet does
          if (e.key.length === 1 && !e.ctrlKey && !e.metaKey && !e.altKey && focus) {
            setEditing({ r: focus.r, c: focus.c, value: e.key });
            e.preventDefault();
          }
      }
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [move, beginEdit, editing, focus, rows, columns, onPick]);

  const commit = async () => {
    if (!editing) return;
    const row = rows[editing.r];
    const col = columns[editing.c];
    const raw = editing.value;
    setEditing(null);
    if (!row || !col) return;
    if (raw === valueAt(row, col)) return;          // nothing changed

    if (col.kind === 'price') {
      const n = parseFloat(raw);
      if (raw === '' || Number.isNaN(n) || n < 0) return;
      await onEditPrice(row, col.currency, col.tier, n);
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

  const colWidth = c => (c.kind === 'price' ? PRICE_W : (c.width || 120));
  /* table-layout:fixed only honours <col> widths when the table has an explicit
     width; left to width:auto it stretches the columns to fill the pane, which
     is exactly the data-dependent sizing we are trying to avoid. */
  const tableWidth = GUTTER_W + columns.reduce((a, c) => a + colWidth(c), 0);

  return (
    <table id="grid" ref={tableRef} style={{ width: tableWidth }}>
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
          <th className="stk s0" rowSpan={2} />
          <th className="stk s1" rowSpan={2}>Article</th>
          <th className="grp" colSpan={3}>Quantity</th>
          {currencies.map(ccy => (
            <th key={ccy} className={`grp ${ccy === 'USD' ? 'usd' : 'thb'}`} colSpan={tiers.length}>
              {ccy} · {ccy === 'USD' ? '$' : '฿'} per unit
            </th>
          ))}
          <th className="grp" colSpan={INFO_COLS.length}>Article detail</th>
        </tr>
        <tr>
          <th className="num">Min</th>
          <th className="num">Max</th>
          <th>Unit</th>
          {currencies.flatMap(ccy =>
            tiers.map(t => (
              <th key={`${ccy}${t}`} className={`num ${ccy === 'USD' ? 'usdc' : 'thbc'}`}>{t}</th>
            )))}
          {INFO_COLS.map(c => <th key={c.key}>{c.label}</th>)}
        </tr>
      </thead>
      <tbody>
        {rows.map((row, ri) => {
          const newArt = row.article !== prevArticle;
          prevArticle = row.article;

          return (
            <tr key={row.key} className={newArt ? 'newart' : ''}>
              <td className="stk s0 rn">{row.isDraft ? '＋' : ri + 1}</td>

              {columns.map((col, ci) => {
                const isFocus = focus?.r === ri && focus?.c === ci;
                const isEdit  = editing?.r === ri && editing?.c === ci;
                const list = col.kind === 'price'
                  ? (row.cells?.[`${col.currency}|${col.tier}`] || []) : [];
                const w = col.kind === 'price'
                  ? winner(row, col.currency, col.tier, picked) : null;

                const cls = ['cell'];
                if (col.kind === 'price') {
                  cls.push('num', col.currency === 'USD' ? 'usdc' : 'thbc');
                  if (list.length > 1) cls.push('conf');
                } else {
                  if (col.num) cls.push('num', 'mono');
                  if (col.sticky) cls.push('stk', 's1', 'mono');
                  if (!col.num && !col.sticky) cls.push('dim');
                }
                if (isFocus) cls.push('foc');

                const display = col.kind === 'price'
                  ? (w ? <>
                        <span className="val mono">{money(w.price, col.currency)}</span>
                        {list.length > 1 && <span className="badge">{list.length}</span>}
                      </>
                    : <span className="nil">·</span>)
                  : (col.key === 'qty_max' && (row.qty_max === null || row.qty_max === '')
                      ? <span className="nil">∞</span>
                      : (row[col.key] || <span className="nil">·</span>));

                return (
                  <td
                    key={col.key || `${col.currency}${col.tier}`}
                    className={cls.join(' ')}
                    data-r={ri} data-c={ci}
                    onClick={e => {
                      setFocus({ r: ri, c: ci });
                      if (e.target.closest('.badge')) {
                        onPick(row, col.currency, col.tier, e.currentTarget.getBoundingClientRect());
                      }
                    }}
                    onDoubleClick={e => {
                      setFocus({ r: ri, c: ci });
                      if (col.kind === 'price' && list.length > 1) {
                        onPick(row, col.currency, col.tier, e.currentTarget.getBoundingClientRect());
                      } else {
                        setEditing({ r: ri, c: ci, value: valueAt(row, col) });
                      }
                    }}
                  >
                    {isEdit ? (
                      <input
                        className={`edit${col.kind === 'price' || col.num ? ' mono' : ''}`}
                        style={col.kind === 'price' || col.num ? undefined : { textAlign: 'left' }}
                        autoFocus
                        value={editing.value}
                        onChange={e => setEditing({ ...editing, value: e.target.value })}
                        onBlur={commit}
                        onKeyDown={e => {
                          e.stopPropagation();
                          if (e.key === 'Enter') { e.preventDefault(); commit(); move(1, 0); }
                          else if (e.key === 'Escape') { e.preventDefault(); setEditing(null); }
                          else if (e.key === 'Tab') { e.preventDefault(); commit(); move(0, e.shiftKey ? -1 : 1); }
                        }}
                      />
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
  );
}
