import { useState, useEffect, useCallback } from 'react';
import { api } from './api';
import CustomerLov from './CustomerLov.jsx';

/**
 * The customers a price list is quoted to.
 *
 * One list often goes to several customers, which the header's single
 * customer_id could never say. This is the whole of it: who is on, add one,
 * take one off.
 *
 * Every call returns the full list afterwards, so what is on screen is what
 * the database holds rather than a local guess kept in step by hand.
 */
export default function CustomerAssign({ headerId, listName, onClose, onChanged }) {
  const [rows, setRows]       = useState([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy]       = useState(false);
  const [error, setError]     = useState(null);
  const [picking, setPicking] = useState(false);

  const load = useCallback(() => {
    setLoading(true);
    api.listAssignedCustomers(headerId)
      .then(r => { setRows(r); setLoading(false); })
      .catch(err => { setError(err.message); setLoading(false); });
  }, [headerId]);

  useEffect(load, [load]);

  const add = useCallback(async picked => {
    setPicking(false);
    if (!picked?.customer_id) return;
    /* Already on the list: say so rather than sending a call that changes
       nothing. The database tolerates it either way. */
    if (rows.some(r => r.customer_id === picked.customer_id)) {
      setError(`${picked.customer_name} is already assigned to this list.`);
      return;
    }
    setBusy(true); setError(null);
    try {
      setRows(await api.assignCustomer(headerId, { customer_id: picked.customer_id }));
      onChanged?.();
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [headerId, rows, onChanged]);

  const remove = useCallback(async row => {
    setBusy(true); setError(null);
    try {
      setRows(await api.unassignCustomer(headerId, row.customer_id));
      onChanged?.();
    } catch (err) { setError(err.message); } finally { setBusy(false); }
  }, [headerId, onChanged]);

  return (
    <>
      <div className="modalwrap" onMouseDown={e => { if (e.target === e.currentTarget) onClose(); }}>
        <div className="modal assignmodal" role="dialog" aria-label="Customers on this price list">
          <h3>Customers</h3>
          <p className="modalsub">
            Who <b>{listName}</b> is quoted to. A list can go to as many customers as it needs.
          </p>

          {error && <div className="modalerr" role="alert">{error}</div>}

          <div className="lovgrid assigngrid">
            <table>
              <thead>
                <tr>
                  <th>Code</th>
                  <th>Customer</th>
                  <th>Ctry</th>
                  <th>Assigned by</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {rows.map(r => (
                  <tr key={r.customer_id}>
                    <td className="mono">{r.custcd || '—'}</td>
                    <td>{r.customer_name || <span className="nil">customer {r.customer_id}</span>}</td>
                    <td className="mono">{r.ctry || ''}</td>
                    {/* MIGRATE marks the one carried over from the header's
                        own customer_id when this table was introduced. */}
                    <td className="dim">{r.created_by === 'MIGRATE' ? 'from import' : (r.created_by || '')}</td>
                    <td>
                      <button className="ghost danger" disabled={busy}
                              onClick={() => remove(r)}>Remove</button>
                    </td>
                  </tr>
                ))}
                {!loading && rows.length === 0 && (
                  <tr>
                    <td colSpan={5} className="lovempty">
                      Nobody is assigned yet. <b>Add customer</b> puts someone on this list.
                    </td>
                  </tr>
                )}
                {loading && <tr><td colSpan={5} className="lovempty">Loading…</td></tr>}
              </tbody>
            </table>
          </div>

          <div className="modalactions">
            <button className="primary" disabled={busy} onClick={() => { setError(null); setPicking(true); }}>
              ＋ Add customer
            </button>
            <button className="ghost" onClick={onClose}>Close</button>
            <span className="spacer" />
            <span className="lovhint">
              {rows.length} assigned
            </span>
          </div>
        </div>
      </div>

      {/* The same picker the header uses, so there is one way to find a
          customer in this app rather than two that drift apart. */}
      {picking && (
        <CustomerLov onPick={add} onClose={() => setPicking(false)} />
      )}
    </>
  );
}
