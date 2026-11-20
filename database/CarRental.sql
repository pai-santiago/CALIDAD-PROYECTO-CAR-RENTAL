CREATE DATABASE IF NOT EXISTS carrental
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE carrental;

CREATE TABLE IF NOT EXISTS isoCountries
(
    IdCountry INT NOT NULL AUTO_INCREMENT,
    CountryName VARCHAR(100) NOT NULL,
    PRIMARY KEY (IdCountry),
    UNIQUE KEY UQ_isoCountries_CountryName (CountryName)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Customers
(
    IdCustomer INT NOT NULL AUTO_INCREMENT,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    SSN VARCHAR(50) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    CountryId INT NOT NULL,
    Mobile VARCHAR(50) NOT NULL,
    StateName VARCHAR(100) NOT NULL,
    PRIMARY KEY (IdCustomer),
    UNIQUE KEY UQ_Customers_Email (Email),
    CONSTRAINT FK_Customers_isoCountries FOREIGN KEY (CountryId)
        REFERENCES isoCountries (IdCountry)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS auth_roles
(
    Id TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Name VARCHAR(32) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UX_auth_roles_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO auth_roles (Id, Name)
VALUES (1, 'User'), (2, 'Admin');

CREATE TABLE IF NOT EXISTS auth_users
(
    Id BIGINT NOT NULL AUTO_INCREMENT,
    Username VARCHAR(60) NOT NULL,
    NormalizedUsername VARCHAR(60) NOT NULL,
    Email VARCHAR(254) NOT NULL,
    NormalizedEmail VARCHAR(254) NOT NULL,
    PasswordHash VARCHAR(512) NOT NULL,
    RoleId TINYINT UNSIGNED NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    PRIMARY KEY (Id),
    UNIQUE KEY UX_auth_users_NormalizedUsername (NormalizedUsername),
    UNIQUE KEY UX_auth_users_NormalizedEmail (NormalizedEmail),
    CONSTRAINT FK_auth_users_auth_roles FOREIGN KEY (RoleId) REFERENCES auth_roles (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO isoCountries (CountryName)
VALUES ('Mexico'), ('United States'), ('Canada');

INSERT IGNORE INTO Customers
    (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
SELECT 'Ana', 'Lopez', 'SAMPLE-001', 'ana.lopez@example.test',
       IdCountry, '555-0101', 'Quintana Roo'
FROM isoCountries
WHERE CountryName = 'Mexico';

INSERT IGNORE INTO Customers
    (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
SELECT 'Luis', 'Garcia', 'SAMPLE-002', 'luis.garcia@example.test',
       IdCountry, '555-0102', 'California'
FROM isoCountries
WHERE CountryName = 'United States';

INSERT IGNORE INTO Customers
    (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
SELECT 'Sofia', 'Martin', 'SAMPLE-003', 'sofia.martin@example.test',
       IdCountry, '555-0103', 'Ontario'
FROM isoCountries
WHERE CountryName = 'Canada';

DELIMITER $$

DROP PROCEDURE IF EXISTS GetCountries$$
CREATE PROCEDURE GetCountries()
BEGIN
    SELECT IdCountry, CountryName
    FROM isoCountries
    ORDER BY IdCountry;
END$$

DROP PROCEDURE IF EXISTS GetCountryById$$
CREATE PROCEDURE GetCountryById(IN p_CountryId INT)
BEGIN
    SELECT IdCountry, CountryName
    FROM isoCountries
    WHERE IdCountry = p_CountryId;
END$$

DROP PROCEDURE IF EXISTS InsertCountry$$
CREATE PROCEDURE InsertCountry(IN p_CountryName VARCHAR(100))
BEGIN
    INSERT INTO isoCountries (CountryName)
    VALUES (p_CountryName);
    SELECT LAST_INSERT_ID() AS newCountryId;
END$$

DROP PROCEDURE IF EXISTS UpdateCountry$$
CREATE PROCEDURE UpdateCountry(IN p_CountryName VARCHAR(100), IN p_CountryId INT)
BEGIN
    UPDATE isoCountries
    SET CountryName = p_CountryName
    WHERE IdCountry = p_CountryId;

    SELECT EXISTS(
        SELECT 1 FROM isoCountries WHERE IdCountry = p_CountryId
    ) AS RowsAffected;
END$$

DROP PROCEDURE IF EXISTS GetCustomers$$
CREATE PROCEDURE GetCustomers()
BEGIN
    SELECT c.IdCustomer, c.FirstName, c.LastName, c.SSN, c.Email,
           country.CountryName AS Country, c.CountryId, c.Mobile, c.StateName
    FROM Customers AS c
    INNER JOIN isoCountries AS country ON country.IdCountry = c.CountryId
    ORDER BY c.IdCustomer;
END$$

DROP PROCEDURE IF EXISTS GetCustomerById$$
CREATE PROCEDURE GetCustomerById(IN p_CustomerId INT)
BEGIN
    SELECT c.IdCustomer, c.FirstName, c.LastName, c.SSN, c.Email,
           country.CountryName AS Country, c.CountryId, c.Mobile, c.StateName
    FROM Customers AS c
    INNER JOIN isoCountries AS country ON country.IdCountry = c.CountryId
    WHERE c.IdCustomer = p_CustomerId;
END$$

DROP PROCEDURE IF EXISTS InsertCustomer$$
CREATE PROCEDURE InsertCustomer(
    IN p_FirstName VARCHAR(50),
    IN p_LastName VARCHAR(50),
    IN p_SSN VARCHAR(50),
    IN p_Email VARCHAR(255),
    IN p_CountryId INT,
    IN p_Mobile VARCHAR(50),
    IN p_StateName VARCHAR(100)
)
BEGIN
    INSERT INTO Customers (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
    VALUES (p_FirstName, p_LastName, p_SSN, p_Email, p_CountryId, p_Mobile, p_StateName);
    SELECT LAST_INSERT_ID() AS newCustomerId;
END$$

DROP PROCEDURE IF EXISTS UpdateCustomer$$
CREATE PROCEDURE UpdateCustomer(
    IN p_FirstName VARCHAR(50),
    IN p_LastName VARCHAR(50),
    IN p_SSN VARCHAR(50),
    IN p_Email VARCHAR(255),
    IN p_CountryId INT,
    IN p_Mobile VARCHAR(50),
    IN p_StateName VARCHAR(100),
    IN p_CustomerId INT
)
BEGIN
    UPDATE Customers
    SET FirstName = p_FirstName,
        LastName = p_LastName,
        SSN = p_SSN,
        Email = p_Email,
        CountryId = p_CountryId,
        Mobile = p_Mobile,
        StateName = p_StateName
    WHERE IdCustomer = p_CustomerId;

    SELECT EXISTS(
        SELECT 1 FROM Customers WHERE IdCustomer = p_CustomerId
    ) AS RowsAffected;
END$$

DROP PROCEDURE IF EXISTS GetAuthUserByUsername$$
CREATE PROCEDURE GetAuthUserByUsername(IN p_NormalizedUsername VARCHAR(60))
BEGIN
    SELECT u.Id, u.Username, u.NormalizedUsername, u.Email, u.NormalizedEmail,
           u.PasswordHash, r.Name AS Role, u.CreatedAtUtc
    FROM auth_users AS u
    INNER JOIN auth_roles AS r ON r.Id = u.RoleId
    WHERE u.NormalizedUsername = p_NormalizedUsername COLLATE utf8mb4_unicode_ci
    LIMIT 1;
END$$

DROP PROCEDURE IF EXISTS CreateAuthUser$$
CREATE PROCEDURE CreateAuthUser(
    IN p_Username VARCHAR(60),
    IN p_NormalizedUsername VARCHAR(60),
    IN p_Email VARCHAR(254),
    IN p_NormalizedEmail VARCHAR(254),
    IN p_PasswordHash VARCHAR(512),
    IN p_CreatedAtUtc DATETIME(6)
)
BEGIN
    INSERT INTO auth_users
        (Username, NormalizedUsername, Email, NormalizedEmail, PasswordHash, RoleId, CreatedAtUtc)
    VALUES
        (p_Username, p_NormalizedUsername, p_Email, p_NormalizedEmail, p_PasswordHash,
         (SELECT Id FROM auth_roles WHERE Name = 'User' COLLATE utf8mb4_unicode_ci), p_CreatedAtUtc);

    SELECT LAST_INSERT_ID() AS newUserId;
END$$

DELIMITER ;
