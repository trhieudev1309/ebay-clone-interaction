using EbayChat.Entities;
using EbayChat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IChatServices _chatServices;

        public ChatController(CloneEbayDbContext context, IChatServices chatServices)
        {
            _context = context;
            _chatServices = chatServices;
        }

        [Route("BoxChat")]
        public async Task<IActionResult> BoxChat()
        {
            var userIdStr = HttpContext.Session.GetInt32("userId");
            if (userIdStr == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = userIdStr.Value;
            var boxChats = await _chatServices.GetBoxChats(userId);

            ViewBag.UserId = userId;
            return View("Index", boxChats);
        }

        // Trang chat với một user cụ thể
        public async Task<IActionResult> Chat(int senderId, int receiverId, int? productId = null)
        {
            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(receiverId);

            if (sender == null || receiver == null)
            {
                return NotFound();
            }

            // Lấy lịch sử tin nhắn
            var messages = await _context.Messages
                .Include(m => m.sender)
                .Include(m => m.receiver)
                .Where(m => (m.senderId == senderId && m.receiverId == receiverId) ||
                           (m.senderId == receiverId && m.receiverId == senderId))
                .OrderBy(m => m.timestamp)
                .ToListAsync();

            ViewBag.SenderId = senderId;
            ViewBag.SenderName = sender.username;
            ViewBag.ReceiverId = receiverId;
            ViewBag.ReceiverName = receiver.username;
            ViewBag.Messages = messages;
            ViewBag.ProductId = productId;
            return View();
        }


        // API để thiết lập chat mới từ Product Page
        [HttpGet]
        public async Task<IActionResult> StartChat(int receiverId, int? productId = null)
        {
            var senderIdStr = HttpContext.Session.GetInt32("userId");
            var senderRole = HttpContext.Session.GetString("role");

            if (senderIdStr == null || string.IsNullOrEmpty(senderRole))
            {
                // Must be logged in to chat
                return RedirectToAction("Login", "Auth");
            }

            // Only Buyers can initiate chats
            if (senderRole.Trim().Equals("Seller", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Sellers cannot initiate messages.";
                return RedirectToAction("BoxChat");
            }

            int senderId = senderIdStr.Value;

            // Don't chat with yourself!
            if (senderId == receiverId)
            {
                return RedirectToAction("BoxChat");
            }

            // Verify the receiver exists and is a Seller
            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver == null || !receiver.role.Trim().Equals("Seller", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "You can only message active Sellers.";
                return RedirectToAction("BoxChat");
            }

            // Redirect directly to the Chat window passing both IDs matching the existing flow
            return RedirectToAction("Chat", new { senderId = senderId, receiverId = receiverId, productId = productId });
        }


        // API để lấy lịch sử tin nhắn
        [HttpGet]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            var messages = await _context.Messages
                .Include(m => m.sender)
                .Where(m => (m.senderId == senderId && m.receiverId == receiverId) ||
                           (m.senderId == receiverId && m.receiverId == senderId))
                .OrderBy(m => m.timestamp)
                .Select(m => new
                {
                    id = m.id,
                    senderId = m.senderId,
                    senderName = m.sender.username,
                    receiverId = m.receiverId,
                    content = m.content,
                    sentAt = m.timestamp.Value.ToString("HH:mm dd/MM/yyyy"),
                })
                .ToListAsync();

            return Json(messages);
        }


    }
}