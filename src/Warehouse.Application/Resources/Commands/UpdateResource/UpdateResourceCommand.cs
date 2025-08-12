using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.Resources.Commands.UpdateResource;

public record UpdateResourceCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class UpdateResourceCommandHandler : IRequestHandler<UpdateResourceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResourceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            var existingResource = await _unitOfWork.Resources.GetByIdAsync(request.Id);
            if (existingResource == null)
            {
                return Result.Failure("Ресурс не найден.");
            }

            if (existingResource.Name != request.Name)
            {
                var exists = await _unitOfWork.Resources.ExistsWithNameAsync(request.Name);
                if (exists)
                {
                    return Result.Failure($"Ресурс с именем '{request.Name}' уже существует.");
                }
            }

            existingResource.Name = request.Name;
            await _unitOfWork.Resources.UpdateAsync(existingResource);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при обновлении ресурса: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(UpdateResourceCommand command)
    {
        var errors = new List<string>();
        
        if (command.Id == Guid.Empty)
            errors.Add("ID ресурса не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add("Название ресурса не может быть пустым");
        
        if (command.Name.Length > 255)
            errors.Add("Название ресурса не может быть длиннее 255 символов");
        
        return errors;
    }
} 