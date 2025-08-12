using Application.Common;
using Application.DTO;
using Application.UnitOfMeasures.Commands.ArchiveUnitOfMeasure;
using Application.UnitOfMeasures.Commands.CreateUnitOfMeasure;
using Application.UnitOfMeasures.Commands.UpdateUnitOfMeasure;
using Application.UnitOfMeasures.Queries.GetUnitOfMeasures;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitOfMeasuresController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitOfMeasuresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitOfMeasureDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] bool? isActive)
    {
        var query = new GetUnitOfMeasuresQuery
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
        var command = new CreateUnitOfMeasureCommand { Name = name };
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
        var command = new UpdateUnitOfMeasureCommand
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
        var command = new ArchiveUnitOfMeasureCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }
} 