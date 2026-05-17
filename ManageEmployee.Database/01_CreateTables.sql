/****** Object:  Table [dbo].[Employee]    Script Date: 5/17/2026 8:28:52 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employee](
	[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SSN] [char](11) NOT NULL,
	[DOB] [date] NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[Zip] [varchar](10) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[JoinDate] [date] NOT NULL,
	[ExitDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SSN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [CK_Employee_ExitAfterJoin] CHECK  (([ExitDate] IS NULL OR [ExitDate]>=[JoinDate]))
GO

ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [CK_Employee_ExitAfterJoin]
GO

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [CK_Employee_ExitDate] CHECK  (([ExitDate] IS NULL OR [ExitDate]<=CONVERT([date],getdate())))
GO

ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [CK_Employee_ExitDate]
GO

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [CK_Employee_JoinDate] CHECK  (([JoinDate]<=CONVERT([date],getdate())))
GO

ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [CK_Employee_JoinDate]
GO
/****** Object:  Table [dbo].[EmployeeSalary]    Script Date: 5/17/2026 8:30:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmployeeSalary](
	[EmployeeSalaryId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[FromDate] [date] NOT NULL,
	[ToDate] [date] NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Salary] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmployeeSalaryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmployeeSalary]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSalary_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO

ALTER TABLE [dbo].[EmployeeSalary] CHECK CONSTRAINT [FK_EmployeeSalary_Employee]
GO

ALTER TABLE [dbo].[EmployeeSalary]  WITH CHECK ADD  CONSTRAINT [CK_EmployeeSalary_FromDate] CHECK  (([FromDate]<=CONVERT([date],getdate())))
GO

ALTER TABLE [dbo].[EmployeeSalary] CHECK CONSTRAINT [CK_EmployeeSalary_FromDate]
GO

ALTER TABLE [dbo].[EmployeeSalary]  WITH CHECK ADD  CONSTRAINT [CK_EmployeeSalary_ToAfterFrom] CHECK  (([ToDate] IS NULL OR [ToDate]>=[FromDate]))
GO

ALTER TABLE [dbo].[EmployeeSalary] CHECK CONSTRAINT [CK_EmployeeSalary_ToAfterFrom]
GO

ALTER TABLE [dbo].[EmployeeSalary]  WITH CHECK ADD  CONSTRAINT [CK_EmployeeSalary_ToDate] CHECK  (([ToDate] IS NULL OR [ToDate]<=CONVERT([date],getdate())))
GO

ALTER TABLE [dbo].[EmployeeSalary] CHECK CONSTRAINT [CK_EmployeeSalary_ToDate]
GO


