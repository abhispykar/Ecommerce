using EOMS.DataAccess.Data;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models;
using EOMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

        public OrderHeader Get(int orderId) 
        {
            return _db.OrderHeaders.FirstOrDefault(o => o.Id == orderId);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void UpdateStatus(int Id, string orderStatus)
        {
            var order = _db.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            if (order != null && Enum.TryParse(orderStatus, out OrderStatus parsedStatus))
            {
                order.OrderStatus = parsedStatus;
                _db.SaveChanges();
            }
        }
    }
}
