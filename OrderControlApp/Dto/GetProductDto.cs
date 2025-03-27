using OrderControlApp.Models;

namespace OrderControlApp.Dto
{
    public class GetProductDto
    {
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public string ClientName { get; set; }
    }
}
