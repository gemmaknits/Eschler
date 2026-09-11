import { useState, useRef, useCallback } from 'react';
import { api } from './api';
import CustomerLov from './CustomerLov.jsx';

/**
 * Customer field with the house LOV behaviour:
 *
 *   type partial text, press Enter
 *     -> exactly one match : resolved silently, no dialog
 *     -> more than one     : the LOV window opens on that filter, to choose
 *     -> none              : says so, and leaves what was typed alone
 *
 * The point is that the common case - typing enough to be unambiguous - never
 * costs a dialog, while an ambiguous one puts the deciding columns (code,
 * country, order count, last order) in front of you instead of guessing.
 */
export default function CustomerField({
  customerId, customerName, custcd, onPick, onClear, autoFocus = false, compact = false
}) {
  const [text, setText]       = useState('');
  const [busy, setBusy]       = useState(false);
  const [msg, setMsg]         = useState(null);      // {kind:'none'|'error', text}
  const [lovFilter, setLovFilter] = useState(null);  // non-null = LOV open
  const inputRef = useRef(null);

  const resolve = useCallback(async () => {
    const q = text.trim();
    if (!q) { setLovFilter(''); return; }   // empty + Enter = browse everything
    setBusy(true); setMsg(null);
    try {
      const rows = await api.lovCustomer({ filter: q, top_n: 200 });
      if (rows.length === 1) {
        const c = rows[0];
        onPick({ customer_id: c.customer_id, customer_name: c.name, custcd: (c.custcd || '').trim() });
        setText('');
      } else if (rows.length === 0) {
        setMsg({ kind: 'none', text: `No customer matches “${q}”.` });
      } else {
        setLovFilter(q);                    // ambiguous - let the user choose
      }
    } catch (err) {
      setMsg({ kind: 'error', text: err.message });
    } finally { setBusy(false); }
  }, [text, onPick]);

  const picked = customerId != null;

  return (
    <>
      <div className={`custfield${compact ? ' compact' : ''}`}>
        {picked ? (
          <>
            <span className="custname" title={customerName}>{customerName}</span>
            {custcd && <span className="mono custid">{custcd}</span>}
            <span className="mono custid">#{customerId}</span>
            <button className="ghost" onClick={() => setLovFilter('')} disabled={busy}>Change…</button>
            <button className="ghost" onClick={onClear} disabled={busy}>Clear</button>
          </>
        ) : (
          <>
            <input
              ref={inputRef}
              autoFocus={autoFocus}
              value={text}
              disabled={busy}
              placeholder="Type name or code, then Enter…"
              onChange={e => { setText(e.target.value); setMsg(null); }}
              onKeyDown={e => {
                if (e.key === 'Enter') { e.preventDefault(); e.stopPropagation(); resolve(); }
                else if (e.key === 'Escape' && text) { e.preventDefault(); setText(''); setMsg(null); }
              }}
            />
            <button className="ghost" onClick={() => setLovFilter(text.trim())} disabled={busy}
                    title="Browse the full customer list">
              {busy ? 'Looking…' : 'Browse…'}
            </button>
          </>
        )}
      </div>

      {msg && (
        <span className={`fldmsg${msg.kind === 'error' ? ' bad' : ''}`}>
          {msg.text}{msg.kind === 'none' && ' Try fewer characters, or Browse.'}
        </span>
      )}

      {lovFilter !== null && (
        <CustomerLov
          initialFilter={lovFilter}
          onPick={c => { onPick(c); setLovFilter(null); setText(''); setMsg(null); }}
          onClose={() => setLovFilter(null)}
        />
      )}
    </>
  );
}
