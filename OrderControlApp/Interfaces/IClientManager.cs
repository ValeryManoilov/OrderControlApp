using OrderControlApp.Models;
using OrderControlApp.Dto;

namespace OrderControlApp.Interfaces
{
    public interface IClientManager
    {
        public Task<string> AddClientAsync(AddClientDto addClientData);
    }
}
