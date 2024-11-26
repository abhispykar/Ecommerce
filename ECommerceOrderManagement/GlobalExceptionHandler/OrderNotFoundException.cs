    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    namespace ECommerceOrderManagement.GlobalExceptionHandler
    {
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(string message) : base(message) { }
    }
}
