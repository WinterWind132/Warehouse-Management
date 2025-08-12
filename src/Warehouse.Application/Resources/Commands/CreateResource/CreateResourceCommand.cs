using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.Resources.Commands.CreateResource;

public record CreateResourceCommand : IRequest<Result>
{
    public string Name { get; init; } = string.Empty;
}

public class CreateResourceCommandHandler : IRequestHandler<CreateResourceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateResourceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _unitOfWork.Resources.ExistsWithNameAsync(request.Name);
            if (exists)
            {
                return Result.Failure($"Ресурс с именем '{request.Name}' уже существует.");
            }

            var resource = new Domain.Resource
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                State = Domain.Enum.EntityState.Active
            };

            await _unitOfWork.Resources.AddAsync(resource);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при создании ресурса: {ex.Message}");
        }
    }
} 