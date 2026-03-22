using System.ComponentModel.DataAnnotations;

namespace EbayChat.Models.DTOs
{
    public class FeedbackSubmissionRequest
    {
        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}