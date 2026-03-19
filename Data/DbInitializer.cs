//using EbayChat.Entities;

//namespace EbayChat.Data
//{
//    public class DbInitializer
//    {
//        private readonly CloneEbayDbContext _context;

//        public DbInitializer(CloneEbayDbContext context)
//        {
//            _context = context;
//        }

//        public void Initialize()
//        {
//            _context.Database.EnsureCreated();

//            SeedCategories();
//            SeedUsers();
//            SeedStores();
//            SeedProducts();
//        }

//        private void SeedCategories()
//        {
//            if (_context.Categories.Any()) return;

//            string[] categoryNames =
//            {
//                "Electronics", "Fashion", "Books", "Home & Garden", "Toys & Hobbies",
//                "Sports & Outdoors", "Health & Beauty", "Automotive", "Collectibles & Art"
//            };

//            var categories = categoryNames.Select(name => new Category { name = name });
//            _context.Categories.AddRange(categories);
//            _context.SaveChanges();
//        }

//        private void SeedUsers()
//        {
//            if (_context.Users.Any(u => u.username == "seller1" || u.username == "seller2")) return;

//            var users = new[]
//            {
//                new User
//                {
//                    username = "seller1",
//                    email = "seller1@example.com",
//                    password = "123456",
//                    role = "Seller",
//                    avatarURL = "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg"
//                },
//                new User
//                {
//                    username = "seller2",
//                    email = "seller2@example.com",
//                    password = "123456",
//                    role = "Seller",
//                    avatarURL = "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg"
//                }
//            };

//            _context.Users.AddRange(users);
//            _context.SaveChanges();
//        }

//        private void SeedStores()
//        {
//            if (_context.Stores.Any()) return;

//            var sellers = _context.Users
//                .Where(u => u.username == "seller1" || u.username == "seller2")
//                .ToDictionary(u => u.username, u => u);

//            if (sellers.TryGetValue("seller1", out var seller1))
//            {
//                _context.Stores.Add(new Store
//                {
//                    storeName = "Best Deals Store",
//                    description = "Your one-stop shop for electronics",
//                    bannerImageURL = "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg",
//                    sellerId = seller1.id
//                });
//            }

//            if (sellers.TryGetValue("seller2", out var seller2))
//            {
//                _context.Stores.Add(new Store
//                {
//                    storeName = "Gadget World",
//                    description = "Latest gadgets and accessories",
//                    bannerImageURL = "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg",
//                    sellerId = seller2.id
//                });
//            }
//            _context.SaveChanges();
//        }

//        private void SeedProducts()
//        {
//            if (_context.Products.Any()) return;

//            var sellers = _context.Users
//                .Where(u => u.username == "seller1" || u.username == "seller2")
//                .ToDictionary(u => u.username, u => u);

//            var categories = _context.Categories
//                .Where(c => c.name == "Electronics" || c.name == "Fashion" || c.name == "Books")
//                .ToDictionary(c => c.name, c => c);

//            if (sellers.TryGetValue("seller1", out var seller1))
//            {
//                if (categories.TryGetValue("Electronics", out var electronics))
//                {
//                    _context.Products.Add(new Product
//                    {
//                        title = "Wireless Mouse",
//                        description = "Ergonomic wireless mouse with long battery life.",
//                        price = 19.99M,
//                        images = "https://www.officewarehouse.com.ph/__resources/_web_data_/products/products/image_gallery/8036_6676.jpg",
//                        categoryId = electronics.id,
//                        sellerId = seller1.id,
//                        isAuction = false
//                    });
//                }

//                if (categories.TryGetValue("Fashion", out var fashion))
//                {
//                    _context.Products.Add(new Product
//                    {
//                        title = "Denim Jacket",
//                        description = "Denim Jacket",
//                        price = 59.99M,
//                        images = "https://thedarkgallery.vn/wp-content/uploads/2022/12/martine-rose-blue-oversized-denim-jacket.jpg",
//                        categoryId = fashion.id,
//                        sellerId = seller1.id,
//                        isAuction = true,
//                        auctionEndTime = DateTime.UtcNow.AddDays(7)
//                    });
//                }
//            }

//            if (sellers.TryGetValue("seller2", out var seller2) &&
//                categories.TryGetValue("Books", out var books))
//            {
//                _context.Products.Add(new Product
//                {
//                    title = "Harry Potter",
//                    description = "Harry Potter",
//                    price = 19.99M,
//                    images = "https://lalabookshop.com/wp-content/uploads/2025/04/892-1.jpg",
//                    categoryId = books.id,
//                    sellerId = seller2.id,
//                    isAuction = false
//                });
//            }
//            _context.SaveChanges();

//        }
//    }
//}
