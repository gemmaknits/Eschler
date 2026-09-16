import { useState, useEffect, useRef, useCallback } from 'react';
import { api } from './api';

const asDate = v => (v ? String(v).slice(0, 10) : '');

/**
 * Customer list of values.
 *
 * A grid rather than a type-ahead, because choosing a customer here is often a
 * comparison, not a lookup: names repeat (three "Agent Provocateur" records,
 * three "Unichela Pvt Ltd."), and the columns that separate them - code,
 * country, order count, last order - have to be visible side by side.
 * Rows are ordered by order count, so the record actually traded on is first.
 */
export default function CustomerLov({ initialFilter = '', onPick, onClose }) {
  const [filter, setFilter]   = useState(initialFilter);
  const [rows, setRows]       = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState(null);
  const [activeOnly, setActiveOnly] = useState(false);
  const [cursor, setCursor]   = useState(0);
  const inputRef = useRef(null);
  const bodyRef  = useRef(null);

  useEffect(() => { inputRef.current?.focus(); inputRef.current?.select(); }, []);

  useEffect(() => {
    let alive = true;
    setLoading(true);
    const t = setTimeout(() => {
      api.lovCustomer({ filter, active_only: activeOnly ? 1 : 0, top_n: 200 })
        .then(r => { if (alive) { setRows(r); setCursor(0); setLoading(false); } })
        .catch(err => { if (alive) { setError(err.message); setLoading(false); } });
    }, filter ? 220 : 0);
    return () => { alive = false; clearTimeout(t); };
  }, [filter, activeOnly]);

  const choose = useCallback(row => {
    onPick({ customer_id: row.customer_id, customer_name: row.name, custcd: row.custcd });
  }, [onPick]);

  const onKey = e => {
    if (e.key === 'Escape') { e.preventDefault(); onClose(); return; }
    if (!rows.length) return;
    if (e.key === 'ArrowDown') { e.preventDefault(); setCursor(c => Math.min(c + 1, rows.length - 1)); }
    else if (e.key === 'ArrowUp') { e.preventDefault(); setCursor(c => Math.max(c - 1, 0)); }
    else if (e.key === 'Enter') { e.preventDefault(); choose(rows[cursor]); }
  };

  // keep the keyboard cursor in view
  useEffect(() => {
    bodyRef.current?.querySelector('tr.cur')?.scrollIntoView({ block: 'nearest' });
  }, [cursor]);

  return (
    <div className="modalwrap" onMouseDown={e => { if (e.target === e.currentTarget) onClose(); }}>
      <div className="modal lov" role="dialog" aria-label="Select a customer" onKeyDown={onKey}>
        <h3>Select customer</h3>
        <p className="modalsub">
          Ordered by sales-order activity — where a name repeats, the record actually traded on is first.
        </p>

        <div className="lovbar">
          <input
            ref={inputRef}
            value={filter}
            placeholder="Filter by name, code or city…"
            onChange={e => setFilter(e.target.value)}
          />
          <label className="chk">
            <input type="checkbox" checked={activeOnly} onChange={e => setActiveOnly(e.target.checked)} />
            <span>Active only</span>
          </label>
          <span className="spacer" />
          <span className="lovcount">
            {loading ? 'searching…' : `${rows.length}${rows.length === 200 ? '+' : ''} match${rows.length === 1 ? '' : 'es'}`}
          </span>
        </div>

        {error && <div className="modalerr" role="alert">{error}</div>}

        <div className="lovgrid" ref={bodyRef}>
          <table>
            <thead>
              <tr>
                <th>Code</th>
                <th>Customer</th>
                <th>Ctry</th>
                <th>City</th>
                <th className="num">Orders</th>
                <th>Last order</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((c, i) => (
                <tr
                  key={c.customer_id}
                  className={`${i === cursor ? 'cur' : ''}${c.active ? '' : ' inactive'}`}
                  onMouseEnter={() => setCursor(i)}
                  onClick={() => choose(c)}
                  onDoubleClick={() => choose(c)}
                >
                  <td className="mono">{c.custcd}</td>
                  <td>
                    {c.name}
                    {!c.active && <span className="lovtag">inactive</span>}
                  </td>
                  <td className="mono">{c.ctry}</td>
                  <td className="dim">{c.city || '—'}</td>
                  <td className="num mono">{c.so_orders}</td>
                  <td className="mono dim">{asDate(c.last_so_date) || '—'}</td>
                </tr>
              ))}
              {!loading && rows.length === 0 && (
                <tr><td colSpan={6} className="lovempty">No customer matches “{filter}”.</td></tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="modalactions">
          <button className="primary" onClick={() => rows[cursor] && choose(rows[cursor])} disabled={!rows.length}>
            Select
          </button>
          <button className="ghost" onClick={onClose}>Cancel</button>
          <span className="spacer" />
          <span className="lovhint"><kbd>↑↓</kbd> move · <kbd>Enter</kbd> select · <kbd>Esc</kbd> close</span>
        </div>
      </div>
    </div>
  );
}
