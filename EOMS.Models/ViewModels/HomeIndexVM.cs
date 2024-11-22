using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.Models.ViewModels
{
    public class HomeIndexVM
    {
        public IEnumerable<Product> Products { get; set; }
        public List<Banner> Banners { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortColumn { get; set; }
        public bool IsAscending { get; set; }
    }
}
