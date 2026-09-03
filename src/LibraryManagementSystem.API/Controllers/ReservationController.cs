using System.Security.Claims;
using LibraryManagementSystem.Application.DTOs.Reservation;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("reservations")]
[Authorize]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Create(CreateReservationRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var reservation = await _reservationService.CreateAsync(userId, request);
            return Ok(reservation);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DuplicateReservationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ReservationDto>>> GetAll()
    {
        var userId = GetCurrentUserId();
        var reservations = await _reservationService.GetAllAsync(userId);
        return Ok(reservations);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return Guid.Parse(userIdClaim!.Value);
    }
}