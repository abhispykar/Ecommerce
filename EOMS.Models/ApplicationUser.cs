using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EOMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        // Store Address as JSON
        public string? ShippingAddressJson { get; set; } 
        [NotMapped]
        public Address? ShippingAddress
        { get => ShippingAddressJson == null ? (Address?)null : JsonSerializer.Deserialize<Address>(ShippingAddressJson); set => ShippingAddressJson = JsonSerializer.Serialize(value); }


    }
}
