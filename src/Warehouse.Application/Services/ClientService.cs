using Application.DTO;
using Application.Interfaces;
using Domain;
using Domain.Enum;
using Mapster;

namespace Warehouse.Application.Services
{
    public class ClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ClientDto>> GetClientsAsync()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            return clients.Adapt<IEnumerable<ClientDto>>();
        }

        public async Task AddClientAsync(CreateClientDto dto)
        {
            var exists = await _unitOfWork.Clients.ExistsWithNameAsync(dto.Name);
            if (exists)
            {
                throw new InvalidOperationException($"Клиент с именем '{dto.Name}' уже существует.");
            }

            var client = dto.Adapt<Client>();
            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ArchiveClientAsync(Guid id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
            {
                throw new InvalidOperationException("Клиент не найден.");
            }
            var isUsed = await _unitOfWork.ShipmentDocuments.IsClientUsedAsync(id);
            
            if (isUsed)
            {
                client.State = EntityState.Archived;
                await _unitOfWork.Clients.UpdateAsync(client);
            }
            else
            {
                await _unitOfWork.Clients.DeleteAsync(id);
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}