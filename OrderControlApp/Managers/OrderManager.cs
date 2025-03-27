using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OrderControlApp.Context;
using OrderControlApp.Dto;
using OrderControlApp.Interfaces;
using OrderControlApp.Models;

namespace OrderControlApp.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly OrderControllAppContext _context;
        private readonly ClientManager _clients;
        private readonly ProductManager _products;

        public OrderManager(OrderControllAppContext context, ClientManager clients, ProductManager products)
        {
            _context = context;
            _clients = clients;
            _products = products;
        }
        public async Task<string> CreateOrderAsync(CreateOrderDto createOrderData)
        {
            var needClient = await _clients.GetClientByNameAsync(createOrderData.ClientName);

            if (needClient.IsLeft)
            {
                return (string)needClient;
            }

            int TotalPrice = 0;

            Order newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                OrderProducts = new List<OrderProduct>()
            };

            foreach(var productData in createOrderData.ProductsData)
            {
                var needProduct = await _products.GetProductByNameAsync(productData.ProductName);

                if (needProduct.IsLeft)
                {
                    return $"Продукта {productData.ProductName} не существует";
                }

                TotalPrice += (Convert.ToInt32(((Product)needProduct).Price) * productData.Quantity);

                OrderProduct newOrderProduct = new OrderProduct
                {
                    OrderId = newOrder.Id,
                    ProductId = ((Product)needProduct).Id,
                    Order = newOrder,
                    Product = (Product)needProduct,
                    Quantity = productData.Quantity
                };

                await _context.OrderProducts.AddAsync(newOrderProduct);

                ((Product)needProduct).OrderProducts.Add(newOrderProduct);

                newOrder.OrderProducts.Add(newOrderProduct);
            }

            newOrder.TotalPrice = TotalPrice;

            await _context.Orders.AddAsync(newOrder);

            ((Client)needClient).Orders.Add(newOrder);

            await _context.SaveChangesAsync();

            return $"Заказ клиента {createOrderData.ClientName} стоимостью {TotalPrice} успешно добавлен";
        }

        public async Task<Either<string, List<GetOrderDto>>> GetClientOrdersAsync(string clientName)
        {
            var needClient = await _clients.GetClientByNameAsync(clientName);

            if (needClient.IsLeft)
            {
                return (string)needClient;
            }

            Client client = (Client)needClient;

            await _context.Entry(client).Collection(c => c.Orders).LoadAsync();

            if (client.Orders.Count() == 0)
            {
                return "У клиента нет заказов";
            }

            List<GetOrderDto> needOrders = new List<GetOrderDto>();

            foreach (var order in client.Orders)
            {
                await _context.Entry(order).Collection(o => o.OrderProducts).Query().Include(op => op.Product).LoadAsync();

                List<GetOrderProductDto> needProducts = new List<GetOrderProductDto>();

                foreach (var oproduct in order.OrderProducts)
                {

                    GetOrderProductDto newProductData = new GetOrderProductDto
                    {
                        Name = oproduct.Product.Name,
                        Price = oproduct.Product.Price,
                        Quantity = oproduct.Quantity
                    };

                    needProducts.Add(newProductData);
                };

                GetOrderDto newOrderDto = new GetOrderDto
                {
                    Id = order.Id,
                    Name = order.Client.Name,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    Products = needProducts
                };

                needOrders.Add(newOrderDto);
            }

            return needOrders;
        }

        public async Task<List<GetOrderDto>> GetAllOrdersAsync()
        {
            List<Order> orders = await _context.Orders.ToListAsync();

            List<GetOrderDto> formatOrders = new List<GetOrderDto>();

            foreach (var order in orders)
            {
                await _context.Entry(order).Collection(o => o.OrderProducts).Query().Include(op => op.Product).LoadAsync();

                await _context.Entry(order).Reference(o => o.Client).LoadAsync();

                List<GetOrderProductDto> needProducts = new List<GetOrderProductDto>();

                foreach (var oproduct in order.OrderProducts)
                {

                    GetOrderProductDto newProductData = new GetOrderProductDto
                    {
                        Name = oproduct.Product.Name,
                        Price = oproduct.Product.Price,
                        Quantity = oproduct.Quantity
                    };

                    needProducts.Add(newProductData);
                };

                GetOrderDto newOrderData = new GetOrderDto
                {
                    Id = order.Id,
                    Name = order.Client.Name,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Products = needProducts
                };

                formatOrders.Add(newOrderData);
            }

            return formatOrders;
        }
    }
}
