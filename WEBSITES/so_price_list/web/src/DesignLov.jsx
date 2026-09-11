import { useState, useEffect, useRef, useCallback } from 'react';
import { api } from './api';

/**
 * Design list of values.
 *
 * A price list often carries the STEM of a design number rather than one the
 * ERP actually holds: the list says 255699AA, dm has 255699AA/14, /15 and /16.
 * Typing the stem and tabbing out opens this instead of reporting "not found".
 *
 * A grid rather than a type-ahead, for the same reason the customer picker is
 * one: the variants differ in ways you have to see side by side. Of those three
 * 255699AA rows only /14 carries a fabric name, so composition and weight are
 * what tell the other two apart.
 */
export default function DesignLov({ initialFilter = '', onPick, onClose }) {
  const [filter, setFilter]   = useState(initialFilter);
  const [rows, setRows]       = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState(null);
  const [cursor, setCursor]   = useState(0);
  const inputRef = useRef(null);
  const bodyRef  = useRef(null);

  useEffect(() => { inputRef.current?.focus(); inputRef.current?.select(); }, []);

  useEffect(() => {
    let alive = true;
    setLoading(true);
    const t = setTimeout(() => {
      api.lovDesign({ filter, top_n: 200 })
        .then(r => { if (alive) { setRows(r); setCursor(0); setLoading(false); } })
        .catch(err => { if (alive) { setError(err.message); setLoading(false); } });
    }, filter ? 220 : 0);
    return () => { alive = false; clearTimeout(t); };
  }, [filter]);

  const choose = useCallback(row => onPick(row), [onPick]);

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
      <div className="modal lov designlovmodal" role="dialog" aria-label="Select a design" onKeyDown={onKey}>
        <h3>Select design</h3>
        <p className="modalsub">
          Closest match first — an exact number, then the ones starting with it,
          then anything carrying it. Searching the fabric name works too.
        </p>

        <div className="lovbar">
          <input
            ref={inputRef}
            value={filter}
            placeholder="Design number or fabric name…"
            onChange={e => setFilter(e.target.value)}
          />
          <span className="spacer" />
          <span className="lovcount">
            {loading ? 'searching…' : `${rows.length}${rows.length === 200 ? '+' : ''} match${rows.length === 1 ? '' : 'es'}`}
          </span>
        </div>

        {error && <div className="modalerr" role="alert">{error}</div>}

        <div className="lovgrid designlov" ref={bodyRef}>
          <table>
            <thead>
              <tr>
                <th>Design no</th>
                <th>Fabric</th>
                <th>Composition</th>
                <th>g/m²</th>
                <th>Usable W</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((d, i) => (
                <tr
                  key={d.design_no}
                  className={i === cursor ? 'cur' : ''}
                  onMouseEnter={() => setCursor(i)}
                  onClick={() => choose(d)}
                  onDoubleClick={() => choose(d)}
                >
                  <td className="mono">{d.design_no}</td>
                  {/* refdesno is empty on plenty of rows, and saying so beats an
                      empty cell that reads like a loading failure. */}
                  <td>{d.fabric_name || <span className="nil">no fabric name</span>}</td>
                  <td>{d.composition || '—'}</td>
                  <td className="mono">{d.weight_gsm || '—'}</td>
                  <td className="mono">{d.usable_width_cm || '—'}</td>
                </tr>
              ))}
              {!loading && rows.length === 0 && (
                <tr>
                  <td colSpan={5} className="lovempty">
                    No design matches “{filter}”. Close this and type the line by hand —
                    not every design in a price list is in the ERP master.
                  </td>
                </tr>
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
