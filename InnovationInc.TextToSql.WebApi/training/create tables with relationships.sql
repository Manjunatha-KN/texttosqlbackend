PRAGMA foreign_keys = ON;

-- Roles Table
CREATE TABLE Roles (
    RoleID INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleName TEXT NOT NULL UNIQUE
);

-- Users Table
CREATE TABLE Users (
    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleID INTEGER NOT NULL,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    DateOfBirth TEXT CHECK (strftime('%Y-%m-%d', DateOfBirth) <= strftime('%Y-%m-%d', 'now')),
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- Categories Table
CREATE TABLE Categories (
    CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT NOT NULL UNIQUE
);

-- Products Table
CREATE TABLE Products (
    ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryID INTEGER NOT NULL,
    ProductName TEXT NOT NULL,
    Price REAL CHECK (Price > 0),
    StockQuantity INTEGER CHECK (StockQuantity >= 0),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

-- Orders Table
CREATE TABLE Orders (
    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    OrderDate TEXT DEFAULT CURRENT_TIMESTAMP,
    TotalAmount REAL CHECK (TotalAmount >= 0),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- OrderItems Table
CREATE TABLE OrderItems (
    OrderItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    ProductID INTEGER NOT NULL,
    Quantity INTEGER CHECK (Quantity > 0),
    Price REAL NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Addresses Table
CREATE TABLE Addresses (
    AddressID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    Street TEXT NOT NULL,
    City TEXT NOT NULL,
    State TEXT NOT NULL,
    PostalCode TEXT NOT NULL,
    IsPrimary INTEGER DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Payments Table
CREATE TABLE Payments (
    PaymentID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    PaymentDate TEXT DEFAULT CURRENT_TIMESTAMP,
    PaymentMethod TEXT NOT NULL,
    Amount REAL NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

-- Reviews Table
CREATE TABLE Reviews (
    ReviewID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductID INTEGER NOT NULL,
    UserID INTEGER NOT NULL,
    Rating INTEGER CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    ReviewDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Coupons Table
CREATE TABLE Coupons (
    CouponID INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL UNIQUE,
    DiscountPercentage REAL CHECK (DiscountPercentage BETWEEN 0 AND 100),
    ExpiryDate TEXT CHECK (strftime('%Y-%m-%d', ExpiryDate) >= strftime('%Y-%m-%d', 'now'))
);

-- Wishlist Table
CREATE TABLE Wishlist (
    WishlistID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    ProductID INTEGER NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Cart Table
CREATE TABLE Cart (
    CartID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL UNIQUE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- CartItems Table
CREATE TABLE CartItems (
    CartItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    CartID INTEGER NOT NULL,
    ProductID INTEGER NOT NULL,
    Quantity INTEGER CHECK (Quantity > 0),
    FOREIGN KEY (CartID) REFERENCES Cart(CartID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Shipment Table
CREATE TABLE Shipment (
    ShipmentID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID INTEGER NOT NULL,
    ShippedDate TEXT,
    EstimatedDeliveryDate TEXT,
    Status TEXT DEFAULT 'Pending',
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

-- Suppliers Table
CREATE TABLE Suppliers (
    SupplierID INTEGER PRIMARY KEY AUTOINCREMENT,
    SupplierName TEXT NOT NULL,
    ContactEmail TEXT NOT NULL UNIQUE
);