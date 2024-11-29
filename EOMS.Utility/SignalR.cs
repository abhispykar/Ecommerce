using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.Utility
{
    public class SignalR : Hub
    {
        public async Task SendOrderStatusUpdate(string userId, string orderStatus, int OrderHeaderId )
        {
            await Clients.User(userId).SendAsync("ReceiveStatusUpdate", orderStatus, OrderHeaderId);
        }
    }
}