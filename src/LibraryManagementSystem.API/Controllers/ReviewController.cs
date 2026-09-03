using System.Security.Claims;
using LibraryManagementSystem.Application.DTOs.Review;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var review = await _reviewService.CreateAsync(userId, request);
            return Ok(review);
        }
        catch (InvalidRatingException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DuplicateReviewException ex)
        {
            return Conflict(new { message = ex.Message }); // 409
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ReviewDto>>> GetAll([FromQuery] Guid? bookId)
    {
        var reviews = await _reviewService.GetAllAsync(bookId);
        return Ok(reviews);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return Guid.Parse(userIdClaim!.Value);
    }
}