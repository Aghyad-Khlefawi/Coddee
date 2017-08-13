-- Copyright (c) Aghyad khlefawi. All rights reserved.  
-- Licensed under the MIT License. See LICENSE file in the project root for full license information.

CREATE VIEW [dbo].CompaniesView AS
	SELECT 
		dbo.Companies.ID ,
		dbo.Companies.Name,
		dbo.Companies.StateID,
		dbo.States.Name AS StateName
	FROM dbo.Companies 
	INNER JOIN dbo.States ON States.ID = Companies.StateID

CREATE VIEW [dbo].EmployeesView AS
	SELECT dbo.Employees.ID ,
           dbo.Employees.FirstName ,
           dbo.Employees.LastName ,
           dbo.Employees.CompanyID,
		   dbo.Companies.NAME AS CompanyName,
		   dbo.Companies.StateID,
		   dbo.States.Name AS StateName
		   FROM dbo.Employees
		   INNER JOIN dbo.Companies ON Companies.ID = Employees.CompanyID
		   INNER JOIN dbo.States ON States.ID = Companies.StateID
	
