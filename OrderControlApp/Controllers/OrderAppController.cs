using Microsoft.AspNetCore.Mvc;
using OrderControlApp.Dto;
using OrderControlApp.Managers;
using System.Net.WebSockets;

namespace OrderControlApp.Controllers
{
    [Controller]
    [Route("controll")]
    public class OrderAppController : ControllerBase
    {
        private readonly OrderManager _orders;
        private readonly ClientManager _clients;
        private readonly ProductManager _products;

        public OrderAppController(OrderManager orders, ClientManager clients, ProductManager products)
        {
            _orders = orders;
            _clients = clients;
            _products = products;
        }
        [HttpGet]
        [Route("getorders")]
        public async Task<IActionResult> GetClientOrdersAsync(string clientName)
        {
            var result = await _orders.GetClientOrdersAsync(clientName);

            if (result.IsLeft)
            {
                return BadRequest((string)result);
            }

            return Ok((List<GetOrderDto>)result);
        }

        [HttpPost]
        [Route("addorder")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderDto createOrderData)
        {
            var result = await _orders.CreateOrderAsync(createOrderData);

            return Ok(result);
        }

        [HttpPost]
        [Route("addclient")]
        public async Task<IActionResult> AddClientAsync([FromBody] AddClientDto clientAddData)
        {
            var result = await _clients.AddClientAsync(clientAddData);

            return Ok(result);
        }

        [HttpPost]
        [Route("addproduct")]
        public async Task<IActionResult> AddProductAsync([FromBody] AddProductDto productAddData)
        {
            var result = await _products.AddProductAsync(productAddData);

            return Ok(result);
        }


        [HttpPost]
        [Route("getproducts")]
        public async Task<IActionResult> GetProductsAsync([FromBody] DateFrameDto getProductData)
        {
            var result = await _products.GetProductByDateAsync(getProductData);

            if (result.IsLeft)
            {
                return BadRequest((string)result);
            }

            return Ok((List<GetProductDto>)result);
        }
    }
}
