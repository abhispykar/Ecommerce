using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOMS.Models
{
    public class Banner
    {

        public int Id { get; set; }


        [ValidateNever]
        public string ImageUrl { get; set; } = string.Empty;
        [ValidateNever]
        public bool IsActive { get; set; }
        [ValidateNever]
        public DateTime CreatedDate { get; set; }
    }
}
