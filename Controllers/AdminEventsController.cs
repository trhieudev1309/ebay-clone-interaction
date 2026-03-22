using EbayChat.Entities;
using EbayChat.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Controllers
{
    [ApiController]
    [Route("admin/events")]
    public class AdminEventsController : ControllerBase
    {
        private readonly CloneEbayDbContext _context;

        public AdminEventsController(CloneEbayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string? status, [FromQuery] string? type)
        {
            if (!IsAdmin()) return Forbid();

            var query = _context.AdminEvents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.status == status);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(e => e.eventType == type);
            }

            var events = await query.OrderByDescending(e => e.createdAt).ToListAsync();
            return Ok(events);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            if (!IsAdmin()) return Forbid();

            var adminEvent = await _context.AdminEvents.FirstOrDefaultAsync(e => e.id == id);
            if (adminEvent == null) return NotFound();

            return Ok(adminEvent);
        }

        [HttpPatch("{id:int}/resolve")]
        public async Task<IActionResult> ResolveEvent(int id, [FromBody] ResolveAdminEventRequest? request)
        {
            if (!IsAdmin()) return Forbid();

            var adminEvent = await _context.AdminEvents.FirstOrDefaultAsync(e => e.id == id);
            if (adminEvent == null) return NotFound();

            adminEvent.status = "Resolved";

            if (!string.IsNullOrWhiteSpace(request?.Note))
            {
                adminEvent.message = $"{adminEvent.message} | Resolution: {request.Note.Trim()}";
            }

            await _context.SaveChangesAsync();
            return Ok(adminEvent);
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("role")?.Trim();
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}