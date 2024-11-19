using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models.ViewModels;
using ECommerceOrderManagement.GlobalExceptionHandler;
using EOMS.Utility;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using EOMS.Models;
using System.Security.Claims;

namespace ECommerceOrderManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IHubContext<OrderStatusChangedHub> _hubContext;
       

        public OrderController(IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository, IHubContext<OrderStatusChangedHub> hubContext)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _hubContext = hubContext;
            
        }

        public IActionResult Index()
        {
            var allOrders = _orderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
            var orderList = allOrders.Select(o => new OrderVM
            {
                OrderHeader = o,
                OrderDetails = _orderDetailRepository.GetAll(d => d.OrderHeaderId == o.Id, includeProperties: "Product")
            }).ToList();

            return View(orderList);
        }

        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var order = _orderHeaderRepository.Get(orderId);

            if (order == null)
            {
                throw new OrderNotFoundException($"Order with ID {orderId} was not found.");
            }

            var allowedTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
    {
        { OrderStatus.Approved, new List<OrderStatus> { OrderStatus.Processed, OrderStatus.Cancelled } },
        { OrderStatus.Processed, new List<OrderStatus> { OrderStatus.Approved, OrderStatus.Shipped, OrderStatus.Cancelled } },
        { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Processed, OrderStatus.Delivered, OrderStatus.Cancelled } },
        { OrderStatus.Delivered, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } }
    };

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                TempData["error"] = "Cannot update status after cancellation.";
                return RedirectToAction(nameof(Index));
            }

            if (allowedTransitions.TryGetValue(order.OrderStatus, out var possibleStatuses) && possibleStatuses.Contains((OrderStatus)Enum.Parse(typeof(OrderStatus), status)))
            {
                _orderHeaderRepository.UpdateStatus(orderId, status);
                _orderHeaderRepository.Save();

                // Notify the client about the status update
                await _hubContext.Clients.User(order.ApplicationUserId).SendAsync("ReceiveStatusUpdate", status, order.Id);
            }
            else
            {
                // Invalid status transition
                TempData["error"] = "Invalid status transition.";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
