using Application.DTO;
using Application.Interfaces;
using Domain;
using Domain.Enum;
using Mapster;

namespace Application.Services;

public class ResourceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ResourceDto>> GetResourcesAsync()
        {
            var resources = await _unitOfWork.Resources.GetAllAsync();
            return resources.Adapt<IEnumerable<ResourceDto>>();
        }
        
        public async Task AddResourceAsync(CreateResourceDto dto)
        {
            var exists = await _unitOfWork.Resources.ExistsWithNameAsync(dto.Name);
            if (exists)
            {
                throw new InvalidOperationException($"Ресурс с именем '{dto.Name}' уже существует.");
            }

            var resource = dto.Adapt<Resource>();
            await _unitOfWork.Resources.AddAsync(resource);
            await _unitOfWork.CompleteAsync();
        }
        
        public async Task UpdateResourceAsync(UpdateResourceDto dto)
        {
            var existingResource = await _unitOfWork.Resources.GetByIdAsync(dto.Id);
            if (existingResource == null)
            {
                throw new InvalidOperationException("Ресурс не найден.");
            }

            if (existingResource.Name != dto.Name)
            {
                var exists = await _unitOfWork.Resources.ExistsWithNameAsync(dto.Name);
                if (exists)
                {
                    throw new InvalidOperationException($"Ресурс с именем '{dto.Name}' уже существует.");
                }
            }
        
            dto.Adapt(existingResource);
            await _unitOfWork.Resources.UpdateAsync(existingResource);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ArchiveResourceAsync(Guid id)
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(id);
            if (resource == null)
            {
                throw new InvalidOperationException("Ресурс не найден.");
            }
            
            var isUsedInIncome = await _unitOfWork.IncomeDocuments.IsResourceUsedAsync(id);
            var isUsedInShipment = await _unitOfWork.ShipmentDocuments.IsResourceUsedAsync(id);

            if (isUsedInIncome || isUsedInShipment)
            {
                resource.State = EntityState.Archived;
                await _unitOfWork.Resources.UpdateAsync(resource);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                await _unitOfWork.Resources.DeleteAsync(id); 
                await _unitOfWork.CompleteAsync();
            }
        }
    }