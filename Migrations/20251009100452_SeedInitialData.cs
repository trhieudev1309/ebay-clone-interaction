using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbayChat.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ===== Category =====
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Fashion" },
                    { 3, "Books" },
                    { 4, "Home & Garden" },
                    { 5, "Toys & Hobbies" },
                    { 6, "Sports & Outdoors" },
                    { 7, "Health & Beauty" },
                    { 8, "Automotive" },
                    { 9, "Collectibles & Art" }
                });

            // ===== User =====
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "id", "username", "email", "password", "role", "avatarURL" },
                values: new object[,]
                {
                    {
                        1,
                        "seller1",
                        "seller1@example.com",
                        "123456",
                        "Seller",
                        "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg"
                    },
                    {
                        2,
                        "seller2",
                        "seller2@example.com",
                        "123456",
                        "Seller",
                        "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg"
                    },
                    {
                        3,
                        "buyer1",
                        "buyer1@example.com",
                        "123456",
                        "Buyer",
                        "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg"
                    },
                    {
                        4,
                        "buyer2",
                        "buyer2@example.com",
                        "123456",
                        "Buyer",
                        "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg"
                    }
                });

            // ===== Store =====
            migrationBuilder.InsertData(
                table: "Store",
                columns: new[] { "id", "storeName", "description", "bannerImageURL", "sellerId" },
                values: new object[,]
                {
                    {
                        1,
                        "Best Deals Store",
                        "Your one-stop shop for electronics",
                        "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg",
                        1
                    },
                    {
                        2,
                        "Gadget World",
                        "Latest gadgets and accessories",
                        "https://static.wixstatic.com/media/868f94_2f5588725fee45559920e92e675c0f25~mv2.jpg",
                        2
                    }
                });

            // ===== Product =====
            migrationBuilder.InsertData(
                table: "Product",
                columns: new[]
                {
                    "id",
                    "title",
                    "description",
                    "price",
                    "images",
                    "categoryId",
                    "sellerId",
                    "isAuction",
                    "auctionEndTime"
                },
                values: new object[,]
                {
                    {
                        1,
                        "Wireless Mouse",
                        "Ergonomic wireless mouse with long battery life.",
                        19.99m,
                        "https://www.officewarehouse.com.ph/__resources/_web_data_/Product/Product/image_gallery/8036_6676.jpg",
                        1,
                        1,
                        false,
                        null
                    },
                    {
                        2,
                        "Denim Jacket",
                        "Denim Jacket",
                        59.99m,
                        "https://thedarkgallery.vn/wp-content/uploads/2022/12/martine-rose-blue-oversized-denim-jacket.jpg",
                        2,
                        1,
                        true,
                        new DateTime(2025, 10, 16)
                    },
                    {
                        3,
                        "Harry Potter",
                        "Harry Potter",
                        19.99m,
                        "https://lalabookshop.com/wp-content/uploads/2025/04/892-1.jpg",
                        3,
                        2,
                        false,
                        null
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove data in reverse order
            migrationBuilder.DeleteData(table: "Product", keyColumn: "id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Product", keyColumn: "id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Product", keyColumn: "id", keyValue: 3);

            migrationBuilder.DeleteData(table: "Store", keyColumn: "id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Store", keyColumn: "id", keyValue: 2);

            migrationBuilder.DeleteData(table: "User", keyColumn: "id", keyValue: 1);
            migrationBuilder.DeleteData(table: "User", keyColumn: "id", keyValue: 2);

            for (int i = 1; i <= 9; i++)
            {
                migrationBuilder.DeleteData(table: "Category", keyColumn: "id", keyValue: i);
            }
        }
    }
}
