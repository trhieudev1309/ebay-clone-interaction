-- 30 sample product feedbacks (reviews) for Product IDs 1, 2, 3
-- reviewerId uses seeded buyers: 3 and 4

INSERT INTO [Review] ([productId], [reviewerId], [rating], [comment], [createdAt]) VALUES
-- Product 1: Wireless Mouse (10)
(1, 3, 5, N'Excellent mouse, very comfortable to use.', DATEADD(day, -30, GETDATE())),
(1, 4, 4, N'Good value for money, battery lasts long.', DATEADD(day, -28, GETDATE())),
(1, 3, 5, N'Plug and play, works perfectly.', DATEADD(day, -26, GETDATE())),
(1, 4, 4, N'Smooth tracking and lightweight.', DATEADD(day, -24, GETDATE())),
(1, 3, 5, N'Very responsive clicks.', DATEADD(day, -22, GETDATE())),
(1, 4, 3, N'Decent product, scroll wheel is a bit noisy.', DATEADD(day, -20, GETDATE())),
(1, 3, 4, N'Comfortable grip for daily work.', DATEADD(day, -18, GETDATE())),
(1, 4, 5, N'Great quality at this price range.', DATEADD(day, -16, GETDATE())),
(1, 3, 4, N'No lag, stable wireless connection.', DATEADD(day, -14, GETDATE())),
(1, 4, 5, N'Highly recommended for office use.', DATEADD(day, -12, GETDATE())),

-- Product 2: Denim Jacket (10)
(2, 3, 5, N'Looks exactly like the photos, great fit.', DATEADD(day, -29, GETDATE())),
(2, 4, 4, N'Material feels premium and durable.', DATEADD(day, -27, GETDATE())),
(2, 3, 4, N'Stylish jacket, good for cool weather.', DATEADD(day, -25, GETDATE())),
(2, 4, 5, N'Love the design, got many compliments.', DATEADD(day, -23, GETDATE())),
(2, 3, 3, N'Nice jacket but sleeves are slightly long.', DATEADD(day, -21, GETDATE())),
(2, 4, 4, N'Color and stitching are very good.', DATEADD(day, -19, GETDATE())),
(2, 3, 5, N'Perfect casual outfit piece.', DATEADD(day, -17, GETDATE())),
(2, 4, 4, N'Comfortable and fits true to size.', DATEADD(day, -15, GETDATE())),
(2, 3, 5, N'Better quality than expected.', DATEADD(day, -13, GETDATE())),
(2, 4, 4, N'Great jacket, worth buying.', DATEADD(day, -11, GETDATE())),

-- Product 3: Harry Potter book (10)
(3, 3, 5, N'Classic story, must-read for fantasy fans.', DATEADD(day, -31, GETDATE())),
(3, 4, 5, N'Book arrived in excellent condition.', DATEADD(day, -29, GETDATE())),
(3, 3, 4, N'Great print quality and paper.', DATEADD(day, -27, GETDATE())),
(3, 4, 5, N'Could not put it down, amazing read.', DATEADD(day, -25, GETDATE())),
(3, 3, 4, N'Good edition, cover looks nice.', DATEADD(day, -23, GETDATE())),
(3, 4, 5, N'Perfect gift for young readers.', DATEADD(day, -21, GETDATE())),
(3, 3, 4, N'Engaging story and easy to follow.', DATEADD(day, -19, GETDATE())),
(3, 4, 5, N'One of my favorite books ever.', DATEADD(day, -17, GETDATE())),
(3, 3, 4, N'Good price for this title.', DATEADD(day, -15, GETDATE())),
(3, 4, 5, N'Fantastic book, highly recommended.', DATEADD(day, -13, GETDATE()));