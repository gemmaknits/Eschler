'use strict';

const express = require('express');
const cors = require('cors');
const { getPool } = require('./db');
const priceList = require('./routes/priceList');

const app = express();
const PORT = Number(process.env.PORT || 3001);

app.use(cors());
app.use(express.json({ limit: '2mb' }));

app.use('/price_list', priceList);

app.get('/health', async (_req, res) => {
  try {
    await getPool();
    res.json({ ok: true, db: 'connected' });
  } catch (err) {
    res.status(503).json({ ok: false, db: 'unavailable', error: err.message });
  }
});

app.use((req, res) => res.status(404).json({ error: `No route for ${req.method} ${req.path}` }));

/* The procs RAISERROR with plain-language messages ("Another price list already
   uses this name."). Those are for the user, so pass them through as 400 rather
   than burying them in a 500. */
app.use((err, _req, res, _next) => {
  const validation = err.number === 50000 || /RAISERROR/i.test(err.message || '');
  if (!validation) console.error('[api]', err);
  res.status(validation ? 400 : 500).json({
    error: err.message || 'Unexpected error.'
  });
});

app.listen(PORT, () => {
  console.log(`so-price-list api listening on ${PORT} - routes under /price_list`);
});
