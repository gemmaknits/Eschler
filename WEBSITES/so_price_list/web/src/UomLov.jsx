import { useState, useEffect, useRef, useCallback } from 'react';
import { api } from './api';

/**
 * Unit of measure picker.
 *
 * Short list — 23 units — so it opens showing all of them rather than waiting
 * for a search. The length units come first, because a price list is quoted
 * per metre and MTS is what nearly every line wants.
 *
 * This is also the answer to a typo: the write procedures refuse a unit that
 * is not in dbo.uom, so rather than reporting that as an error the grid opens
 * this and lets the unit be chosen.
 */
export default function UomLov({ initialFilter = '', invalid = null, onPick, onClose }) {
  const [filter, setFilter] = useState('');
  const [rows, setRows]     = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError]   = useState(null);
  const [cursor, setCursor] = useState(0);
  const inputRef = useRef(null);
  const bodyRef  = useRef(null);

  useEffect(() => { inputRef.current?.focus(); }, []);

  useEffect(() => {
    let alive = true;
    setLoading(true);
    api.lovUom({ filter })
      .then(r => { if (alive) { setRows(r); setCursor(0); setLoading(false); } })
      .catch(err => { if (alive) { setError(err.message); setLoading(false); } });
    return () => { alive = false; };
  }, [filter]);

  const choose = useCallback(row => onPick(row.uom), [onPick]);

  const onKey = e => {
    if (e.key === 'Escape') { e.preventDefault(); onClose(); return; }
    if (!rows.length) return;
    if (e.key === 'ArrowDown') { e.preventDefault(); setCursor(c => Math.min(c + 1, rows.length - 1)); }
    else if (e.key === 'ArrowUp') { e.preventDefault(); setCursor(c => Math.max(c - 1, 0)); }
    else if (e.key === 'Enter') { e.preventDefault(); choose(rows[cursor]); }
  };

  useEffect(() => {
    bodyRef.current?.querySelector('tr.cur')?.scrollIntoView({ block: 'nearest' });
  }, [cursor]);

  return (
    <div className="modalwrap" onMouseDown={e => { if (e.target === e.currentTarget) onClose(); }}>
      <div className="modal lov uomlovmodal" role="dialog" aria-label="Select a unit" onKeyDown={onKey}>
        <h3>Select unit</h3>
        {invalid
          ? <p className="modalsub">
              <b>“{invalid}”</b> is not a unit this system knows, so it cannot be saved.
              Pick one below — metres are <b>MTS</b>.
            </p>
          : <p className="modalsub">
              The units the system knows. Length units first — a price list is quoted per metre.
            </p>}

        <div className="lovbar">
          <input
            ref={inputRef}
            value={filter}
            placeholder="Filter units…"
            onChange={e => setFilter(e.target.value)}
          />
          <span className="spacer" />
          <span className="lovcount">{loading ? 'loading…' : `${rows.length} units`}</span>
        </div>

        {error && <div className="modalerr" role="alert">{error}</div>}

        <div className="lovgrid uomlov" ref={bodyRef}>
          <table>
            <thead>
              <tr><th>Unit</th><th>Name</th><th>Class</th></tr>
            </thead>
            <tbody>
              {rows.map((u, i) => (
                <tr
                  key={u.uom_id}
                  className={i === cursor ? 'cur' : ''}
                  onMouseEnter={() => setCursor(i)}
                  onClick={() => choose(u)}
                  onDoubleClick={() => choose(u)}
                >
                  <td className="mono">{u.uom}</td>
                  <td>{u.uom_name || '—'}</td>
                  <td className="dim">{u.is_length ? 'length' : ''}</td>
                </tr>
              ))}
              {!loading && rows.length === 0 && (
                <tr><td colSpan={3} className="lovempty">No unit matches “{filter}”.</td></tr>
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
