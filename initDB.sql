-- Create PermissionTypes table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PermissionTypes' AND xtype='U')
BEGIN
    CREATE TABLE PermissionTypes (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Description NVARCHAR(255) NOT NULL UNIQUE
    );
END

-- Create Permissions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Permissions' AND xtype='U')
BEGIN
    CREATE TABLE Permissions (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EmployeeName NVARCHAR(255) NOT NULL,
        EmployeeLastName NVARCHAR(255) NOT NULL,
        PermissionTypeId INT NOT NULL,
        Date DATETIME NOT NULL,
        FOREIGN KEY (PermissionTypeId) REFERENCES PermissionTypes(Id),
        CONSTRAINT UQ_Employee UNIQUE (EmployeeName, EmployeeLastName)
    );
END

-- Insert initial data into PermissionTypes if it doesn't exist
IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Level 1')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Level 1');
END

IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Level 2')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Level 2');
END

IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Level 3')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Level 3');
END