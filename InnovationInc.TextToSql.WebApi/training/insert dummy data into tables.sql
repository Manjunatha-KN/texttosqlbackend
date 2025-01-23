BEGIN TRANSACTION;

-- Insert into Roles
INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Customer'), ('Seller');

-- Insert Users (Generate > 5k records using loops or scripts)
DECLARE @ui INT = 1;
WHILE @ui <= 5000
BEGIN
    INSERT INTO Users (RoleID, FirstName, LastName, Email, PasswordHash, DateOfBirth)
    VALUES (
        (CASE WHEN @ui % 3 = 0 THEN 1 WHEN @ui % 3 = 1 THEN 2 ELSE 3 END),
        'FirstName' + CAST(@ui AS NVARCHAR),
        'LastName' + CAST(@ui AS NVARCHAR),
        'user' + CAST(@ui AS NVARCHAR) + '@example.com',
        HASHBYTES('SHA2_256', CAST(@ui AS NVARCHAR)),
        DATEADD(YEAR, -25, GETDATE())
    );
    SET @ui = @ui + 1;
END;

-- Insert Categories
INSERT INTO Categories (CategoryName)
VALUES ('Electronics'), ('Books'), ('Clothing'), ('Home & Kitchen'), ('Toys');

-- Cache max UserID and ProductID
DECLARE @maxUserID INT, @maxProductID INT, @maxOrderID INT;
SELECT @maxUserID = MAX(UserID) FROM Users;

-- Insert Products
DECLARE @pi INT = 1;
WHILE @pi <= 2000
BEGIN
    INSERT INTO Products (CategoryID, ProductName, Price, StockQuantity)
    VALUES (
        ((@pi - 1) % 5) + 1,  -- CategoryID cycles through 1 to 5
        'Product_' + CAST(@pi AS NVARCHAR(10)),
        CAST((10 + (@pi % 491)) AS DECIMAL(10, 2)),  -- Price range 10.00 to 500.00
        (@pi % 1001)  -- StockQuantity range 0 to 1000
    );
    SET @pi = @pi + 1;
END;

SELECT @maxProductID = MAX(ProductID) FROM Products;

-- Insert Orders
DECLARE @oi INT = 1;
WHILE @oi <= 2000
BEGIN
    INSERT INTO Orders (UserID, OrderDate, TotalAmount)
    VALUES (
        ((@oi - 1) % @maxUserID) + 1,  -- UserID cycles between 1 and @maxUserID
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 180), GETDATE()),  -- Random date within 180 days
        CAST((20 + ABS(CHECKSUM(NEWID()) % 1981)) AS DECIMAL(10, 2))  -- TotalAmount range
    );
    SET @oi = @oi + 1;
END;

SELECT @maxOrderID = MAX(OrderID) FROM Orders;

-- Insert OrderItems
DECLARE @oii INT = 1;
WHILE @oii <= 5000
BEGIN
    INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price)
    VALUES (
        ((@oii - 1) % @maxOrderID) + 1,
        ((@oii - 1) % @maxProductID) + 1,
        ABS(CHECKSUM(NEWID()) % 10) + 1,  -- Quantity 1-10
        CAST((20 + ABS(CHECKSUM(NEWID()) % 981)) AS DECIMAL(10, 2))  -- Price range
    );
    SET @oii = @oii + 1;
END;

-- Insert Addresses
DECLARE @ai INT = 1;
WHILE @ai <= 3000
BEGIN
    INSERT INTO Addresses (UserID, Street, City, State, PostalCode, IsPrimary)
    VALUES (
        ((@ai - 1) % @maxUserID) + 1,
        'Street ' + CAST(@ai AS NVARCHAR),
        CASE ABS(CHECKSUM(NEWID()) % 5)
            WHEN 0 THEN 'New York'
            WHEN 1 THEN 'Los Angeles'
            WHEN 2 THEN 'Chicago'
            WHEN 3 THEN 'Houston'
            WHEN 4 THEN 'Phoenix'
        END,
        CASE ABS(CHECKSUM(NEWID()) % 5)
            WHEN 0 THEN 'NY'
            WHEN 1 THEN 'CA'
            WHEN 2 THEN 'IL'
            WHEN 3 THEN 'TX'
            WHEN 4 THEN 'AZ'
        END,
        RIGHT('00000' + CAST(ABS(CHECKSUM(NEWID()) % 100000) AS NVARCHAR(5)), 5),
        ABS(CHECKSUM(NEWID()) % 2)
    );
    SET @ai = @ai + 1;
END;
COMMIT TRANSACTION;