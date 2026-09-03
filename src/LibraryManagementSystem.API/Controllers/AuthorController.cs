using LibraryManagementSystem.Application.DTOs.Author;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("authors")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> GetAll()
    {
        var authors = await _authorService.GetAllAsync();
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetById(Guid id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author is null) return NotFound();

        return Ok(author);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorRequest request)
    {
        var author = await _authorService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<AuthorDto>> Update(Guid id, UpdateAuthorRequest request)
    {
        var author = await _authorService.UpdateAsync(id, request);
        if (author is null) return NotFound();

        return Ok(author);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _authorService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}