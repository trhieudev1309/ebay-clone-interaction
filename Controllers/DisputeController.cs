using EbayChat.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Controllers
{
    public class DisputeController : Controller
    {
        private readonly CloneEbayDbContext _context;

        public DisputeController(CloneEbayDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Raise(int orderId, string reason, string description)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var order = await _context.OrderTables.FindAsync(orderId);
            if (order == null) return NotFound();

            var dispute = new Dispute
            {
                orderId = orderId,
                raisedBy = userId.Value,
                description = $"[{reason}] {description}",
                status = "Pending",
                resolution = "Under Investigation"
            };

            _context.Disputes.Add(dispute);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your dispute claim has been submitted to the seller and admin.";
            return RedirectToAction("MyOrders", "Order");
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var role = HttpContext.Session.GetString("role")?.Trim();

            IQueryable<Dispute> query = _context.Disputes
                .Include(d => d.order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.product)
                .Include(d => d.raisedByNavigation);

            if (role == "Seller")
            {
                // Sellers see disputes for their own products
                query = query.Where(d => d.order.OrderItems.Any(oi => oi.product.sellerId == userId));
            }
            else
            {
                // Buyers see disputes they raised
                query = query.Where(d => d.raisedBy == userId);
            }

            var disputes = await query.OrderByDescending(d => d.id).ToListAsync();
            return View(disputes);
        }
    }
}
