using Application.DTO;
using Application.Interfaces;
using Domain;
using Mapster;

namespace Application.Services;

public class IncomeDocumentService
{
    private readonly IUnitOfWork _unitOfWork;

    public IncomeDocumentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddIncomeDocumentAsync(CreateIncomeDocumentDto dto)
    {
        if (await _unitOfWork.IncomeDocuments.ExistsWithNumberAsync(dto.DocumentNumber))
        {
            throw new InvalidOperationException("Документ с таким номером уже существует.");
        }

        var document = dto.Adapt<IncomeDocument>();
        await _unitOfWork.IncomeDocuments.AddAsync(document);

        foreach (var item in document.IncomeResources)
        {
            await _unitOfWork.Balances.AddOrUpdateAsync(item.ResourceId, item.UnitOfMeasureId, item.Quantity);
        }
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteIncomeDocumentAsync(Guid id)
    {
        var document = await _unitOfWork.IncomeDocuments.GetByIdAsync(id);
        if (document == null)
        {
            throw new InvalidOperationException("Документ не найден.");
        }
        
        foreach (var item in document.IncomeResources)
        {
            var balance = await _unitOfWork.Balances.GetByResourceAndUnitAsync(item.ResourceId, item.UnitOfMeasureId);
            if (balance == null || balance.Quantity < item.Quantity)
            {
                throw new InvalidOperationException("Недостаточно ресурсов на складе для отмены документа поступления.");
            }
        }
        foreach (var item in document.IncomeResources)
        {
            await _unitOfWork.Balances.AddOrUpdateAsync(item.ResourceId, item.UnitOfMeasureId, -item.Quantity);
        }

        await _unitOfWork.IncomeDocuments.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }
}