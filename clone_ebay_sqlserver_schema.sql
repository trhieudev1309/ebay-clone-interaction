
CREATE DATABASE CloneEbayDB;
GO

USE CloneEbayDB;
GO

CREATE TABLE [User] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [username] NVARCHAR(100),
    [email] NVARCHAR(100) UNIQUE,
    [password] NVARCHAR(255),
    [role] NVARCHAR(20),
    [avatarURL] NVARCHAR(MAX)
);

CREATE TABLE [Address] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [fullName] NVARCHAR(100),
    [phone] NVARCHAR(20),
    [street] NVARCHAR(100),
    [city] NVARCHAR(50),
    [state] NVARCHAR(50),
    [country] NVARCHAR(50),
    [isDefault] BIT
);

CREATE TABLE [Category] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [name] NVARCHAR(100)
);

CREATE TABLE [Product] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [title] NVARCHAR(255),
    [description] NVARCHAR(MAX),
    [price] DECIMAL(10,2),
    [images] NVARCHAR(MAX),
    [categoryId] INT FOREIGN KEY REFERENCES [Category](id),
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [isAuction] BIT,
    [auctionEndTime] DATETIME
);

CREATE TABLE [OrderTable] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [buyerId] INT FOREIGN KEY REFERENCES [User](id),
    [addressId] INT FOREIGN KEY REFERENCES [Address](id),
    [orderDate] DATETIME,
    [totalPrice] DECIMAL(10,2),
    [status] NVARCHAR(20)
);

CREATE TABLE [OrderItem] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [quantity] INT,
    [unitPrice] DECIMAL(10,2)
);

CREATE TABLE [Payment] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [amount] DECIMAL(10,2),
    [method] NVARCHAR(50),
    [status] NVARCHAR(20),
    [paidAt] DATETIME
);

CREATE TABLE [ShippingInfo] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [carrier] NVARCHAR(100),
    [trackingNumber] NVARCHAR(100),
    [status] NVARCHAR(50),
    [estimatedArrival] DATETIME
);

CREATE TABLE [ReturnRequest] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [reason] NVARCHAR(MAX),
    [status] NVARCHAR(20),
    [createdAt] DATETIME
);

CREATE TABLE [Bid] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [bidderId] INT FOREIGN KEY REFERENCES [User](id),
    [amount] DECIMAL(10,2),
    [bidTime] DATETIME
);

CREATE TABLE [Review] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [reviewerId] INT FOREIGN KEY REFERENCES [User](id),
    [rating] INT,
    [comment] NVARCHAR(MAX),
    [createdAt] DATETIME
);

CREATE TABLE [Message] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [senderId] INT FOREIGN KEY REFERENCES [User](id),
    [receiverId] INT FOREIGN KEY REFERENCES [User](id),
    [content] NVARCHAR(MAX),
    [timestamp] DATETIME
);

CREATE TABLE [Coupon] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [code] NVARCHAR(50),
    [discountPercent] DECIMAL(5,2),
    [startDate] DATETIME,
    [endDate] DATETIME,
    [maxUsage] INT,
    [productId] INT FOREIGN KEY REFERENCES [Product](id)
);

CREATE TABLE [Inventory] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [quantity] INT,
    [lastUpdated] DATETIME
);

CREATE TABLE [Feedback] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [averageRating] DECIMAL(3,2),
    [totalReviews] INT,
    [positiveRate] DECIMAL(5,2)
);

CREATE TABLE [Dispute] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [raisedBy] INT FOREIGN KEY REFERENCES [User](id),
    [description] NVARCHAR(MAX),
    [status] NVARCHAR(20),
    [resolution] NVARCHAR(MAX)
);

CREATE TABLE [Store] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [storeName] NVARCHAR(100),
    [description] NVARCHAR(MAX),
    [bannerImageURL] NVARCHAR(MAX)
);
