using EOMS.DataAccess.Data;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.DataAccess.Repository
{
    public class CartRepository: Repository<Cart>,ICartRepository
    {
        private ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrementCartItem(Cart cart, int count)
        {
            cart.Count -= count;
            return cart.Count;
        }

        public void Delete(Cart cart)
        {
            _db.Carts.Remove(cart);
        }

        public int IncrementCartItem(Cart cart, int count)
        {
            cart.Count += count;
            return cart.Count;
        }
        public void DeleteRange(IEnumerable<Cart> carts)
        {
            _db.Carts.RemoveRange(carts); 
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
