using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerceOrderManagement.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository, ICartRepository cartRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _productRepository.GetAll(includeProperties: "Category");
            return View(productList);
        }

        [HttpGet]
        public IActionResult Details(int? ProductId)
        {
            if (ProductId == null)
            {
                return NotFound();
            }
            Cart cart = new Cart()
            {
                Product = _productRepository.GetT(x => x.ProductId == ProductId.Value, includeProperties: "Category"),
                Count = 1
            };

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                var cartItem = _cartRepository.GetT(x => x.ProductId == cart.ProductId && x.ApplicationUserId == claim.Value);
                if (cartItem == null)
                {
                    _cartRepository.Add(cart);
                }
                else
                {
                    _cartRepository.IncrementCartItem(cartItem, cart.Count);
                }
                _cartRepository.Save();
                return RedirectToAction("Index");
            }

            cart.Product = _productRepository.GetT(x => x.ProductId == cart.ProductId, includeProperties: "Category");
            return View(cart);
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
