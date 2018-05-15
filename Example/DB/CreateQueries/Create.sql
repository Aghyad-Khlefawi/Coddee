/*-----------------------Countries-----------------------*/
CREATE TABLE dbo.Countries
(
    Id INT NOT NULL IDENTITY(0,1),
    Name NVARCHAR(100) NOT NULL
)
GO
ALTER TABLE dbo.Countries ADD CONSTRAINT [PK_Countries_Id] PRIMARY KEY CLUSTERED(Id) 
GO 
/*---------------------------------------------------*/

/*-----------------------Cities-----------------------*/
CREATE TABLE dbo.Cities(
    Id INT NOT NULL IDENTITY(0,1),
    CountryId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL
)
GO
ALTER TABLE dbo.Cities ADD CONSTRAINT [PK_Cities_Id] PRIMARY KEY CLUSTERED(Id) 
GO
ALTER TABLE dbo.Cities ADD CONSTRAINT [FK_Cities_Countries_CountryId] FOREIGN KEY (CountryId) REFERENCES dbo.Countries(Id) 
GO 
/*---------------------------------------------------*/


/*-----------------------Jobs-----------------------*/
CREATE TABLE dbo.Jobs
(
    Id INT NOT NULL IDENTITY(0,1),
	Title NVARCHAR(50) NOT NULL
)
ALTER TABLE dbo.Jobs ADD CONSTRAINT [PK_Jobs_Id] PRIMARY KEY CLUSTERED(Id) 
GO 
/*---------------------------------------------------*/

/*-----------------------Companies-----------------------*/
CREATE TABLE dbo.Companies
(
    Id INT NOT NULL IDENTITY(0,1),
	[Name] NVARCHAR(50) NOT NULL,
)
ALTER TABLE dbo.Companies ADD CONSTRAINT [PK_Companies_Id] PRIMARY KEY CLUSTERED(Id) 
GO 
/*---------------------------------------------------*/

/*-----------------------Braches-----------------------*/
CREATE TABLE dbo.Branches
(
    Id INT NOT NULL IDENTITY(0,1),
	[Name] NVARCHAR(50) NOT NULL,
	CompanyId INT NOT NULL,
	CityId INT NOT NULL 
)
GO
ALTER TABLE dbo.Branches ADD CONSTRAINT [PK_Branches_Id] PRIMARY KEY CLUSTERED(Id) 
GO
ALTER TABLE dbo.Branches ADD CONSTRAINT [FK_Branches_Companies_CompanyId] FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id) 
GO
ALTER TABLE dbo.Branches ADD CONSTRAINT [FK_Branches_Cities_CityId] FOREIGN KEY (CityId) REFERENCES dbo.Cities(Id) 
GO
/*---------------------------------------------------*/

/*-----------------------Departments-----------------------*/
CREATE TABLE dbo.Departments
(
    Id INT NOT NULL IDENTITY(0,1),
	Title NVARCHAR(50) NOT NULL,
)
GO
ALTER TABLE dbo.Departments ADD CONSTRAINT [PK_Departments_Id] PRIMARY KEY CLUSTERED(Id) 
GO 
/*---------------------------------------------------*/


/*-----------------------Employees-----------------------*/
CREATE TABLE dbo.Employees
(
    Id INT NOT NULL IDENTITY(0,1),
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
)
GO
ALTER TABLE dbo.Employees ADD CONSTRAINT [PK_Employees_Id] PRIMARY KEY CLUSTERED(Id) 
GO
/*---------------------------------------------------*/


/*-----------------------EmployeeJobs-----------------------*/
CREATE TABLE dbo.EmployeeJobs
(
	EmployeeId INT NOT NULL,
	JobId INT NOT NULL,
	DepartmentId INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NULL,
	BranchId INT NOT NULL
)

GO
ALTER TABLE dbo.EmployeeJobs ADD CONSTRAINT [PK_EmployeeJobs_Id] PRIMARY KEY CLUSTERED (EmployeeId,JobId) 
GO
ALTER TABLE dbo.EmployeeJobs ADD CONSTRAINT [FK_Departments_Employees_EmployeeId] FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(Id) 
GO
ALTER TABLE dbo.EmployeeJobs ADD CONSTRAINT [FK_Departments_Jobs_JobId] FOREIGN KEY (JobId) REFERENCES dbo.Jobs(Id) 
GO
ALTER TABLE dbo.EmployeeJobs ADD CONSTRAINT [FK_EmployeeJobs_Branches_BranchId] FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id) 
GO 
/*---------------------------------------------------*/

/*-----------------------Users-----------------------*/
 CREATE TABLE dbo.Users
 (
     Id INT NOT NULL IDENTITY(0,1),
	 Username NVARCHAR(30) NOT NULL,
	 Email NVARCHAR(50) NOT NULL,
	 PasswordHash NVARCHAR(255) NOT NULL,
	 PasswordSalt NVARCHAR(255) NOT NULL,
 )
 GO
 ALTER TABLE dbo.Users ADD CONSTRAINT [PK_Users_Id] PRIMARY KEY CLUSTERED(Id) 
 GO
/*---------------------------------------------------*/

/*-----------------------CitiesView-----------------------*/
 CREATE VIEW dbo.CitiesView
 --WITH ENCRYPTION, SCHEMABINDING, VIEW_METADATA
 AS
     SELECT Cities.Id,
            Cities.CountryId,
            Cities.Name,
			dbo.Countries.Name AS CountryName
			FROM dbo.Cities
			INNER JOIN dbo.Countries ON Countries.Id = Cities.CountryId
 -- WITH CHECK OPTION
 GO
/*---------------------------------------------------*/

/*-----------------------BranchesView-----------------------*/
 CREATE VIEW dbo.BranchesView
 --WITH ENCRYPTION, SCHEMABINDING, VIEW_METADATA
 AS
     SELECT Branches.Id,
            Branches.Name,
            Branches.CompanyId,
            Branches.CityId,
			dbo.Cities.CountryId,
			dbo.Cities.Name AS CityName,
			dbo.Countries.Name AS CountryName,
			dbo.Companies.Name AS CompanyName
			FROM dbo.Branches
			INNER JOIN dbo.Cities ON Cities.Id = Branches.CityId
			INNER JOIN dbo.Countries ON Countries.Id = Cities.CountryId
			INNER JOIN dbo.Companies ON Companies.Id = Branches.CompanyId 
 -- WITH CHECK OPTION
 GO
 
/*---------------------------------------------------*/

/*-----------------------CompaniesView-----------------------*/
 CREATE VIEW dbo.CompaniesView
 --WITH ENCRYPTION, SCHEMABINDING, VIEW_METADATA
 AS
     SELECT Companies.Id,
            Companies.Name,
			(SELECT COUNT(*) FROM dbo.Branches WHERE CompanyId = dbo.Companies.Id) AS BranchCount,
			(SELECT COUNT(*) FROM dbo.EmployeeJobs 
				INNER JOIN dbo.Branches ON Branches.Id = EmployeeJobs.BranchId
				WHERE CompanyId = dbo.Companies.Id) AS EmployeeCount
			FROM dbo.Companies
 -- WITH CHECK OPTION
 GO
 
/*---------------------------------------------------*/