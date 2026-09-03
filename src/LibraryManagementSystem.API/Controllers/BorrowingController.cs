using System.Security.Claims;
using LibraryManagementSystem.Application.DTOs.Borrowing;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("borrowings")]
[Authorize]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;

    public BorrowingController(IBorrowingService borrowingService)
    {
        _borrowingService = borrowingService;
    }

    [HttpPost]
    public async Task<ActionResult<BorrowingDto>> Create(CreateBorrowingRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var borrowing = await _borrowingService.CreateAsync(userId, request);
            return Ok(borrowing);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/return")]
    public async Task<ActionResult<BorrowingDto>> Return(Guid id)
    {
        try
        {
            var borrowing = await _borrowingService.ReturnAsync(id);
            return Ok(borrowing);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (AlreadyReturnedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<List<BorrowingDto>>> GetAll()
    {
        var borrowings = await _borrowingService.GetAllAsync();
        return Ok(borrowings);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return Guid.Parse(userIdClaim!.Value);
    }
}