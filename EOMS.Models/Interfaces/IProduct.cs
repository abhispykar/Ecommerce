namespace ECommerceOrderManagement.Interfaces
{
    public interface IProduct
    {
        int ProductId { get; set; }
        string Name { get; set; }
        decimal Price { get; set; }
        int Stock { get; set; }

    }
}
