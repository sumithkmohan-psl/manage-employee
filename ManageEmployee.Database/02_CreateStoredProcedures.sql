GO

/****** Object:  StoredProcedure [dbo].[SP_AddEmployee]    Script Date: 5/17/2026 8:31:15 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[SP_AddEmployee]
(
    @Name       NVARCHAR(100),
    @SSN        NVARCHAR(11),
    @DOB        DATE,
    @Address    NVARCHAR(200),
    @City       NVARCHAR(100),
    @State      NVARCHAR(50),
    @Zip        NVARCHAR(10),
    @Phone      NVARCHAR(15),
    @JoinDate   DATE,
    @Salary     DECIMAL(12,2),
    @Title      NVARCHAR(100)
)
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE @EmployeeId INT;

        -- =============================================
        -- INSERT EMPLOYEE
        -- =============================================

        INSERT INTO dbo.Employee
        (
            Name,
            SSN,
            DOB,
            Address,
            City,
            State,
            Zip,
            Phone,
            JoinDate,
            ExitDate
        )
        VALUES
        (
            @Name,
            @SSN,
            @DOB,
            @Address,
            @City,
            @State,
            @Zip,
            @Phone,
            @JoinDate,
            NULL
        );

        SET @EmployeeId = SCOPE_IDENTITY();

        -- =============================================
        -- INSERT EMPLOYEE SALARY
        -- =============================================

        INSERT INTO dbo.EmployeeSalary
        (
            EmployeeId,
            FromDate,
            Title,
            Salary
        )
        VALUES
        (
            @EmployeeId,
            @JoinDate,
            @Title,
            @Salary
        );

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH

END
GO

/****** Object:  StoredProcedure [dbo].[SP_GetEmployees]    Script Date: 5/17/2026 8:32:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetEmployees]
	-- Add the parameters for the stored procedure here
	@keyword NVARCHAR(100) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--SELECT EmployeeId, Name, SSN, DOB, Address, City, State, Zip, Phone, JoinDate, ExitDate, s.Salary, s.Title 
	--FROM Employee e
	--CROSS APPLY
	--(
	--	select top 1 Salary,Title from EmployeeSalary
	--	where EmployeeId = e.EmployeeId
	--	order by FromDate desc
	--) s
	--WHERE @keyword IS NULL OR (@keyword IS NOT NULL AND (Name LIKE @keyword OR Title LIKE @keyword))

	SELECT
    e.EmployeeId,
    e.Name,
    e.SSN,
    e.DOB,
    e.Address,
    e.City,
    e.State,
    e.Zip,
    e.Phone,
    e.JoinDate,
    e.ExitDate,
    s.Salary,
    s.Title
	FROM dbo.Employee e
	OUTER APPLY
	(
	    SELECT TOP 1
	        Salary,
	        Title
	    FROM dbo.EmployeeSalary es
	    WHERE es.EmployeeId = e.EmployeeId
	    AND es.ToDate IS NULL
	) s
	WHERE
	    @keyword IS NULL
	    OR
	    (
	        e.Name LIKE '%' + @keyword + '%'
	        OR
	        s.Title LIKE '%' + @keyword + '%'
	    )
	ORDER BY e.Name;
END
GO

