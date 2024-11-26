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
    public class BannerRepository : Repository<Banner>, IBannerRepository
    {
        private readonly ApplicationDbContext _context;

        public BannerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Banner banner)
        {
            var objFromDb = _context.Banner.FirstOrDefault(b => b.Id == banner.Id);
            if (objFromDb != null)
            {
                if (!string.IsNullOrEmpty(banner.ImageUrl))
                {
                    objFromDb.ImageUrl = banner.ImageUrl;
                }
                objFromDb.IsActive = banner.IsActive;
                objFromDb.CreatedDate = banner.CreatedDate;
            }

        }
    }
}
