using Application.Common;
using Application.DTO;
using Application.Resources.Commands.CreateResource;
using Application.Resources.Commands.UpdateResource;
using Application.Resources.Queries.GetResources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourcesController(IMediator mediator)
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
    public async Task<ActionResult> Create(CreateResourceCommand command)
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
    public async Task<ActionResult> Update(UpdateResourceCommand command)
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
} 