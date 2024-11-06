using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime DateOfOrder { get; set; }

        public DateTime DateOfShipping { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }

        public DateTime DateOfPayment { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public string Phone { get; set; }

        public string? ShippingAddressJson { get; set; }

        // Address property, ignored by EF Core
        [NotMapped]
        public Address? Address
        {
            get => string.IsNullOrEmpty(ShippingAddressJson) ? null : JsonSerializer.Deserialize<Address>(ShippingAddressJson);
            set => ShippingAddressJson = JsonSerializer.Serialize(value);
        }

        [Required]
        public string Name { get; set; }



    }
}
