using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.UnitOfMeasures.Commands.UpdateUnitOfMeasure;

public record UpdateUnitOfMeasureCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class UpdateUnitOfMeasureCommandHandler : IRequestHandler<UpdateUnitOfMeasureCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUnitOfMeasureCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            var existingUnitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(request.Id);
            if (existingUnitOfMeasure == null)
            {
                return Result.Failure("Единица измерения не найдена.");
            }

            if (existingUnitOfMeasure.Name != request.Name)
            {
                var exists = await _unitOfWork.UnitOfMeasures.ExistsWithNameAsync(request.Name);
                if (exists)
                {
                    return Result.Failure($"Единица измерения с именем '{request.Name}' уже существует.");
                }
            }

            existingUnitOfMeasure.Name = request.Name;
            await _unitOfWork.UnitOfMeasures.UpdateAsync(existingUnitOfMeasure);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при обновлении единицы измерения: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(UpdateUnitOfMeasureCommand command)
    {
        var errors = new List<string>();
        
        if (command.Id == Guid.Empty)
            errors.Add("ID единицы измерения не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add("Название единицы измерения не может быть пустым");
        
        if (command.Name.Length > 255)
            errors.Add("Название единицы измерения не может быть длиннее 255 символов");
        
        return errors;
    }
} 