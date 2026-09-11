import { useEffect, useRef, useState } from 'react';

const money = (v, ccy) => {
  const n = Number(v);
  return ccy === 'THB'
    ? n.toFixed(2).replace(/\.00$/, '')
    : n.toFixed(4).replace(/0+$/, '').replace(/\.$/, '');
};

/**
 * Shown when a cell is backed by more than one price line.
 *
 * Two different actions, deliberately kept apart:
 *   "Use this price"  - a choice for THIS order only. Nothing is written.
 *   "Retire"          - soft-deletes the losing line for good, so the cell
 *                       stops asking anyone. That is a data fix, so it is
 *                       behind a confirm.
 */
export default function ConflictPicker({
  row, currency, tier, anchor, picked, onChoose, onRetire, onClose
}) {
  const ref = useRef(null);
  const [confirming, setConfirming] = useState(null);
  const lines = row.cells[`${currency}|${tier}`] || [];
  const cellKey = `${row.key}|${currency}|${tier}`;
  const chosenId = picked[cellKey] ?? lines[0]?.so_price_list_detail_id;

  useEffect(() => {
    const onDown = e => { if (ref.current && !ref.current.contains(e.target)) onClose(); };
    const onKey  = e => { if (e.key === 'Escape') onClose(); };
    document.addEventListener('mousedown', onDown);
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('mousedown', onDown);
      document.removeEventListener('keydown', onKey);
    };
  }, [onClose]);

  // keep the popover on screen even when the cell is near an edge
  const style = {};
  if (anchor) {
    style.left = Math.max(8, Math.min(anchor.left, window.innerWidth - 300));
    style.top  = Math.min(anchor.bottom + 4, window.innerHeight - 240);
  }

  const spread = lines.length > 1
    ? Math.abs(Number(lines[0].price) - Number(lines[lines.length - 1].price))
    : 0;

  return (
    <div className="pop" ref={ref} style={style} role="dialog" aria-label="Choose a price">
      <h4>{lines.length} price lines match</h4>
      <p className="sub">
        {row.article} · {tier} · {currency} ·{' '}
        {row.qty_min}–{row.qty_max === null ? '∞' : row.qty_max} {row.qty_unit}
        {spread > 0 && <> · spread {money(spread, currency)}</>}
      </p>

      {lines.map(d => {
        const isChosen = d.so_price_list_detail_id === chosenId;
        const isConfirming = confirming === d.so_price_list_detail_id;
        return (
          <div key={d.so_price_list_detail_id}>
            <button
              className={`opt${isChosen ? ' chosen' : ''}`}
              onClick={() => onChoose(cellKey, d.so_price_list_detail_id)}
            >
              <span className="p">{money(d.price, currency)}</span>
              <span className="m">
                detail #{d.so_price_list_detail_id}
                <br />
                {d.source_row ? `sheet row ${d.source_row}` : 'entered in app'}
              </span>
              <span className="tick">✓</span>
            </button>

            {isConfirming ? (
              <div style={{ display: 'flex', gap: 6, margin: '-2px 0 8px 7px' }}>
                <button
                  onClick={() => { setConfirming(null); onRetire(d); }}
                  style={{ borderColor: 'var(--warn-rule)', color: 'var(--warn)' }}
                >
                  Retire permanently
                </button>
                <button className="ghost" onClick={() => setConfirming(null)}>Cancel</button>
              </div>
            ) : (
              lines.length > 1 && (
                <button
                  className="ghost"
                  style={{ fontSize: 10.5, margin: '-2px 0 8px 7px' }}
                  onClick={() => setConfirming(d.so_price_list_detail_id)}
                >
                  Retire this line…
                </button>
              )
            )}
          </div>
        );
      })}

      <p className="sub" style={{ margin: '2px 0 0' }}>
        Picking applies to this order. Retiring removes the line for everyone.
      </p>
    </div>
  );
}
