'use strict';

const express = require('express');
const { sql, callProc } = require('../db');

const router = express.Router();

/* Every proc takes @logempcd last. Until real auth is wired in, the caller
   supplies it via header or query and it lands in created_by / updated_by. */
const who = req =>
  (req.get('X-Emp-Cd') || req.query.logempcd || req.body?.logempcd || '').slice(0, 15);

const int  = v => (v === undefined || v === null || v === '' ? null : Number(v));
const str  = v => (v === undefined || v === null || v === '' ? null : String(v));
const date = v => (v === undefined || v === null || v === '' ? null : new Date(v));
const num  = v => (v === undefined || v === null || v === '' ? null : Number(v));

/* Wrap async handlers so a rejected promise becomes a normal error response
   instead of an unhandled rejection that kills the process. */
const wrap = fn => (req, res, next) => Promise.resolve(fn(req, res, next)).catch(next);

/* ---------------------------------------------------------------- reads --- */

// GET /price_list  - headers for the picker
router.get('/', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_select_price_list', [
    ['so_price_list_header_id', sql.BigInt,        int(req.query.header_id)],
    ['customer_id',             sql.BigInt,        int(req.query.customer_id)],
    ['search',                  sql.NVarChar(100), str(req.query.search)],
    ['as_of',                   sql.Date,          date(req.query.as_of)],
    ['logempcd',                sql.VarChar(15),   who(req)]
  ]);
  res.json(rows);
}));

// GET /price_list/price - THE order-entry lookup.
// Declared before /:id so "price" is not read as an id.
router.get('/price', wrap(async (req, res) => {
  if (!req.query.header_id || !req.query.article) {
    return res.status(400).json({ error: 'header_id and article are required.' });
  }
  const rows = await callProc('P_SO_PRICE_LIST_PKG_get_price', [
    ['so_price_list_header_id', sql.BigInt,       int(req.query.header_id)],
    ['article',                 sql.NVarChar(30), str(req.query.article)],
    ['color_tier',              sql.NVarChar(30), str(req.query.color_tier)],
    ['qty',                     sql.Int,          int(req.query.qty)],
    ['qty_unit',                sql.Char(2),      str(req.query.qty_unit) || 'M'],
    ['currency',                sql.Char(3),      str(req.query.currency)],
    ['as_of',                   sql.Date,         date(req.query.as_of)],
    ['logempcd',                sql.VarChar(15),  who(req)]
  ]);

  // The whole point of the design: 1 line resolves itself, 2+ need a human.
  res.json({
    match_count: rows.length,
    resolved: rows.length === 1 ? rows[0] : null,
    needs_selection: rows.length > 1,
    matches: rows
  });
}));

// GET /price_list/customer - customer LOV
router.get('/customer', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_select_customer', [
    ['search',      sql.NVarChar(100), str(req.query.search)],
    ['parent_only', sql.Bit,           req.query.parent_only === '1' ? 1 : 0],
    ['top_n',       sql.Int,           int(req.query.top_n) || 50],
    ['logempcd',    sql.VarChar(15),   who(req)]
  ]);
  res.json(rows);
}));

// GET /price_list/color_tier - tier LOV
router.get('/color_tier', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_select_color_tier', [
    ['so_price_list_header_id', sql.BigInt,      int(req.query.header_id)],
    ['logempcd',                sql.VarChar(15), who(req)]
  ]);
  res.json(rows);
}));

// GET /price_list/:id/detail - the lines of one list
router.get('/:id/detail', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_select_price_list_detail', [
    ['so_price_list_header_id', sql.BigInt,        int(req.params.id)],
    ['article',                 sql.NVarChar(30),  str(req.query.article)],
    ['search',                  sql.NVarChar(100), str(req.query.search)],
    ['conflicts_only',          sql.Bit,           req.query.conflicts_only === '1' ? 1 : 0],
    ['logempcd',                sql.VarChar(15),   who(req)]
  ]);
  res.json(rows);
}));

/* --------------------------------------------------------------- writes --- */

// POST /price_list/validate_name
router.post('/validate_name', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_validate_price_list_name', [
    ['list_name',               sql.NVarChar(60), str(req.body.list_name)],
    ['so_price_list_header_id', sql.BigInt,       int(req.body.header_id)],
    ['logempcd',                sql.VarChar(15),  who(req)]
  ]);
  res.json(rows[0] || { is_valid: false, message: 'No response from validation.' });
}));

// POST /price_list  - upsert header (omit header_id to insert)
router.post('/', wrap(async (req, res) => {
  const b = req.body;
  const rows = await callProc('P_SO_PRICE_LIST_PKG_update_price_list', [
    ['so_price_list_header_id', sql.BigInt,        int(b.header_id)],
    ['list_name',               sql.NVarChar(60),  str(b.list_name)],
    ['list_desc',               sql.NVarChar(400), str(b.list_desc)],
    ['customer_id',             sql.BigInt,        int(b.customer_id)],
    ['customer_excel',          sql.NVarChar(120), str(b.customer_excel)],
    ['list_date',               sql.Date,          date(b.list_date)],
    ['valid_from',              sql.Date,          date(b.valid_from)],
    ['valid_to',                sql.Date,          date(b.valid_to)],
    ['terms',                   sql.NVarChar(60),  str(b.terms)],
    ['quote_ref',               sql.NVarChar(200), str(b.quote_ref)],
    ['sonoid',                  sql.NVarChar(30),  str(b.sonoid)],
    ['so_line_id',              sql.BigInt,        int(b.so_line_id)],
    ['notes',                   sql.NVarChar(500), str(b.notes)],
    ['logempcd',                sql.VarChar(15),   who(req)]
  ]);
  res.json(rows[0] || {});
}));

// POST /price_list/detail  - upsert one price line
router.post('/detail', wrap(async (req, res) => {
  const b = req.body;
  const rows = await callProc('P_SO_PRICE_LIST_PKG_update_price_list_detail', [
    ['so_price_list_detail_id', sql.BigInt,        int(b.detail_id)],
    ['so_price_list_header_id', sql.BigInt,        int(b.header_id)],
    ['article',                 sql.NVarChar(30),  str(b.article)],
    ['design_no',               sql.Char(20),      str(b.design_no)],
    ['article_variant',         sql.NVarChar(20),  str(b.article_variant)],
    ['fabric_name',             sql.NVarChar(120), str(b.fabric_name)],
    ['composition',             sql.NVarChar(200), str(b.composition)],
    ['full_width_cm',           sql.NVarChar(30),  str(b.full_width_cm)],
    ['usable_width_cm',         sql.NVarChar(30),  str(b.usable_width_cm)],
    ['weight_gsm',              sql.NVarChar(30),  str(b.weight_gsm)],
    ['moq',                     sql.NVarChar(30),  str(b.moq)],
    ['qty_min',                 sql.Int,           int(b.qty_min)],
    ['qty_max',                 sql.Int,           int(b.qty_max)],
    ['qty_unit',                sql.Char(2),       str(b.qty_unit) || 'M'],
    ['color_tier',              sql.NVarChar(30),  str(b.color_tier)],
    ['currency',                sql.Char(3),       str(b.currency)],
    ['price',                   sql.Decimal(18,4), num(b.price)],
    ['line_no',                 sql.Int,           int(b.line_no)],
    ['notes',                   sql.NVarChar(500), str(b.notes)],
    ['logempcd',                sql.VarChar(15),   who(req)]
  ]);
  res.json(rows[0] || {});
}));

// POST /price_list/:id/grid_shape - which tier/currency columns this list shows
router.post('/:id/grid_shape', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_update_price_list_grid_shape', [
    ['so_price_list_header_id', sql.BigInt,        int(req.params.id)],
    ['tier_set',                sql.NVarChar(200), str(req.body.tier_set)],
    ['currency_set',            sql.NVarChar(20),  str(req.body.currency_set)],
    ['logempcd',                sql.VarChar(15),   who(req)]
  ]);
  res.json(rows[0] || {});
}));

// POST /price_list/:id/row - row-level edit, applied to every line in the row.
// One grid row stands for up to 12 detail lines; moving the article or the qty
// band has to move all of them together or the row splits.
router.post('/:id/row', wrap(async (req, res) => {
  const b = req.body;
  const rows = await callProc('P_SO_PRICE_LIST_PKG_update_price_list_row', [
    ['so_price_list_header_id', sql.BigInt,        int(req.params.id)],
    ['article',                 sql.NVarChar(30),  str(b.article)],
    ['article_variant',         sql.NVarChar(20),  str(b.article_variant)],
    ['qty_min',                 sql.Int,           int(b.qty_min)],
    ['qty_max',                 sql.Int,           int(b.qty_max)],
    ['qty_unit',                sql.Char(2),       str(b.qty_unit) || 'M'],
    ['new_article',             sql.NVarChar(30),  str(b.new_article)],
    ['new_article_variant',     sql.NVarChar(20),  str(b.new_article_variant)],
    ['new_qty_min',             sql.Int,           int(b.new_qty_min)],
    ['new_qty_max',             sql.Int,           int(b.new_qty_max)],
    ['clear_qty_max',           sql.Bit,           b.clear_qty_max ? 1 : 0],
    ['new_qty_unit',            sql.Char(2),       str(b.new_qty_unit)],
    ['fabric_name',             sql.NVarChar(120), str(b.fabric_name)],
    ['composition',             sql.NVarChar(200), str(b.composition)],
    ['full_width_cm',           sql.NVarChar(30),  str(b.full_width_cm)],
    ['usable_width_cm',         sql.NVarChar(30),  str(b.usable_width_cm)],
    ['weight_gsm',              sql.NVarChar(30),  str(b.weight_gsm)],
    ['moq',                     sql.NVarChar(30),  str(b.moq)],
    ['design_no',               sql.Char(20),      str(b.design_no)],
    ['logempcd',                sql.VarChar(15),   who(req)]
  ]);
  res.json(rows[0] || {});
}));

// POST /price_list/:id/copy
router.post('/:id/copy', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_copy_price_list', [
    ['source_header_id', sql.BigInt,       int(req.params.id)],
    ['list_name',        sql.NVarChar(60), str(req.body.list_name)],
    ['customer_id',      sql.BigInt,       int(req.body.customer_id)],
    ['valid_from',       sql.Date,         date(req.body.valid_from)],
    ['valid_to',         sql.Date,         date(req.body.valid_to)],
    ['logempcd',         sql.VarChar(15),  who(req)]
  ]);
  res.json(rows[0] || {});
}));

// DELETE /price_list/detail/:id  - before /:id, same reason as /price
router.delete('/detail/:id', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_delete_price_list_detail', [
    ['so_price_list_detail_id', sql.BigInt,      int(req.params.id)],
    ['logempcd',                sql.VarChar(15), who(req)]
  ]);
  res.json(rows[0] || {});
}));

// DELETE /price_list/:id
router.delete('/:id', wrap(async (req, res) => {
  const rows = await callProc('P_SO_PRICE_LIST_PKG_delete_price_list', [
    ['so_price_list_header_id', sql.BigInt,      int(req.params.id)],
    ['logempcd',                sql.VarChar(15), who(req)]
  ]);
  res.json(rows[0] || {});
}));

module.exports = router;
