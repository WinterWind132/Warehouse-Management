using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces;
using Domain;
using Domain.Enum;

namespace Warehouse.Application.Services
{
    public class UnitOfMeasureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfMeasureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UnitOfMeasureDto>> GetUnitOfMeasuresAsync()
        {
            var units = await _unitOfWork.UnitOfMeasures.GetAllAsync();
            return units.Adapt<IEnumerable<UnitOfMeasureDto>>();
        }

        public async Task AddUnitOfMeasureAsync(CreateUnitOfMeasureDto dto)
        {
            var exists = await _unitOfWork.UnitOfMeasures.ExistsWithNameAsync(dto.Name);
            if (exists)
            {
                throw new InvalidOperationException($"Единица измерения с именем '{dto.Name}' уже существует.");
            }

            var unit = dto.Adapt<UnitOfMeasure>();
            await _unitOfWork.UnitOfMeasures.AddAsync(unit);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ArchiveUnitOfMeasureAsync(Guid id)
        {
            var unit = await _unitOfWork.UnitOfMeasures.GetByIdAsync(id);
            if (unit == null)
            {
                throw new InvalidOperationException("Единица измерения не найдена.");
            }

            var isUsedInIncome = await _unitOfWork.IncomeDocuments.IsUnitOfMeasureUsedAsync(id);
            var isUsedInShipment = await _unitOfWork.ShipmentDocuments.IsUnitOfMeasureUsedAsync(id);
            
            if (isUsedInIncome || isUsedInShipment)
            {
                unit.State = EntityState.Archived;
                await _unitOfWork.UnitOfMeasures.UpdateAsync(unit);
            }
            else
            {
                await _unitOfWork.UnitOfMeasures.DeleteAsync(id);
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}