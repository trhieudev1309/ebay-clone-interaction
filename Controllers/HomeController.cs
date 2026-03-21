using System.Diagnostics;
using EbayChat.Models;
using EbayChat.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, ICategoryService categoryService, IProductService productService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var categories = _categoryService.GetAllCategories().Result;
            var products = _productService.GetAllProducts().Result;

            // Calculate average rating for each product
            var productRatings = new Dictionary<int, double>();
            foreach (var product in products)
            {
                var reviews = _productService.GetReviewsByProductId(product.id).Result;
                double avgRating = reviews.Any() ? reviews.Average(r => r.rating ?? 0) : 0;
                productRatings[product.id] = avgRating;
            }

            ViewBag.Categories = categories;
            ViewBag.Products = products;
            ViewBag.ProductRatings = productRatings;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
