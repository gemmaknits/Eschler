import { useMemo, useRef, useEffect } from 'react';

/**
 * Left navigation: every price list, searchable, with the counts that matter
 * before you open one (lines, and cells that will ask the user to pick).
 *
 * Filtering is client-side: all 53 lists load once at startup, so searching
 * is instant and does not wait on the network.
 */
export default function PriceListNav({
  lists, selectedId, search, onSearch, onSelect, onNew, busy
}) {
  const listRef = useRef(null);

  const shown = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return lists;
    return lists.filter(l =>
      (l.list_name || '').toLowerCase().includes(q) ||
      (l.customer_name || '').toLowerCase().includes(q) ||
      (l.customer_excel || '').toLowerCase().includes(q) ||
      (l.list_desc || '').toLowerCase().includes(q));
  }, [lists, search]);

  // keep the open list in view when it changes from elsewhere (deep link, save)
  useEffect(() => {
    const el = listRef.current?.querySelector('.navitem.on');
    el?.scrollIntoView({ block: 'nearest' });
  }, [selectedId, shown.length]);

  const totalConflicts = lists.reduce((a, l) => a + (l.conflict_count || 0), 0);

  return (
    <nav className="nav">
      <div className="navhead">
        <button className="newbtn" onClick={onNew} disabled={busy}>
          <span aria-hidden="true">＋</span> New price list
        </button>
        <input
          type="search"
          className="navsearch"
          placeholder="Search lists…"
          value={search}
          onChange={e => onSearch(e.target.value)}
          onKeyDown={e => {
            // Enter opens the only remaining match - fast keyboard path
            if (e.key === 'Enter' && shown.length === 1) onSelect(shown[0].so_price_list_header_id);
            if (e.key === 'Escape') onSearch('');
          }}
        />
      </div>

      <div className="navlist" ref={listRef}>
        {shown.length === 0 && (
          <p className="navempty">
            No list matches “{search}”.
            <button className="ghost" onClick={() => onSearch('')}>Clear</button>
          </p>
        )}

        {shown.map(l => {
          const id = l.so_price_list_header_id;
          const expired = l.expired_flag === 'Y';
          return (
            <button
              key={id}
              className={`navitem${id === selectedId ? ' on' : ''}`}
              onClick={() => onSelect(id)}
              title={l.list_desc || l.list_name}
            >
              <span className="navname">{l.list_name}</span>
              <span className="navmeta">
                <span className="navlines">{l.line_count} lines</span>
                {l.conflict_count > 0 && (
                  <span className="navconf" title={`${l.conflict_count} cells need a pick`}>
                    {l.conflict_count}
                  </span>
                )}
                {expired && <span className="navexp" title="Past its valid-to date">expired</span>}
              </span>
              {l.customer_name
                ? <span className="navcust">{l.customer_name}</span>
                : <span className="navcust dim">not mapped to a customer</span>}
            </button>
          );
        })}
      </div>

      <div className="navfoot">
        <span><b>{shown.length}</b>{shown.length !== lists.length && <> of <b>{lists.length}</b></>} lists</span>
        {totalConflicts > 0 && <span className="navfootwarn">{totalConflicts} conflicts</span>}
      </div>
    </nav>
  );
}
