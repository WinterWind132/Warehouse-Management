using Application.Common;
using Application.DTO;
using Application.Balances.Queries.GetBalances;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BalancesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BalancesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BalanceDto>>> GetAll([FromQuery] Guid? resourceId, [FromQuery] Guid? unitOfMeasureId)
    {
        var query = new GetBalancesQuery
        {
            ResourceId = resourceId,
            UnitOfMeasureId = unitOfMeasureId
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok(result.Value);
    }
} 