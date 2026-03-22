using EbayChat.Events;
using EbayChat.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EbayChat.Controllers
{
    [ApiController]
    [Route("feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly IEventDispatcher _eventDispatcher;

        public FeedbackController(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] FeedbackSubmissionRequest request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return Unauthorized();

            await _eventDispatcher.PublishAsync(
                new FeedbackSubmittedEvent(
                    userId.Value,
                    0,
                    new Dictionary<string, string>
                    {
                        ["Subject"] = request.Subject,
                        ["Message"] = request.Message
                    }));

            return Ok(new { message = "Feedback submitted successfully." });
        }
    }
}