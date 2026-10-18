using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Data;
using TripSplit.Api.Models;

namespace TripSplit.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TripsController(
        AppDbContext context)
    {
        _context = context;
    }

    // GET: api/trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trip>>>
        GetTrips()
    {
        var userId =
            GetCurrentUserId();

        var trips =
            await _context.Trips
                .Where(
                    trip =>
                        trip.OwnerUserId ==
                        userId)
                .OrderBy(
                    trip =>
                        trip.StartDate)
                .ToListAsync();

        return Ok(trips);
    }

    // GET: api/trips/2
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Trip>>
        GetTrip(int id)
    {
        var userId =
            GetCurrentUserId();

        var trip =
            await _context.Trips
                .FirstOrDefaultAsync(
                    trip =>
                        trip.Id == id &&
                        trip.OwnerUserId ==
                        userId);

        if (trip is null)
            return NotFound();

        return Ok(trip);
    }

    // POST: api/trips
    [HttpPost]
    public async Task<ActionResult<Trip>>
        CreateTrip(Trip trip)
    {
        var userId =
            GetCurrentUserId();

        trip.Id = 0;

        trip.OwnerUserId =
            userId;

        trip.Name =
            trip.Name?.Trim()
            ?? string.Empty;

        trip.Destination =
            trip.Destination?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(
                trip.Name))
        {
            return BadRequest(
                "Назва подорожі є обов'язковою.");
        }

        if (string.IsNullOrWhiteSpace(
                trip.Destination))
        {
            return BadRequest(
                "Місце призначення є обов'язковим.");
        }

        if (trip.EndDate <
            trip.StartDate)
        {
            return BadRequest(
                "Дата завершення не може бути раніше дати початку.");
        }

        _context.Trips.Add(trip);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTrip),
            new
            {
                id = trip.Id
            },
            trip);
    }

    // PUT: api/trips/2
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        UpdateTrip(
            int id,
            Trip trip)
    {
        var userId =
            GetCurrentUserId();

        var existingTrip =
            await _context.Trips
                .FirstOrDefaultAsync(
                    existing =>
                        existing.Id == id &&
                        existing.OwnerUserId ==
                        userId);

        if (existingTrip is null)
            return NotFound();

        var name =
            trip.Name?.Trim()
            ?? string.Empty;

        var destination =
            trip.Destination?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(
                "Назва подорожі є обов'язковою.");
        }

        if (string.IsNullOrWhiteSpace(
                destination))
        {
            return BadRequest(
                "Місце призначення є обов'язковим.");
        }

        if (trip.EndDate <
            trip.StartDate)
        {
            return BadRequest(
                "Дата завершення не може бути раніше дати початку.");
        }

        existingTrip.Name =
            name;

        existingTrip.Destination =
            destination;

        existingTrip.StartDate =
            trip.StartDate;

        existingTrip.EndDate =
            trip.EndDate;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/trips/2
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteTrip(int id)
    {
        var userId =
            GetCurrentUserId();

        var trip =
            await _context.Trips
                .FirstOrDefaultAsync(
                    trip =>
                        trip.Id == id &&
                        trip.OwnerUserId ==
                        userId);

        if (trip is null)
            return NotFound();

        _context.Trips.Remove(trip);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(
                   ClaimTypes.NameIdentifier)
               ?? throw new InvalidOperationException(
                   "User identifier was not found.");
    }
}