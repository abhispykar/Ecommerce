using EOMS.DataAccess.Repository.IRepository;
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
        public CartVM itemList { get; set; }

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            itemList = new CartVM()
            {
                ListOfCart = _cartRepository.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            foreach (var item in itemList.ListOfCart)
            {
                itemList.Total += (item.Product.Price * item.Count);
            }
            return View(itemList);
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
    }
}
