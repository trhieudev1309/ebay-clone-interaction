using EbayChat.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayChat.Controllers
{
    [Route("Product")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = categoryId.HasValue
                ? await _productService.GetProductsByCategory(categoryId.Value)
                : await _productService.GetAllProducts();

            var categories = await _categoryService.GetAllCategories();

            var selectedCategory = categoryId.HasValue
                ? await _categoryService.GetCategoryById(categoryId.Value)
                : null;

            Console.WriteLine($"[ProductController] Category: {categoryId}, Products: {products?.Count}");

            ViewBag.Categories = categories;
            ViewBag.Products = products;
            ViewBag.SelectedCategory = selectedCategory;

            return View();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var reviews = await _productService.GetReviewsByProductId(id);
            var feedback = product.sellerId.HasValue 
                ? await _productService.GetSellerFeedback(product.sellerId.Value) 
                : null;

            ViewBag.Product = product;
            ViewBag.Reviews = reviews;
            ViewBag.Feedback = feedback;
            
            return View();
        }
    }
}
