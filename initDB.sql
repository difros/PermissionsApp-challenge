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
        FOREIGN KEY (PermissionTypeId) REFERENCES PermissionTypes(Id)
    );
END

-- Insert initial data into PermissionTypes if it doesn't exist
IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Read')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Read');
END

IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Create')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Create');
END

IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Update')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Update');
END

IF NOT EXISTS (SELECT * FROM PermissionTypes WHERE Description = 'Delete')
BEGIN
    INSERT INTO PermissionTypes (Description) VALUES ('Delete');
END