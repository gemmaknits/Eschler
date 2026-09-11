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
  { key: 'active',   label: 'Act',     width: 52, flag: true },
  { key: 'design_no', label: 'Design No', sticky: true, width: 118 },
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
     who quoted it, when, on what terms - so they are worth a column. Long
     ones truncate; the full text is on hover and in the editor. */
  { key: 'notes',           label: 'Notes',      width: 300, wide: true }
];

/* Every column has a fixed width, and the table is laid out fixed, so the grid
   has identical geometry on every price list. Sized to content it would shift
   column to column and list to list, and a long fabric name would squeeze the
   numbers - the thing you actually read. */
const GUTTER_W = 42;
const PRICE_W  = 96;

/* One detail per cell. Alternatives for the same article and quantity band are
   separate rows, bracketed together by a rail in the row-number gutter - so
   there is nothing hidden behind a badge and nothing to click to reveal. */
const cellAt = (row, ccy, tier) => row.cells?.[`${ccy}|${tier}`] || null;

/* Whole string or nothing. parseFloat('3.5xyz') is 3.5 and parseInt('12abc')
   is 12, so a typo would be silently truncated and saved. */
const IS_QTY   = /^\d{1,9}$/;                  // non-negative whole number
const IS_PRICE = /^\d{1,12}(\.\d{1,4})?$/;     // decimal(18,4), non-negative

export default function PriceGrid({
  grid, tiers, currencies,
  onEditPrice, onEditRow, onAddRow, onInvalid,
  onCopyRow, onInsertCopied, onDeleteRow, copied
}) {
  const [focus, setFocus] = useState(null);      // {r, c} - c indexes ALL columns
  const [editing, setEditing] = useState(null);  // {r, c, value, mode}
  const tableRef = useRef(null);
  const lastCommit = useRef(null);
  const [invalid, setInvalid] = useState(false);
  /* {x, y, row} - where the menu sits and which row it acts on. Held here
     rather than per row so only one can ever be open. */
  const [menu, setMenu] = useState(null);

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
        case 'F2':         beginEdit('edit'); e.preventDefault(); break;
        case ' ': {
          if (!focus) break;
          const row = rows[focus.r], col = columns[focus.c];
          if (col?.flag) {
            onEditRow(row, col, row.active === 'N' ? 'Y' : 'N');
            e.preventDefault();
            break;
          }
          e.preventDefault();
          break;
        }
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
  }, [move, beginEdit, editing, focus, rows, columns]);

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

  const colWidth = c => (c.kind === 'price' ? PRICE_W : (c.width || 120));
  /* table-layout:fixed only honours <col> widths when the table has an explicit
     width; left to width:auto it stretches the columns to fill the pane, which
     is exactly the data-dependent sizing we are trying to avoid. */
  const tableWidth = GUTTER_W + columns.reduce((a, c) => a + colWidth(c), 0);

  return (
    <>
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
          <th className="stk sAct" rowSpan={2} title="Active — N is a withdrawn price">Act</th>
          <th className="stk s1" rowSpan={2}>Design No</th>
          <th className="grp" colSpan={3}>Quantity</th>
          {currencies.map(ccy => (
            <th key={ccy} className={`grp ${ccy === 'USD' ? 'usd' : 'thb'}`} colSpan={tiers.length}>
              {ccy} · {ccy === 'USD' ? '$' : '฿'} per unit
            </th>
          ))}
          <th className="grp" colSpan={INFO_COLS.length}>Design detail</th>
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
          const newArt = row.design_no !== prevArticle;
          prevArticle = row.design_no;

          return (
            <tr key={row.key}
                className={`${newArt ? 'newart' : ''}${row.active === 'N' ? ' rowinactive' : ''}`}
                onContextMenu={e => {
                  /* A draft has no set_no yet - there is nothing on the server
                     to copy or delete - so it gets the browser's own menu. */
                  if (row.isDraft) return;
                  e.preventDefault();
                  setMenu({ x: e.clientX, y: e.clientY, row });
                }}>
              <td className={[
                    'stk', 's0', 'rn',
                    row.groupSize > 1 ? 'grp' : '',
                    row.groupSize > 1 && row.groupPos === 0 ? 'grpfirst' : '',
                    row.groupSize > 1 && row.groupPos === row.groupSize - 1 ? 'grplast' : ''
                  ].filter(Boolean).join(' ')}
                  title={row.groupSize > 1
                    ? `${row.groupPos + 1} of ${row.groupSize} prices for ${row.design_no}, ${row.qty_min}–${row.qty_max === null ? '∞' : row.qty_max} ${row.qty_unit}`
                    : undefined}>
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
                  if (col.num) cls.push('num', 'mono');
                  if (col.sticky) cls.push('stk', 's1', 'mono');
                  if (!col.num && !col.sticky) cls.push('dim');
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
                    title={col.wide && row[col.key] ? row[col.key] : undefined}
                    data-r={ri} data-c={ci}
                    onClick={() => {
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
                        className={`edit${col.kind === 'price' || col.num ? ' mono' : ''}${invalid ? ' bad' : ''}`}
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

    {menu && (
      /* Positioned against the viewport, so it is not clipped by the grid's
         own scroll container. onClick stops here: the window listener that
         closes the menu would otherwise fire before the action runs. */
      <ul className="ctxmenu"
          style={{ left: menu.x, top: menu.y }}
          onClick={e => e.stopPropagation()}>
        <li onClick={() => { onCopyRow(menu.row); setMenu(null); }}>
          Copy line
        </li>
        <li className={copied ? '' : 'disabled'}
            onClick={() => {
              if (!copied) return;
              onInsertCopied(menu.row);
              setMenu(null);
            }}>
          {copied
            ? `Insert copied (${copied.design_no}) below`
            : 'Insert copied — nothing copied yet'}
        </li>
        <li className="danger"
            onClick={() => { onDeleteRow(menu.row); setMenu(null); }}>
          Delete line
        </li>
      </ul>
    )}
    </>
  );
}
