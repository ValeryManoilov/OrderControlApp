using OrderControlApp.Models;

namespace OrderControlApp.Dto
{
    public class CreateOrderDto
    {
        public string ClientName { get; set; }
        public List<AddProductToOrderDto> ProductsData { get; set; }
    }
}
