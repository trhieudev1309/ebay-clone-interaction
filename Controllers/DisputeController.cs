using EbayChat.Entities;
using EbayChat.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Controllers
{
    public class DisputeController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IEventDispatcher _eventDispatcher;

        public DisputeController(CloneEbayDbContext context, IEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
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

            var isReturnRequest = reason.Contains("return", StringComparison.OrdinalIgnoreCase)
                                  || reason.Contains("refund", StringComparison.OrdinalIgnoreCase);

            if (isReturnRequest)
            {
                var returnRequest = new ReturnRequest
                {
                    orderId = orderId,
                    userId = userId.Value,
                    reason = description,
                    status = "Pending",
                    createdAt = DateTime.UtcNow.AddHours(7)
                };

                _context.ReturnRequests.Add(returnRequest);
            }

            await _context.SaveChangesAsync();

            await _eventDispatcher.PublishAsync(
                new DisputeCreatedEvent(
                    userId.Value,
                    orderId,
                    new Dictionary<string, string>
                    {
                        ["Reason"] = reason,
                        ["DisputeId"] = dispute.id.ToString()
                    }));

            if (isReturnRequest)
            {
                await _eventDispatcher.PublishAsync(
                    new ReturnRequestedEvent(
                        userId.Value,
                        orderId,
                        new Dictionary<string, string>
                        {
                            ["Reason"] = reason
                        }));
            }

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
                query = query.Where(d => d.order != null &&
                                         d.order.OrderItems.Any(oi => oi.product != null && oi.product.sellerId == userId));
            }
            else if (role == "Buyer")
            {
                // Buyers see disputes they raised
                query = query.Where(d => d.raisedBy == userId);
            }

            var disputes = await query.OrderByDescending(d => d.id).ToListAsync();
            return View(disputes);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return Unauthorized();

            var role = HttpContext.Session.GetString("role")?.Trim();
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var dispute = await _context.Disputes
                .Include(d => d.raisedByNavigation)
                .FirstOrDefaultAsync(d => d.id == id);

            if (dispute == null) return NotFound();

            return Json(new
            {
                id = dispute.id,
                orderId = dispute.orderId,
                raisedBy = dispute.raisedByNavigation?.username ?? $"User {dispute.raisedBy}",
                description = dispute.description ?? string.Empty,
                status = dispute.status ?? "Pending",
                resolution = dispute.resolution ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int disputeId, string status, string? resolution)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var role = HttpContext.Session.GetString("role")?.Trim();
            if (role != "Admin")
            {
                return Forbid();
            }

            var allowedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Pending",
                "In Review",
                "Resolved",
                "Rejected"
            };

            if (!allowedStatuses.Contains(status))
            {
                TempData["ErrorMessage"] = "Invalid dispute status.";
                return RedirectToAction(nameof(Manage));
            }

            var dispute = await _context.Disputes.FindAsync(disputeId);
            if (dispute == null) return NotFound();

            dispute.status = status;
            dispute.resolution = string.IsNullOrWhiteSpace(resolution) ? dispute.resolution : resolution.Trim();

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Dispute status updated successfully.";

            return RedirectToAction(nameof(Manage));
        }
    }
}
