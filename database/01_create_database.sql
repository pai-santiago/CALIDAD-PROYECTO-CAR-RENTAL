USE CarRental;

CREATE TABLE IF NOT EXISTS auth_roles (
    Id TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Name VARCHAR(32) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UX_auth_roles_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO auth_roles (Id, Name) VALUES (1, 'User'), (2, 'Admin');

CREATE TABLE IF NOT EXISTS auth_users (
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

SELECT u.Id, u.Username, u.Email, r.Name AS Role, u.CreatedAtUtc
FROM auth_users u
JOIN auth_roles r ON r.Id = u.RoleId
ORDER BY u.Id;
