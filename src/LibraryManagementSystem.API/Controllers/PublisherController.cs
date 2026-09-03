using LibraryManagementSystem.Application.DTOs.Publisher;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("publishers")]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublisherController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PublisherDto>>> GetAll()
    {
        var publishers = await _publisherService.GetAllAsync();
        return Ok(publishers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PublisherDto>> GetById(Guid id)
    {
        var publisher = await _publisherService.GetByIdAsync(id);
        if (publisher is null) return NotFound();

        return Ok(publisher);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<PublisherDto>> Create(CreatePublisherRequest request)
    {
        var publisher = await _publisherService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = publisher.Id }, publisher);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<PublisherDto>> Update(Guid id, UpdatePublisherRequest request)
    {
        var publisher = await _publisherService.UpdateAsync(id, request);
        if (publisher is null) return NotFound();

        return Ok(publisher);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _publisherService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}