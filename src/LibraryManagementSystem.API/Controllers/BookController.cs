using LibraryManagementSystem.Application.DTOs.Book;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("books")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookDto>>> GetAll([FromQuery] BookQueryParameters parameters)
    {
        var result = await _bookService.GetAllAsync(parameters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null) return NotFound();

        return Ok(book);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<BookDto>> Create(CreateBookRequest request)
    {
        var book = await _bookService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<BookDto>> Update(Guid id, UpdateBookRequest request)
    {
        var book = await _bookService.UpdateAsync(id, request);
        if (book is null) return NotFound();

        return Ok(book);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _bookService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}