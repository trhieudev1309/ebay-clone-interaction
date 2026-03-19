USE CloneEbayDB;
GO

-- 1. Insert Categories
SET IDENTITY_INSERT [Category] ON;
INSERT INTO [Category] ([id], [name]) VALUES 
(1, 'Electronics'),
(2, 'Fashion'),
(3, 'Books'),
(4, 'Home & Garden'),
(5, 'Toys & Hobbies'),
(6, 'Sports & Outdoors'),
(7, 'Health & Beauty'),
(8, 'Automotive'),
(9, 'Collectibles & Art');
SET IDENTITY_INSERT [Category] OFF;
GO

-- 2. Insert Users
SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] ([id], [username], [email], [password], [role], [avatarURL]) VALUES 
(1, 'seller1', 'seller1@example.com', '123456', 'Seller', 'https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg'),
(2, 'seller2', 'seller2@example.com', '123456', 'Seller', 'https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg'),
(3, 'buyer1', 'buyer1@example.com', '123456', 'Buyer', 'https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg'),
(4, 'buyer2', 'buyer2@example.com', '123456', 'Buyer', 'https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg');
SET IDENTITY_INSERT [User] OFF;
GO

-- 3. Insert Stores
SET IDENTITY_INSERT [Store] ON;
INSERT INTO [Store] ([id], [storeName], [description], [bannerImageURL], [sellerId]) VALUES 
(1, 'Best Deals Store', 'Your one-stop shop for electronics', 'https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg', 1),
(2, 'Gadget World', 'Latest gadgets and accessories', 'https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg', 2);
SET IDENTITY_INSERT [Store] OFF;
GO

-- 4. Insert Products
SET IDENTITY_INSERT [Product] ON;
INSERT INTO [Product] ([id], [title], [description], [price], [images], [categoryId], [sellerId], [isAuction], [auctionEndTime]) VALUES 
(1, 'Wireless Mouse', 'Ergonomic wireless mouse with long battery life.', 19.99, 'https://www.officewarehouse.com.ph/__resources/_web_data_/Product/Product/image_gallery/8036_6676.jpg', 1, 1, 0, NULL),
(2, 'Denim Jacket', 'Classic denim jacket for all seasons.', 59.99, 'https://thedarkgallery.vn/wp-content/uploads/2022/12/martine-rose-blue-oversized-denim-jacket.jpg', 2, 1, 1, DATEADD(day, 7, GETDATE())),
(3, 'Harry Potter and the Sorcerers Stone', 'The first book in the Harry Potter series.', 19.99, 'https://lalabookshop.com/wp-content/uploads/2025/04/892-1.jpg', 3, 2, 0, NULL);
SET IDENTITY_INSERT [Product] OFF;
GO
