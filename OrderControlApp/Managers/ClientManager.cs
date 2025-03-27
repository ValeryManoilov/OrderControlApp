using Microsoft.EntityFrameworkCore;
using OrderControlApp.Context;
using OrderControlApp.Dto;
using OrderControlApp.Models;
using LanguageExt;
using OrderControlApp.Interfaces;
namespace OrderControlApp.Managers
{
    public class ClientManager: IClientManager
    {
        private readonly OrderControllAppContext _context;

        public ClientManager(OrderControllAppContext context)
        {
            _context = context;
        }
        public async Task<Either<string, Client>> GetClientByNameAsync(string clientName)
        {
            Client needClient = await _context.Clients.Include(c => c.Orders).SingleOrDefaultAsync(c => c.Name == clientName);

            if (needClient == null)
            {
                return $"Клиент {clientName} не найден";
            }

            return needClient;
        }

        public async Task<Either<string, Client>> GetClientByIdAsync(int id)
        {
            Client needClient = await _context.Clients.SingleOrDefaultAsync(c => c.Id == id);

            if (needClient == null)
            {
                return $"Клиент с id = {id} не найден";
            }

            return needClient;
        }

        public async Task<string> AddClientAsync(AddClientDto addClientData)
        {
            var checkedClient = await GetClientByNameAsync(addClientData.Name);

            if (!checkedClient.IsLeft)
            {
                return $"Клиент {addClientData.Name} уже существует";
            }

            Client newClient = new Client
            {
                Name = addClientData.Name,
                Phone = addClientData.Phone,
                Email = addClientData.Email
            };

            await _context.Clients.AddAsync(newClient);

            await _context.SaveChangesAsync();

            return $"Клиент {addClientData.Name} успешно добавлен";
        }

        public async Task<List<GetClientDto>> GetAllClientsAsync()
        {
            List<Client> clients = await _context.Clients.ToListAsync();

            List<GetClientDto> formatClients = new List<GetClientDto>();

            foreach (Client client in clients)
            {
                GetClientDto newClientData = new GetClientDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    Phone = client.Phone
                };

                formatClients.Add(newClientData);
            }

            return formatClients;
        }
    }
}
