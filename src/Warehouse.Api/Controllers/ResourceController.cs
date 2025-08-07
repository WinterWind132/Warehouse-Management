using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourceController : ControllerBase
{
    private readonly ResourceService _resourceService;

    public ResourceController(ResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll()
    {
        var resources = await _resourceService.GetResourcesAsync();
        return Ok(resources);
    }

    [HttpPost]
    public async Task<ActionResult> AddResource(CreateResourceDto dto)
    {
        try
        {
            await _resourceService.AddResourceAsync(dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
        
    [HttpPut]
    public async Task<ActionResult> UpdateResource(UpdateResourceDto dto)
    {
        try
        {
            await _resourceService.UpdateResourceAsync(dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> ArchiveResource(Guid id)
    {
        try
        {
            await _resourceService.ArchiveResourceAsync(id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}