using Application.Common;
using Application.DTO;
using Application.ShipmentDocuments.Commands.CreateShipmentDocument;
using Application.ShipmentDocuments.Commands.UpdateShipmentDocument;
using Application.ShipmentDocuments.Commands.SignShipmentDocument;
using Application.ShipmentDocuments.Commands.RevokeShipmentDocument;
using Application.ShipmentDocuments.Queries.GetShipmentDocuments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentDocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShipmentDocumentDto>>> GetAll(
        [FromQuery] DateTime? fromDate, 
        [FromQuery] DateTime? toDate,
        [FromQuery] List<string>? documentNumbers,
        [FromQuery] List<Guid>? resourceIds,
        [FromQuery] List<Guid>? unitOfMeasureIds)
    {
        var query = new GetShipmentDocumentsQuery
        {
            FromDate = fromDate,
            ToDate = toDate,
            DocumentNumbers = documentNumbers ?? new List<string>(),
            ResourceIds = resourceIds ?? new List<Guid>(),
            UnitOfMeasureIds = unitOfMeasureIds ?? new List<Guid>()
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateShipmentDocumentCommand command)
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
    public async Task<ActionResult> Update(UpdateShipmentDocumentCommand command)
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

    [HttpPost("{id:guid}/sign")]
    public async Task<ActionResult> Sign(Guid id)
    {
        var command = new SignShipmentDocumentCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }

    [HttpPost("{id:guid}/revoke")]
    public async Task<ActionResult> Revoke(Guid id)
    {
        var command = new RevokeShipmentDocumentCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }
} 