using EbayChat.Entities;

namespace EbayChat.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetProductsByCategory(int categoryId);
        Task<Product?> GetProductById(int productId);
        Task<List<Review>> GetReviewsByProductId(int productId);
        Task<Feedback?> GetSellerFeedback(int sellerId);
    }
}
