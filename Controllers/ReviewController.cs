using EbayChat.Entities;
using EbayChat.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Controllers
{
    public class ReviewController : Controller
    {
        private readonly CloneEbayDbContext _context;
        private readonly IEventDispatcher _eventDispatcher;

        public ReviewController(CloneEbayDbContext context, IEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var review = new Review
            {
                productId = productId,
                reviewerId = userId.Value,
                rating = rating,
                comment = comment,
                createdAt = DateTime.UtcNow.AddHours(7)
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            if (rating <= 2)
            {
                await _eventDispatcher.PublishAsync(
                    new LowRatingDetectedEvent(
                        userId.Value,
                        productId,
                        new Dictionary<string, string>
                        {
                            ["Rating"] = rating.ToString(),
                            ["ReviewId"] = review.id.ToString()
                        }));
            }

            var product = await _context.Products.Include(p => p.seller).FirstOrDefaultAsync(p => p.id == productId);
            if (product != null && product.sellerId.HasValue)
            {
                await UpdateSellerFeedback(product.sellerId.Value);
            }

            return RedirectToAction("Detail", "Product", new { id = productId });
        }

        private async Task UpdateSellerFeedback(int sellerId)
        {
            var sellerReviews = await _context.Reviews
                .Include(r => r.product)
                .Where(r => r.product.sellerId == sellerId)
                .ToListAsync();

            if (sellerReviews.Any())
            {
                var avgRating = (decimal)sellerReviews.Average(r => r.rating ?? 0);
                var total = sellerReviews.Count;
                var positiveCount = sellerReviews.Count(r => r.rating >= 4);
                var positiveRate = (decimal)positiveCount / total * 100;

                var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.sellerId == sellerId);
                if (feedback == null)
                {
                    feedback = new Feedback { sellerId = sellerId };
                    _context.Feedbacks.Add(feedback);
                }

                feedback.averageRating = avgRating;
                feedback.totalReviews = total;
                feedback.positiveRate = positiveRate;

                await _context.SaveChangesAsync();
            }
        }
    }
}
