using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EOMS.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
