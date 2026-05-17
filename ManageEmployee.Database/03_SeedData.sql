SET NOCOUNT ON;

DECLARE @Counter INT = 1;
DECLARE @EmployeeId INT;

DECLARE @FirstNames TABLE
(
    Id INT IDENTITY(1,1),
    Name NVARCHAR(50)
);

INSERT INTO @FirstNames (Name)
VALUES
('Michael'),
('Emily'),
('Daniel'),
('Sophia'),
('James'),
('Olivia'),
('William'),
('Emma'),
('Benjamin'),
('Charlotte'),
('Elijah'),
('Amelia'),
('Alexander'),
('Mia'),
('Matthew'),
('Harper'),
('David'),
('Evelyn'),
('Joseph'),
('Abigail');

DECLARE @LastNames TABLE
(
    Id INT IDENTITY(1,1),
    Name NVARCHAR(50)
);

INSERT INTO @LastNames (Name)
VALUES
('Johnson'),
('Smith'),
('Williams'),
('Brown'),
('Jones'),
('Garcia'),
('Miller'),
('Davis'),
('Wilson'),
('Anderson'),
('Thomas'),
('Taylor'),
('Moore'),
('Martin'),
('Jackson'),
('White'),
('Harris'),
('Clark'),
('Lewis'),
('Walker');

DECLARE @Cities TABLE
(
    Id INT IDENTITY(1,1),
    City NVARCHAR(100),
    StateName NVARCHAR(100),
    ZipCode NVARCHAR(10)
);

INSERT INTO @Cities (City, StateName, ZipCode)
VALUES
('Dallas', 'Texas', '75201'),
('Seattle', 'Washington', '98101'),
('Chicago', 'Illinois', '60601'),
('Austin', 'Texas', '73301'),
('Phoenix', 'Arizona', '85001'),
('Atlanta', 'Georgia', '30301'),
('Boston', 'Massachusetts', '02101'),
('Denver', 'Colorado', '80201'),
('Miami', 'Florida', '33101'),
('San Diego', 'California', '92101');

DECLARE @Titles TABLE
(
    Id INT IDENTITY(1,1),
    Title NVARCHAR(100),
    MinSalary DECIMAL(12,2),
    MaxSalary DECIMAL(12,2)
);

INSERT INTO @Titles (Title, MinSalary, MaxSalary)
VALUES
('Software Engineer', 70000, 120000),
('Senior Software Engineer', 90000, 150000),
('QA Engineer', 60000, 95000),
('Project Manager', 95000, 140000),
('Business Analyst', 65000, 105000),
('Database Administrator', 80000, 130000),
('DevOps Engineer', 85000, 140000),
('Technical Lead', 110000, 160000);

WHILE @Counter <= 100
BEGIN
    DECLARE @FirstName NVARCHAR(50);
    DECLARE @LastName NVARCHAR(50);
    DECLARE @FullName NVARCHAR(100);

    DECLARE @City NVARCHAR(100);
    DECLARE @State NVARCHAR(100);
    DECLARE @Zip NVARCHAR(10);

    DECLARE @Title NVARCHAR(100);
    DECLARE @Salary DECIMAL(12,2);

    DECLARE @DOB DATE;
    DECLARE @JoinDate DATE;

    DECLARE @Address NVARCHAR(200);
    DECLARE @Phone NVARCHAR(20);
    DECLARE @SSN NVARCHAR(11);

    SELECT TOP 1
        @FirstName = Name
    FROM @FirstNames
    ORDER BY NEWID();

    SELECT TOP 1
        @LastName = Name
    FROM @LastNames
    ORDER BY NEWID();

    SET @FullName =
        @FirstName + ' ' + @LastName;

    SELECT TOP 1
        @City = City,
        @State = StateName,
        @Zip = ZipCode
    FROM @Cities
    ORDER BY NEWID();

    SELECT TOP 1
        @Title = Title,
        @Salary = CAST(
            RAND(CHECKSUM(NEWID()))
            * (MaxSalary - MinSalary)
            + MinSalary AS DECIMAL(12,2))
    FROM @Titles
    ORDER BY NEWID();

    SET @DOB = DATEADD(
        DAY,
        -1 * (RAND(CHECKSUM(NEWID())) * 15000 + 8030),
        CAST(GETDATE() AS DATE));

    SET @JoinDate = DATEADD(
        DAY,
        -1 * (RAND(CHECKSUM(NEWID())) * 3650),
        CAST(GETDATE() AS DATE));

    SET @Address =
    CAST(
        CAST(
            (RAND(CHECKSUM(NEWID())) * 9999 + 1) AS INT
        ) AS VARCHAR(10)
    )
    + ' Oak Street';

    SET @Phone =
        '555-'
        + RIGHT('000' + CAST(ABS(CHECKSUM(NEWID())) % 1000 AS VARCHAR), 3)
        + '-'
        + RIGHT('0000' + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS VARCHAR), 4);

    SET @SSN =
        RIGHT('000' + CAST(ABS(CHECKSUM(NEWID())) % 900 + 100 AS VARCHAR), 3)
        + '-'
        + RIGHT('00' + CAST(ABS(CHECKSUM(NEWID())) % 90 + 10 AS VARCHAR), 2)
        + '-'
        + RIGHT('0000' + CAST(ABS(CHECKSUM(NEWID())) % 9000 + 1000 AS VARCHAR), 4);

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Employee
        WHERE SSN = @SSN
    )
    BEGIN
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
            @FullName,
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

        INSERT INTO dbo.EmployeeSalary
        (
            EmployeeId,
            FromDate,
            ToDate,
            Title,
            Salary
        )
        VALUES
        (
            @EmployeeId,
            @JoinDate,
            NULL,
            @Title,
            @Salary
        );

        SET @Counter = @Counter + 1;
    END
END

-- ADD SECOND SALARY RECORDS FOR SOME EMPLOYEES

INSERT INTO dbo.EmployeeSalary
(
    EmployeeId,
    FromDate,
    ToDate,
    Title,
    Salary
)
SELECT TOP 20
    es.EmployeeId,
    DATEADD(YEAR, 1, es.FromDate),
    NULL,
    CASE
        WHEN es.Title = 'Software Engineer'
            THEN 'Senior Software Engineer'
        WHEN es.Title = 'QA Engineer'
            THEN 'Senior QA Engineer'
        WHEN es.Title = 'Business Analyst'
            THEN 'Senior Business Analyst'
		WHEN es.Title = 'Senior Software Engineer'
            THEN 'Technical Lead'
		WHEN es.Title = 'DevOps Engineer'
            THEN 'Senior DevOps Engineer'
        ELSE es.Title
    END,
    es.Salary + 15000
FROM dbo.EmployeeSalary es
WHERE es.ToDate IS NULL
AND DATEADD(YEAR, 1, es.FromDate)
    <= CAST(GETDATE() AS DATE);
-- CLOSE OLD SALARY RECORDS

UPDATE oldSalary
SET oldSalary.ToDate =
    DATEADD(
        DAY,
        -1,
        newSalary.FromDate)
FROM dbo.EmployeeSalary oldSalary
INNER JOIN dbo.EmployeeSalary newSalary
    ON oldSalary.EmployeeId =
        newSalary.EmployeeId
WHERE oldSalary.EmployeeSalaryId
        <> newSalary.EmployeeSalaryId
AND oldSalary.FromDate
        < newSalary.FromDate
AND oldSalary.ToDate IS NULL;

-- CLOSE CURRENT SALARY RECORDS
-- FOR SOME EMPLOYEES

UPDATE TOP (10) es
SET es.ToDate =
    CASE
        WHEN DATEADD(
                DAY,
                365,
                es.FromDate) > CAST(GETDATE() AS DATE)
        THEN CAST(GETDATE() AS DATE)
        ELSE DATEADD(
                DAY,
                365,
                es.FromDate)
    END
FROM dbo.EmployeeSalary es
WHERE es.ToDate IS NULL;

-- UPDATE EMPLOYEE EXIT DATE
-- BASED ON LAST SALARY END DATE

UPDATE e
SET e.ExitDate = s.LastToDate
FROM dbo.Employee e
INNER JOIN
(
    SELECT
        EmployeeId,
        MAX(ToDate) AS LastToDate
    FROM dbo.EmployeeSalary
    WHERE ToDate IS NOT NULL
    GROUP BY EmployeeId
) s
    ON e.EmployeeId = s.EmployeeId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.EmployeeSalary es
    WHERE es.EmployeeId = e.EmployeeId
    AND es.ToDate IS NULL
);

PRINT '100 employee records inserted successfully.';


--## Features

--* Generates 100 realistic US employees
--* Prevents duplicate SSN
--* Uses realistic US cities/states
--* Generates realistic salaries
--* Ensures dates are not future dates
--* Adds salary record for every employee
--* Uses random realistic job titles
