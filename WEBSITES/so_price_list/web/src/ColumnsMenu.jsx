import { useEffect, useRef, useState } from 'react';
import { TIER_ORDER } from './api';

/**
 * Which colour tiers and currencies this list's grid shows.
 *
 * These are a property of the LIST, not of the rows: a new list has no rows to
 * infer them from, and a tier you intend to price but have not filled in yet
 * still needs a column to type into. Unticking a tier only hides the column -
 * the price lines underneath are untouched, so it is always reversible.
 */
export default function ColumnsMenu({ tiers, currencies, allTiers, onApply, onClose }) {
  const ref = useRef(null);
  const [t, setT] = useState(new Set(tiers));
  const [c, setC] = useState(new Set(currencies));
  const [custom, setCustom] = useState('');

  useEffect(() => {
    const onDown = e => { if (ref.current && !ref.current.contains(e.target)) onClose(); };
    const onKey = e => { if (e.key === 'Escape') onClose(); };
    document.addEventListener('mousedown', onDown);
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('mousedown', onDown);
      document.removeEventListener('keydown', onKey);
    };
  }, [onClose]);

  const known = [...new Set([...TIER_ORDER, ...allTiers, ...tiers])];
  const toggle = (set, setter, v) => {
    const next = new Set(set);
    next.has(v) ? next.delete(v) : next.add(v);
    setter(next);
  };

  const addCustom = () => {
    const v = custom.trim();
    if (!v) return;
    setT(prev => new Set(prev).add(v));
    setCustom('');
  };

  const apply = () => {
    if (!t.size) return;                 // a grid with no price columns is useless
    if (!c.size) return;
    onApply(
      [...t].sort((a, b) => {
        const ia = TIER_ORDER.indexOf(a), ib = TIER_ORDER.indexOf(b);
        return (ia < 0 ? 99 : ia) - (ib < 0 ? 99 : ib) || a.localeCompare(b);
      }),
      ['USD', 'THB'].filter(x => c.has(x))
    );
  };

  return (
    <div className="pop colmenu" ref={ref} role="dialog" aria-label="Grid columns">
      <h4>Columns</h4>
      <p className="sub">Which colour tiers and currencies this list prices.</p>

      <div className="colgroup">
        <span className="collabel">Currency</span>
        {['USD', 'THB'].map(x => (
          <label key={x} className="chk">
            <input type="checkbox" checked={c.has(x)} onChange={() => toggle(c, setC, x)} />
            <span>{x}</span>
          </label>
        ))}
      </div>

      <div className="colgroup">
        <span className="collabel">Colour tier</span>
        {known.map(x => (
          <label key={x} className="chk">
            <input type="checkbox" checked={t.has(x)} onChange={() => toggle(t, setT, x)} />
            <span>{x}</span>
          </label>
        ))}
      </div>

      <div className="colgroup">
        <span className="collabel">Add a tier</span>
        <input
          value={custom}
          placeholder="e.g. Neon"
          onChange={e => setCustom(e.target.value)}
          onKeyDown={e => { if (e.key === 'Enter') { e.preventDefault(); addCustom(); } }}
          style={{ width: 110 }}
        />
        <button className="ghost" onClick={addCustom}>Add</button>
      </div>

      <div className="colactions">
        <button onClick={apply} disabled={!t.size || !c.size}>Apply</button>
        <button className="ghost" onClick={onClose}>Cancel</button>
      </div>
      <p className="sub" style={{ margin: '6px 0 0' }}>
        Hiding a tier keeps its prices — nothing is deleted.
      </p>
    </div>
  );
}
