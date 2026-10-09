using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Data;
using TripSplit.Api.Models;

namespace TripSplit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TripsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
    {
        var trips = await _context.Trips.ToListAsync();
        return Ok(trips);
    }

    // GET: api/trips/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Trip>> GetTrip(int id)
    {
        var trip = await _context.Trips.FindAsync(id);

        if (trip is null)
            return NotFound();

        return Ok(trip);
    }

    // POST: api/trips
    [HttpPost]
    public async Task<ActionResult<Trip>> CreateTrip(Trip trip)
    {
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTrip),
            new { id = trip.Id },
            trip);
    }

    // PUT: api/trips/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTrip(int id, Trip trip)
    {
        if (id != trip.Id)
            return BadRequest();

        var exists = await _context.Trips.AnyAsync(t => t.Id == id);

        if (!exists)
            return NotFound();

        _context.Entry(trip).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/trips/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var trip = await _context.Trips.FindAsync(id);

        if (trip is null)
            return NotFound();

        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}