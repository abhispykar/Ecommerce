using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models;
using EOMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceOrderManagement.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private ICartRepository _cartRepository;
        private IApplicationUserRepository _applicationUserRepository;
        public CartVM vm { get; set; }

        public CartController(ICartRepository cartRepository, IApplicationUserRepository applicationUserRepository)
        {
            _cartRepository = cartRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            vm = new CartVM()
            {
                ListOfCart = _cartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"), OrderHeader  = new EOMS.Models.OrderHeader()

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

    }
}
