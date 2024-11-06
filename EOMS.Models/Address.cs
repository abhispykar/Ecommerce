namespace EOMS.Models
{
    public struct Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public Address(string street, string city, string zipCode)
        { Street = street; City = city; ZipCode = zipCode;
        }
        public override string ToString() 
        {
            return $"{Street}, {City}, {ZipCode}";
        }
    }
}
