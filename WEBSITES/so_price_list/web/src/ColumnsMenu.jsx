import { useEffect, useLayoutEffect, useRef, useState } from 'react';
import { TIER_ORDER } from './api';

/* Distance from the button, and the smallest gap left against the window edge. */
const GAP = 6;

/**
 * Which colour tiers and currencies this list's grid shows.
 *
 * These are a property of the LIST, not of the rows: a new list has no rows to
 * infer them from, and a tier you intend to price but have not filled in yet
 * still needs a column to type into. Unticking a tier only hides the column -
 * the price lines underneath are untouched, so it is always reversible.
 */
export default function ColumnsMenu({ tiers, currencies, allTiers,
                                      tiersInData = [], currenciesInData = [],
                                      anchor, onApply, onClose }) {
  const ref = useRef(null);
  const [t, setT] = useState(new Set(tiers));
  const [c, setC] = useState(new Set(currencies));
  const [custom, setCustom] = useState('');
  /* null until measured. The panel used to be pinned to the top-right corner
     of the window, which put it nowhere near the button that opens it. */
  const [pos, setPos] = useState(null);

  /* Layout, not paint: this runs before the browser draws, so the panel is
     never seen in the wrong place first. */
  useLayoutEffect(() => {
    const el = ref.current;
    const btn = anchor?.current;
    if (!el) return;

    const place = () => {
      if (!btn) return;
      const b = btn.getBoundingClientRect();
      const w = el.offsetWidth, h = el.offsetHeight;

      /* Left edges aligned with the button, pulled back inside the window if
         the panel would hang off the right. */
      const left = Math.max(GAP, Math.min(b.left, window.innerWidth - w - GAP));
      /* Below the button, unless there is no room down there - then above it. */
      let top = b.bottom + GAP;
      if (top + h > window.innerHeight - GAP) top = Math.max(GAP, b.top - GAP - h);

      setPos(p => (p && p.top === top && p.left === left ? p : { top, left }));
    };

    place();
    /* Ticking a tier changes the panel's height, which can change whether it
       still fits below the button. */
    const ro = typeof ResizeObserver === 'function' ? new ResizeObserver(place) : null;
    ro?.observe(el);
    window.addEventListener('resize', place);
    window.addEventListener('scroll', place, true);
    return () => {
      ro?.disconnect();
      window.removeEventListener('resize', place);
      window.removeEventListener('scroll', place, true);
    };
  }, [anchor]);

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

  /* A tier added by hand has no checkbox of its own until it is applied,
     because "known" is built from the fixed order plus what exists. Include
     whatever is currently ticked and the new one appears at once. */
  const known = [...new Set([...TIER_ORDER, ...allTiers, ...tiers, ...t])];

  /* Unticking only ever hid a column - the prices stayed in the table. That is
     still true, but it does not survive contact with a reviewer: a column of
     prices vanishing reads as the prices being gone, and the way back is a
     menu they have just closed. So a tier or currency THIS LIST HAS PRICES IN
     cannot be turned off. Empty ones still can, which is the case the menu is
     actually for. */
  const lockedTier = v => tiersInData.includes(v);
  const lockedCcy  = v => currenciesInData.includes(v);

  const toggle = (set, setter, v, locked) => {
    if (locked && set.has(v)) return;       // on, and holding prices: stays on
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
    <div
      className="pop colmenu"
      ref={ref}
      role="dialog"
      aria-label="Grid columns"
      /* Hidden rather than absent for the one frame before it is measured:
         it has to be in the document to have a size to measure. */
      style={pos ? { top: pos.top, left: pos.left } : { visibility: 'hidden' }}
    >
      <h4>Columns</h4>
      <p className="sub">Which colour tiers and currencies this list prices.</p>

      <div className="colgroup">
        <span className="collabel">Currency</span>
        {['USD', 'THB'].map(x => {
          const locked = lockedCcy(x) && c.has(x);
          return (
            <label key={x} className={`chk${locked ? ' locked' : ''}`}
                   title={locked ? `This list has ${x} prices. Clear them before removing the column.` : undefined}>
              <input type="checkbox" checked={c.has(x)} disabled={locked}
                     onChange={() => toggle(c, setC, x, lockedCcy(x))} />
              <span>{x}</span>
            </label>
          );
        })}
      </div>

      <div className="colgroup">
        <span className="collabel">Colour tier</span>
        {known.map(x => {
          const locked = lockedTier(x) && t.has(x);
          return (
            <label key={x} className={`chk${locked ? ' locked' : ''}`}
                   title={locked ? `This list has prices at ${x}. Clear them before removing the column.` : undefined}>
              <input type="checkbox" checked={t.has(x)} disabled={locked}
                     onChange={() => toggle(t, setT, x, lockedTier(x))} />
              <span>{x}</span>
            </label>
          );
        })}
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
        {tiersInData.length || currenciesInData.length
          ? 'Columns holding prices cannot be removed. Empty ones can.'
          : 'Hiding a tier keeps its prices — nothing is deleted.'}
      </p>
    </div>
  );
}
