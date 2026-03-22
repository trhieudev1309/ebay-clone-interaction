using System.ComponentModel.DataAnnotations;

namespace EbayChat.Models.DTOs
{
    public class SellerReportRequest
    {
        [Required]
        public int SellerId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}