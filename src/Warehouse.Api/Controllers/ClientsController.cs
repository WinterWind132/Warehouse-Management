using Application.Clients.Commands.ArchiveClient;
using Application.Common;
using Application.DTO;
using Application.Clients.Commands.CreateClient;
using Application.Clients.Commands.UpdateClient;
using Application.Clients.Queries.GetClients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] bool? isActive)
    {
        var query = new GetClientsQuery
        {
            SearchTerm = searchTerm,
            IsActive = isActive
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateClientCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
            {
                return BadRequest(new { Errors = result.ValidationErrors });
            }
            
            return BadRequest(result.Error);
        }
        
        return Ok();
    }
        
    [HttpPut]
    public async Task<ActionResult> Update(UpdateClientCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
            {
                return BadRequest(new { Errors = result.ValidationErrors });
            }
            
            return BadRequest(result.Error);
        }
        
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Archive(Guid id)
    {
        var command = new ArchiveClientCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }
} 