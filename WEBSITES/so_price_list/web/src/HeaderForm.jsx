import { useState, useEffect, useRef, useCallback } from 'react';
import { api } from './api';
import CustomerField from './CustomerField.jsx';

const asDate = v => (v ? String(v).slice(0, 10) : '');

/**
 * Create or edit a price list header.
 *
 * Only list_name is required - a list is useful the moment it has a name, and
 * the imported 53 show that customer, dates and terms are often unknown at the
 * start. Everything else can be filled in later.
 */
export default function HeaderForm({ header, onSaved, onClose, onDeleted }) {
  const isNew = !header;
  const [f, setF] = useState({
    list_name: header?.list_name || '',
    list_desc: header?.list_desc || '',
    customer_id: header?.customer_id || null,
    customer_name: header?.customer_name || '',
    custcd: '',
    customer_excel: header?.customer_excel || '',
    valid_from: asDate(header?.valid_from),
    valid_to: asDate(header?.valid_to),
    terms: header?.terms || '',
    quote_ref: header?.quote_ref || '',
    notes: header?.notes || ''
  });
  const [nameState, setNameState] = useState({ checking: false, ok: true, message: '' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const nameRef = useRef(null);

  useEffect(() => { nameRef.current?.focus(); }, []);

  const set = (k, v) => setF(prev => ({ ...prev, [k]: v }));

  /* list_name has a filtered UNIQUE index, so check it before the user has
     filled in everything else and hits a database error at the end. */
  useEffect(() => {
    const name = f.list_name.trim();
    if (!name) { setNameState({ checking: false, ok: false, message: '' }); return; }
    let alive = true;
    setNameState(s => ({ ...s, checking: true }));
    const t = setTimeout(() => {
      api.validateName({ list_name: name, header_id: header?.so_price_list_header_id })
        .then(r => alive && setNameState({
          checking: false, ok: !!r.is_valid, message: r.message || ''
        }))
        .catch(() => alive && setNameState({ checking: false, ok: true, message: '' }));
    }, 350);
    return () => { alive = false; clearTimeout(t); };
  }, [f.list_name, header]);

  const save = useCallback(async () => {
    const name = f.list_name.trim();
    if (!name) { setError('Give the price list a name.'); return; }
    if (f.valid_from && f.valid_to && f.valid_to < f.valid_from) {
      setError('Valid to cannot be earlier than valid from.'); return;
    }
    setSaving(true); setError(null);
    try {
      const res = await api.saveHeader({
        header_id: header?.so_price_list_header_id ?? null,
        list_name: name,
        list_desc: f.list_desc || null,
        customer_id: f.customer_id,
        customer_excel: f.customer_excel || null,
        valid_from: f.valid_from || null,
        valid_to: f.valid_to || null,
        terms: f.terms || null,
        quote_ref: f.quote_ref || null,
        notes: f.notes || null
      });
      onSaved(Number(res.so_price_list_header_id), isNew);
    } catch (err) {
      setError(err.message);
    } finally { setSaving(false); }
  }, [f, header, isNew, onSaved]);

  const remove = useCallback(async () => {
    setSaving(true); setError(null);
    try {
      const res = await api.deleteList(header.so_price_list_header_id);
      onDeleted(header.so_price_list_header_id, res.deleted_line_count);
    } catch (err) { setError(err.message); setSaving(false); }
  }, [header, onDeleted]);

  const nameBad = f.list_name.trim() && !nameState.ok;

  return (
    <div className="modalwrap" onMouseDown={e => { if (e.target === e.currentTarget) onClose(); }}>
      <div className="modal" role="dialog" aria-label={isNew ? 'New price list' : 'Edit price list'}
           onKeyDown={e => { if (e.key === 'Escape') onClose(); }}>
        <h3>{isNew ? 'New price list' : 'Edit price list'}</h3>
        <p className="modalsub">
          {isNew
            ? 'Only the name is required — customer, dates and terms can follow later.'
            : <>Header <span className="mono">#{header.so_price_list_header_id}</span> · {header.line_count} price lines</>}
        </p>

        {error && <div className="modalerr" role="alert">{error}</div>}

        <label className="fld">
          <span className="fldk">List name <em>required</em></span>
          <input
            ref={nameRef} value={f.list_name} maxLength={60}
            onChange={e => set('list_name', e.target.value)}
            className={nameBad ? 'bad' : ''}
            placeholder="e.g. ANITA 2027"
          />
          {nameBad && <span className="fldmsg bad">{nameState.message}</span>}
          {!nameBad && f.list_name.trim() && !nameState.checking &&
            <span className="fldmsg ok">Name is free</span>}
        </label>

        <label className="fld">
          <span className="fldk">Description</span>
          <input value={f.list_desc} maxLength={400}
                 onChange={e => set('list_desc', e.target.value)}
                 placeholder="What this list covers" />
        </label>

        <div className="fld">
          <span className="fldk">Customer</span>
          <CustomerField
            customerId={f.customer_id}
            customerName={f.customer_name}
            custcd={f.custcd}
            onPick={c => {
              set('customer_id', c.customer_id);
              set('customer_name', c.customer_name);
              set('custcd', (c.custcd || '').trim());
            }}
            onClear={() => {
              set('customer_id', null); set('customer_name', ''); set('custcd', '');
            }}
          />
          {f.customer_id == null &&
            <span className="fldmsg">Optional — a list works without one.</span>}
        </div>

        <div className="fldrow">
          <label className="fld">
            <span className="fldk">Valid from</span>
            <input type="date" value={f.valid_from} onChange={e => set('valid_from', e.target.value)} />
          </label>
          <label className="fld">
            <span className="fldk">Valid to</span>
            <input type="date" value={f.valid_to} onChange={e => set('valid_to', e.target.value)} />
          </label>
        </div>
        <p className="fldmsg">Leave both empty and the list is always current.</p>

        <div className="fldrow">
          <label className="fld">
            <span className="fldk">Terms</span>
            <input value={f.terms} maxLength={60} onChange={e => set('terms', e.target.value)}
                   placeholder="e.g. FOB Bangkok" />
          </label>
          <label className="fld">
            <span className="fldk">Quote ref</span>
            <input value={f.quote_ref} maxLength={200} onChange={e => set('quote_ref', e.target.value)} />
          </label>
        </div>

        <div className="modalactions">
          <button className="primary" onClick={save} disabled={saving || !f.list_name.trim() || nameBad}>
            {saving ? 'Saving…' : isNew ? 'Create price list' : 'Save changes'}
          </button>
          <button className="ghost" onClick={onClose} disabled={saving}>Cancel</button>

          {!isNew && (
            <span className="modaldel">
              {confirmDelete ? (
                <>
                  <span className="delwarn">Delete this list and its {header.line_count} lines?</span>
                  <button className="danger" onClick={remove} disabled={saving}>Delete</button>
                  <button className="ghost" onClick={() => setConfirmDelete(false)} disabled={saving}>Keep</button>
                </>
              ) : (
                <button className="ghost" onClick={() => setConfirmDelete(true)} disabled={saving}>
                  Delete list…
                </button>
              )}
            </span>
          )}
        </div>
      </div>

    </div>
  );
}
