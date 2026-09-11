'use strict';

const sql = require('mssql');

const config = {
  server:   process.env.DB_SERVER   || '172.16.3.10',
  database: process.env.DB_NAME     || 'gemmasoft',
  user:     process.env.DB_USER     || 'sa',
  password: process.env.DB_PASSWORD || 'sql@min',
  port:     Number(process.env.DB_PORT || 1433),
  options: {
    // SQL Server 2014 predates the certificate defaults the driver expects.
    encrypt: false,
    trustServerCertificate: true,
    enableArithAbort: true
  },
  pool: { max: 10, min: 0, idleTimeoutMillis: 30000 },
  requestTimeout: 60000,
  connectionTimeout: 30000
};

let poolPromise = null;

function getPool() {
  if (!poolPromise) {
    poolPromise = new sql.ConnectionPool(config).connect().catch(err => {
      poolPromise = null;           // let the next request retry a dead pool
      throw err;
    });
  }
  return poolPromise;
}

// The SO_PRICE_LIST_PKG procedures live in schema SO; the tables stay in dbo.
const PROC_SCHEMA = process.env.DB_PROC_SCHEMA || 'SO';

/**
 * Call a stored procedure. Params are [name, sqlType, value] triples.
 * Names are passed unqualified and schema-qualified here, so moving the
 * package between schemas is a one-line change.
 *
 * article is always bound as NVarChar: 487 of the 4,468 price lines carry
 * codes like '255484AA/11', so binding it as an integer rejects them outright.
 */
async function callProc(procName, params = []) {
  const pool = await getPool();
  const request = pool.request();
  for (const [name, type, value] of params) {
    request.input(name, type, value === undefined ? null : value);
  }
  const result = await request.execute(`[${PROC_SCHEMA}].[${procName}]`);
  return result.recordset || [];
}

module.exports = { sql, getPool, callProc, config, PROC_SCHEMA };
