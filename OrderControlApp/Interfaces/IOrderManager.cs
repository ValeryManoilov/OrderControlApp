using OrderControlApp.Models;
using OrderControlApp.Dto;
using System.Security.Cryptography.X509Certificates;
using LanguageExt;


namespace OrderControlApp.Interfaces
{
    public interface IOrderManager
    {
        public Task<string> CreateOrderAsync(CreateOrderDto createOrderData);
        public Task<Either<string, List<GetOrderDto>>> GetClientOrdersAsync(string clientName);
    }
}
