using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            var existingClient = await _unitOfWork.Clients.GetByIdAsync(request.Id);
            if (existingClient == null)
            {
                return Result.Failure("Клиент не найден.");
            }

            if (existingClient.Name != request.Name)
            {
                var exists = await _unitOfWork.Clients.ExistsWithNameAsync(request.Name);
                if (exists)
                {
                    return Result.Failure($"Клиент с именем '{request.Name}' уже существует.");
                }
            }

            existingClient.Name = request.Name;
            existingClient.Address = request.Address;
            await _unitOfWork.Clients.UpdateAsync(existingClient);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при обновлении клиента: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(UpdateClientCommand command)
    {
        var errors = new List<string>();
        
        if (command.Id == Guid.Empty)
            errors.Add("ID клиента не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add("Название клиента не может быть пустым");
        
        if (command.Name.Length > 255)
            errors.Add("Название клиента не может быть длиннее 255 символов");
        
        if (string.IsNullOrWhiteSpace(command.Address))
            errors.Add("Адрес клиента не может быть пустым");
        
        if (command.Address.Length > 500)
            errors.Add("Адрес клиента не может быть длиннее 500 символов");
        
        return errors;
    }
} 