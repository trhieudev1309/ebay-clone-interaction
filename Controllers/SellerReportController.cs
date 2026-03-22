using EbayChat.Entities;
using EbayChat.Events;
using EbayChat.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Controllers
{
    [ApiController]
    [Route("seller-reports")]
    public class SellerReportController : ControllerBase
    {
        private readonly CloneEbayDbContext _context;
        private readonly IEventDispatcher _eventDispatcher;

        public SellerReportController(CloneEbayDbContext context, IEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> Report([FromBody] SellerReportRequest request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return Unauthorized();

            var seller = await _context.Users.FirstOrDefaultAsync(u => u.id == request.SellerId);
            if (seller == null || !string.Equals(seller.role, "Seller", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid seller.");
            }

            await _eventDispatcher.PublishAsync(
                new SellerReportedEvent(
                    userId.Value,
                    request.SellerId,
                    new Dictionary<string, string>
                    {
                        ["Reason"] = request.Reason,
                        ["Description"] = request.Description
                    }));

            return Ok(new { message = "Seller report submitted successfully." });
        }
    }
}