using EbayChat.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Controllers
{
    public class OrderController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public OrderController(CloneEbayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _context.OrderTables
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.buyerId == userId)
                .OrderByDescending(o => o.orderDate)
                .ToListAsync();

            return View("Index", orders);
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            // Simple Order Creation for Demo
            var order = new OrderTable
            {
                buyerId = userId.Value,
                orderDate = DateTime.UtcNow.AddHours(7),
                status = "Completed",
                totalPrice = product.price * quantity
            };

            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                orderId = order.id,
                productId = productId,
                quantity = quantity,
                unitPrice = product.price
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Chúc mừng! Bạn đã mua hàng thành công. Đơn hàng đã được tạo tự động.";
            return RedirectToAction("MyOrders");
        }
    }
}
