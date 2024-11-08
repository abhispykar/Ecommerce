using EOMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void Save();
        int IncrementCartItem(Cart cart, int count);
        int DecrementCartItem(Cart cart, int count);
        void Delete(Cart cart);
        void DeleteRange(IEnumerable<Cart> carts);
    }
}
