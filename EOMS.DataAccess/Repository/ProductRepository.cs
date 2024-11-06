using ECommerceOrderManagement.Interfaces;
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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.ProductId == obj.ProductId);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Price = obj.Price;
                objFromDb.Stock = obj.Stock;
                objFromDb.CategoryId = obj.CategoryId;

                if (obj.ImageUrl != null) 
                { 
                    objFromDb.ImageUrl = obj.ImageUrl;  
                }
            }
        }
        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
