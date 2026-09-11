/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 8 : customers assigned to a price list

   A price list is quoted to more than one customer. The header carries a
   single customer_id, matched during the import, which cannot say that - so
   the assignments live in their own table and the header's own column is left
   alone rather than migrated away underneath anything that still reads it.

   dbo.customers belongs to the ERP: read, never written. It is Thai_CI_AI
   like the rest of them, so comparisons are COLLATE DATABASE_DEFAULT.

   Removing an assignment is a HARD delete. An assignment carries nothing of
   its own - no prices, no history - so a soft one would only leave rows to
   filter out for ever. What was assigned and by whom stays in created_by.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.so_price_list_customers','U') IS NULL
BEGIN
    CREATE TABLE SO.so_price_list_customers (
        so_price_list_customer_id bigint IDENTITY(1,1)
            CONSTRAINT PK_so_price_list_customers PRIMARY KEY,
        so_price_list_header_id   bigint      NOT NULL,
        customer_id               bigint      NOT NULL,
        creation_date             datetime2(0) NOT NULL
            CONSTRAINT DF_splc_creation_date DEFAULT (SYSDATETIME()),
        created_by                varchar(15) NULL
    );

    /* One customer cannot be assigned to the same list twice. The UI hides an
       already-assigned customer, but the rule belongs here, not there. */
    CREATE UNIQUE INDEX UX_splc_header_customer
        ON SO.so_price_list_customers (so_price_list_header_id, customer_id);

    CREATE NONCLUSTERED INDEX IX_splc_customer
        ON SO.so_price_list_customers (customer_id);

    PRINT 'SO.so_price_list_customers created';
END
ELSE PRINT 'SO.so_price_list_customers already exists';
GO

/* Carry over the single customer each header already has, so nothing that was
   matched during the import is lost when the UI starts reading this table. */
INSERT INTO SO.so_price_list_customers (so_price_list_header_id, customer_id, created_by)
SELECT h.so_price_list_header_id, h.customer_id, 'MIGRATE'
FROM   SO.so_price_list_header h
WHERE  h.customer_id IS NOT NULL
  AND  h.delete_mark <> 'Y'
  AND  NOT EXISTS (SELECT 1 FROM SO.so_price_list_customers c
                   WHERE c.so_price_list_header_id = h.so_price_list_header_id
                     AND c.customer_id = h.customer_id);
PRINT CONCAT('existing header customers carried over: ', @@ROWCOUNT);
GO

/* ---------------------------------------------------------------------------
   select_price_list_customer  -  who is assigned to this list.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_price_list_customer','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_price_list_customer;
GO
-- =============================================
-- Description: The customers assigned to one price list.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_price_list_customer 3, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_price_list_customer]
    @so_price_list_header_id bigint,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  c.so_price_list_customer_id,
            c.so_price_list_header_id,
            c.customer_id,
            LTRIM(RTRIM(cu.custcd)) COLLATE DATABASE_DEFAULT AS custcd,
            cu.name       COLLATE DATABASE_DEFAULT AS customer_name,
            cu.ctry       COLLATE DATABASE_DEFAULT AS ctry,
            cu.city       COLLATE DATABASE_DEFAULT AS city,
            c.creation_date,
            c.created_by
    FROM    SO.so_price_list_customers c
    LEFT JOIN dbo.customers cu ON cu.customer_id = c.customer_id
    WHERE   c.so_price_list_header_id = @so_price_list_header_id
    ORDER BY cu.name, c.customer_id;
END
GO

/* ---------------------------------------------------------------------------
   assign_price_list_customer  -  add one.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_assign_price_list_customer','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_assign_price_list_customer;
GO
-- =============================================
-- Description: Assign a customer to a price list. Assigning twice is not an
--              error - it reports the assignment that is already there.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_assign_price_list_customer 3, 173, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_assign_price_list_customer]
    @so_price_list_header_id bigint,
    @customer_id             bigint,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    IF @customer_id IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.customers
                                           WHERE customer_id = @customer_id)
    BEGIN
        RAISERROR('That customer does not exist.', 16, 1);
        RETURN;
    END

    /* Assigning someone who is already assigned is not a mistake worth an
       error - two people can reach for it at once. Say it is done. */
    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_customers
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND customer_id = @customer_id)
        INSERT INTO SO.so_price_list_customers
               (so_price_list_header_id, customer_id, created_by)
        VALUES (@so_price_list_header_id, @customer_id, @logempcd);

    EXEC SO.P_SO_PRICE_LIST_PKG_select_price_list_customer
         @so_price_list_header_id, @logempcd;
END
GO

/* ---------------------------------------------------------------------------
   unassign_price_list_customer  -  take one off.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_unassign_price_list_customer','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_unassign_price_list_customer;
GO
-- =============================================
-- Description: Remove a customer from a price list. Returns who is left.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_unassign_price_list_customer 3, 173, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_unassign_price_list_customer]
    @so_price_list_header_id bigint,
    @customer_id             bigint,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DELETE FROM SO.so_price_list_customers
    WHERE  so_price_list_header_id = @so_price_list_header_id
      AND  customer_id = @customer_id;

    EXEC SO.P_SO_PRICE_LIST_PKG_select_price_list_customer
         @so_price_list_header_id, @logempcd;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 8 (customers) created';
GO
