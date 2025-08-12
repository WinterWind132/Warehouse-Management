using Application.Common;
using Application.DTO;
using Application.Resources.Commands.ArchiveResource;
using Application.Resources.Commands.CreateResource;
using Application.Resources.Commands.UpdateResource;
using Application.Resources.Queries.GetResources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] bool? isActive)
    {
        var query = new GetResourcesQuery
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
    public async Task<ActionResult> Create([FromBody] string name)
    {
        var command = new CreateResourceCommand { Name = name };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
            {
                return BadRequest(result.ValidationErrors);
            }
            return BadRequest(result.Error);
        }
        
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] string name)
    {
        var command = new UpdateResourceCommand
        {
            Id = id,
            Name = name
        };
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
            {
                return BadRequest(result.ValidationErrors);
            }
            return BadRequest(result.Error);
        }
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Archive(Guid id)
    {
        var command = new ArchiveResourceCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }
}