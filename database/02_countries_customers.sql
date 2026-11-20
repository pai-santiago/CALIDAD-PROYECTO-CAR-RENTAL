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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO isoCountries (CountryName)
VALUES ('Mexico'), ('United States'), ('Canada');

INSERT IGNORE INTO Customers
    (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
SELECT 'Ana', 'Lopez', 'SAMPLE-001', 'ana.lopez@example.test',
       IdCountry, '555-0101', 'Quintana Roo'
FROM isoCountries WHERE CountryName = 'Mexico';

INSERT IGNORE INTO Customers
    (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
SELECT 'Luis', 'Garcia', 'SAMPLE-002', 'luis.garcia@example.test',
       IdCountry, '555-0102', 'California'
FROM isoCountries WHERE CountryName = 'United States';

INSERT IGNORE INTO Customers
    (FirstName, LastName, SSN, Email, CountryId, Mobile, StateName)
SELECT 'Sofia', 'Martin', 'SAMPLE-003', 'sofia.martin@example.test',
       IdCountry, '555-0103', 'Ontario'
FROM isoCountries WHERE CountryName = 'Canada';

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

DELIMITER ;
