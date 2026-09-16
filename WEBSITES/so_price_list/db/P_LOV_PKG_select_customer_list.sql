/* ============================================================================
   Customer list of values.

   Follows the house LOV convention: schema LOV, named
   P_LOV_PKG_select_<subject>_list, taking a single @p_filter_text.

   Unlike the older dbo.p_customers_lov this carries sales-order activity
   (order count and last order date). That matters here because customer names
   repeat - "Agent Provocateur" is three records, "Unichela Pvt Ltd." is three -
   and order history is the only reliable way to tell which record is the one
   actually traded on.

   No blank leading row: the existing so_list LOV unions one in for WinForms
   combo binding, but this LOV is a searchable dialog where an empty row would
   just be a dead entry.
   ============================================================================ */

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'LOV')
    EXEC('CREATE SCHEMA LOV');
GO

IF OBJECT_ID('LOV.P_LOV_PKG_select_customer_list','P') IS NOT NULL
    DROP PROCEDURE LOV.P_LOV_PKG_select_customer_list;
GO
-- =============================================
-- Author		 : Claude
-- Create date	 : 10/09/2026
-- Last Modified :
-- Description	: This procedure is used for presenting a list of customers to search and filter.
-- Execute1		: LOV.[P_LOV_PKG_select_customer_list] 'anita'
-- Execute2		: LOV.[P_LOV_PKG_select_customer_list] ''
-- =============================================
CREATE PROCEDURE LOV.[P_LOV_PKG_select_customer_list]
	@p_filter_text nvarchar(100) = '',
	@p_active_only bit           = 0,
	@p_top_n       int           = 200
AS
BEGIN
	SET NOCOUNT ON;

	IF @p_top_n IS NULL OR @p_top_n <= 0 SET @p_top_n = 200;

	DECLARE @f nvarchar(102) = '%' + LTRIM(RTRIM(ISNULL(@p_filter_text,''))) + '%';

	SELECT TOP (@p_top_n)
			cust.customer_id,
			cust.custcd,
			RTRIM(cust.name)  AS name,
			RTRIM(ISNULL(cust.name2,'')) AS name2,
			cust.ctry,
			ISNULL(cust.city,'')     AS city,
			ISNULL(cust.district,'') AS district,
			cust.active,
			cust.parent_customer_id,
			ISNULL(cust.parent_customer_flag,'') AS parent_customer_flag,
			ISNULL(so.so_orders, 0)  AS so_orders,
			so.last_so_date
	FROM	dbo.customers cust
	LEFT JOIN (
			SELECT custcd, COUNT(*) AS so_orders, MAX(sodt) AS last_so_date
			FROM   dbo.so
			GROUP BY custcd
	) so ON so.custcd = cust.custcd
	WHERE  (NULLIF(LTRIM(RTRIM(@p_filter_text)),'') IS NULL
	        OR cust.name   LIKE @f
	        OR cust.name2  LIKE @f
	        OR cust.custcd LIKE @f
	        OR cust.city   LIKE @f)
	  AND  (@p_active_only = 0 OR cust.active = 1)
	/* the record actually traded on is nearly always the one wanted */
	ORDER BY ISNULL(so.so_orders,0) DESC, cust.name;
END
GO

PRINT 'LOV.P_LOV_PKG_select_customer_list created';
GO
