using LanguageExt;
using LanguageExt.TypeClasses;
using Microsoft.EntityFrameworkCore;
using OrderControlApp.Context;
using OrderControlApp.Dto;
using OrderControlApp.Interfaces;
using OrderControlApp.Models;

namespace OrderControlApp.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly OrderControllAppContext _context;
        private readonly ClientManager _clients;

        public ProductManager(OrderControllAppContext context, ClientManager client)
        {
            _context = context;
            _clients = client;
        }

        public async Task<Either<string, Product>> GetProductByNameAsync(string productName)
        {

            Product needProduct = await _context.Products.SingleOrDefaultAsync(p => p.Name == productName);

            if (needProduct == null)
            {
                return $"Продукт {productName} не найден";
            }

            return needProduct;
        }

        public async Task<string> AddProductAsync(AddProductDto addProductData)
        {

            var checkedClient = await GetProductByNameAsync(addProductData.Name);

            if (!checkedClient.IsLeft)
            {
                return $"Продукт {addProductData.Name} уже существует";
            }

            Product newProduct = new Product
            {
                Name = addProductData.Name,
                Price = addProductData.Price,
                OrderProducts = new List<OrderProduct>()
            };

            await _context.Products.AddAsync(newProduct);

            await _context.SaveChangesAsync();

            return $"Продукт {addProductData.Name} успешно добавлен";
        }

        public Either<string, DateTimeCheckDto> CheckDate(DateFrameDto getProductData)
        {
            var getStartDate = getProductData.StartDate;

            var getEndDate = getProductData.EndDate;

            try
            {
                if (!DateTime.TryParse($"{getStartDate.Year}-{getStartDate.Month}-{getStartDate.Day}", out DateTime startDate))
                {
                    return "Некорректно введено начало периода";
                }

                if (!DateTime.TryParse($"{getEndDate.Year}-{getEndDate.Month}-{getEndDate.Day}", out DateTime endDate))
                {
                    return "Некорректно введен конец периода";
                }

                if (startDate > endDate)
                {
                    return "Неправильно обозначены границы периода";
                }

                return new DateTimeCheckDto
                {
                    StartDate = startDate,
                    EndDate = endDate
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async Task<Either<string, List<GetProductDto>>> GetProductByDateAsync(DateFrameDto getProductData)
        {
            var result = CheckDate(getProductData);

            if (result.IsLeft)
            {
                return (string)result;
            }

            DateTime utcStartDate = ((DateTimeCheckDto)result).StartDate;

            DateTime utcEndDate = ((DateTimeCheckDto)result).EndDate;

            List<GetProductDto> getProductDtos = new List<GetProductDto>();

            foreach(var product in await _context.Products.Include(p => p.OrderProducts).ThenInclude(op => op.Order).ToListAsync())
            {
                foreach(var orderProduct in product.OrderProducts)
                {
                    if (orderProduct.Order.OrderDate >= utcStartDate && orderProduct.Order.OrderDate <= utcEndDate)
                        {
                        GetProductDto newProductDto = new GetProductDto
                        {
                            ClientName = ((Client)await _clients.GetClientByIdAsync(orderProduct.Order.ClientId)).Name,
                            Price = product.Price,
                            TotalPrice = orderProduct.Order.TotalPrice,
                            Quantity = orderProduct.Quantity,
                            ProductName = product.Name,
                            OrderDate = orderProduct.Order.OrderDate,
                        };

                        getProductDtos.Add(newProductDto);
                    }
                }    
            }

            if (getProductDtos.Count() == 0)
            {
                return "Отсутствуют заказы за данный период";
            }

            return getProductDtos;
        }

        public async Task<List<GetFormatProductDto>> GetAllProductsAsync()
        {
            List<Product> products = await _context.Products.ToListAsync();

            List<GetFormatProductDto> formatProducts = new List<GetFormatProductDto>();

            foreach (Product product in products)
            {
                GetFormatProductDto newProductData = new GetFormatProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                };

                formatProducts.Add(newProductData);
            }

            return formatProducts;
        }
    }
}
