using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.UnitOfMeasures.Commands.CreateUnitOfMeasure;

public record CreateUnitOfMeasureCommand : IRequest<Result>
{
    public string Name { get; init; } = string.Empty;
}

public class CreateUnitOfMeasureCommandHandler : IRequestHandler<CreateUnitOfMeasureCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUnitOfMeasureCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _unitOfWork.UnitOfMeasures.ExistsWithNameAsync(request.Name);
            if (exists)
            {
                return Result.Failure($"Единица измерения с именем '{request.Name}' уже существует.");
            }

            var unitOfMeasure = new Domain.UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                State = Domain.Enum.EntityState.Active
            };

            await _unitOfWork.UnitOfMeasures.AddAsync(unitOfMeasure);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при создании единицы измерения: {ex.Message}");
        }
    }
} 