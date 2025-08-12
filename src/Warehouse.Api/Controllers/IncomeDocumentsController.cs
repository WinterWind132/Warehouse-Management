using Application.Common;
using Application.DTO;
using Application.IncomeDocuments.Commands.CreateIncomeDocument;
using Application.IncomeDocuments.Commands.UpdateIncomeDocument;
using Application.IncomeDocuments.Commands.DeleteIncomeDocument;
using Application.IncomeDocuments.Queries.GetIncomeDocuments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomeDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public IncomeDocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncomeDocumentDto>>> GetAll(
        [FromQuery] DateTime? fromDate, 
        [FromQuery] DateTime? toDate,
        [FromQuery] List<string>? documentNumbers,
        [FromQuery] List<Guid>? resourceIds,
        [FromQuery] List<Guid>? unitOfMeasureIds)
    {
        var query = new GetIncomeDocumentsQuery
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
    public async Task<ActionResult> Create(CreateIncomeDocumentCommand command)
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
    public async Task<ActionResult> Update(UpdateIncomeDocumentCommand command)
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
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteIncomeDocumentCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }
} 