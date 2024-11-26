using EOMS.DataAccess.Repository;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models;
using EOMS.Models.ViewModels;
using EOMS.Utility;
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
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IBannerRepository _bannerRepository;


        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository, ICartRepository cartRepository, IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository, IBannerRepository bannerRepository
            )
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _bannerRepository = bannerRepository;

        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10, string sortColumn = "Name", bool isAscending = true)
        {
            var products = _productRepository.GetAllPagedAndSorted(
                includeProperties: "Category",
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortColumn: sortColumn,
                isAscending: isAscending
            );

            var totalProducts = _productRepository.GetAll().Count();

            var banners = _bannerRepository.GetAll(b => b.IsActive).ToList(); 

            var viewModel = new HomeIndexVM
            {
                Products = products,
                Banners = banners,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                SortColumn = sortColumn,
                IsAscending = isAscending
            };

            return View(viewModel);
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
        [Authorize(Roles = SD.Role_Customer)]
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
                    TempData["success"] = "Added to Cart";
                }
                else
                {
                    _cartRepository.IncrementCartItem(cartItem, cart.Count);
                }
                _cartRepository.Save();
              
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

        public IActionResult MyOrders()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            var orders = _orderHeaderRepository.GetAll(
                x => x.ApplicationUserId == claim.Value,
                includeProperties: "ApplicationUser");


            var orderList = orders.Select(o => new OrderVM
            {
                OrderHeader = o,
                OrderDetails = _orderDetailRepository.GetAll(d => d.OrderHeaderId == o.Id, includeProperties: "Product")
            }).ToList();

            return View(orderList);
        }

        public IActionResult Categories()
        {
            IEnumerable<Category> categories = _categoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult ProductsByCategory(int categoryId)
        {

            var productList = _productRepository.GetAll(
                predicate: product => product.CategoryId == categoryId,
                includeProperties: "Category");


            var selectedCategory = _categoryRepository.GetFirstOrDefault(
                c => c.Id == categoryId);

            ViewBag.SelectedCategoryName = selectedCategory?.Name ?? "Products";
            return View("Products", productList);
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("SearchResults", new List<Product>()); 
            }
            var results = _productRepository.GetAll(
                p => p.Name.Contains(query),
                includeProperties: "Category");

            return View("SearchResults", results);
        }

    }
}
