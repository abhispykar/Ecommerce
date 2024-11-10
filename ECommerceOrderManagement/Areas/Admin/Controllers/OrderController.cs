using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models.ViewModels;

namespace ECommerceOrderManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderController(IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public IActionResult Index()
        {
            var allOrders = _orderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
            var orderList = allOrders.Select(o => new OrderVM
            {
                OrderHeader = o,
                OrderDetails = _orderDetailRepository.GetAll(d => d.OrderHeaderId == o.Id, includeProperties: "Product")
            });

            return View(orderList);
        }

        public IActionResult UpdateStatus(int orderId, string status)
        {
            _orderHeaderRepository.UpdateStatus(orderId, status);
            _orderHeaderRepository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
