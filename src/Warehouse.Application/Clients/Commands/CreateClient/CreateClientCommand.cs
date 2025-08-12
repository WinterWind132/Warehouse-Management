using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.Clients.Commands.CreateClient;

public record CreateClientCommand : IRequest<Result>
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            var exists = await _unitOfWork.Clients.ExistsWithNameAsync(request.Name);
            if (exists)
            {
                return Result.Failure($"Клиент с именем '{request.Name}' уже существует.");
            }

            var client = new Domain.Client
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                State = Domain.Enum.EntityState.Active
            };

            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при создании клиента: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(CreateClientCommand command)
    {
        var errors = new List<string>();
        
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