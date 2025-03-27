using OrderControlApp.Models;

namespace OrderControlApp.Dto
{
    public class GetOrderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<GetOrderProductDto> Products { get; set; }
    }
}
