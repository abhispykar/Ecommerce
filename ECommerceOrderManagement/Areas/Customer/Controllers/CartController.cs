using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EOMS.Utility;
using EOMS.Models;

namespace ECommerceOrderManagement.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class CartController : Controller
    {
        private ICartRepository _cartRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private IOrderHeaderRepository _orderHeaderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        public CartVM vm { get; set; }

        public CartController(ICartRepository cartRepository, IApplicationUserRepository applicationUserRepository, IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _cartRepository = cartRepository;
            _applicationUserRepository = applicationUserRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            vm = new CartVM()
            {
                ListOfCart = _cartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new EOMS.Models.OrderHeader()

            };

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(vm);
        }
        public IActionResult plus(int id)
        {
            var cart = _cartRepository.GetT(x => x.Id == id);
            _cartRepository.IncrementCartItem(cart, 1);
            _cartRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int id)
        {
            var cart = _cartRepository.GetT(x => x.Id == id);
            if (cart.Count <= 1)
            {
                _cartRepository.Delete(cart);
            }
            else
            {
                _cartRepository.DecrementCartItem(cart, 1);

            }
            _cartRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult delete(int id)
        {
            var cart = _cartRepository.GetT(x => x.Id == id);
            _cartRepository.Delete(cart);
            _cartRepository.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            vm = new CartVM()
            {
                ListOfCart = _cartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new EOMS.Models.OrderHeader()

            };
            vm.OrderHeader.ApplicationUser = _applicationUserRepository.GetT(x => x.Id == claim.Value);

            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;

            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser?.Address;

            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser?.City;

            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser?.State;

            vm.OrderHeader.ZipCode = vm.OrderHeader.ApplicationUser?.ZipCode;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Summary(CartVM vm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm.ListOfCart = _cartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");
            vm.OrderHeader.OrderStatus = OrderStatus.Approved;
            vm.OrderHeader.DateOfOrder = DateTime.Now;
            vm.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            _orderHeaderRepository.Add(vm.OrderHeader);
            _orderHeaderRepository.Save();

            foreach (var item in vm.ListOfCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = vm.OrderHeader.Id,
                    Count = item.Count,
                    Price = (double)item.Product.Price
                };
                _orderDetailRepository.Add(orderDetail);
                _orderDetailRepository.Save();
            }

            _cartRepository.DeleteRange(vm.ListOfCart);
            _cartRepository.Save();

            return RedirectToAction("OrderSuccess", new { id = vm.OrderHeader.Id });
        }

        public IActionResult OrderSuccess(int id)
        {
            var order = _orderHeaderRepository.GetT(x => x.Id == id);
            return View(order); 
        }
    }
}
