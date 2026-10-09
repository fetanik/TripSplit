using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Data;
using TripSplit.Api.Models;

namespace TripSplit.Api.Controllers;

[ApiController]
[Route("api/trips/{tripId:int}/participants")]
public class ParticipantsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParticipantsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/trips/1/participants
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Participant>>> GetParticipants(
        int tripId)
    {
        var tripExists =
            await _context.Trips.AnyAsync(trip => trip.Id == tripId);

        if (!tripExists)
            return NotFound("Подорож не знайдено.");

        var participants = await _context.Participants
            .Where(participant => participant.TripId == tripId)
            .OrderBy(participant => participant.Name)
            .ToListAsync();

        return Ok(participants);
    }

    // POST: api/trips/1/participants
    [HttpPost]
    public async Task<ActionResult<Participant>> CreateParticipant(
        int tripId,
        Participant participant)
    {
        var tripExists =
            await _context.Trips.AnyAsync(trip => trip.Id == tripId);

        if (!tripExists)
            return NotFound("Подорож не знайдено.");

        if (string.IsNullOrWhiteSpace(participant.Name))
            return BadRequest("Ім'я учасника є обов'язковим.");

        participant.Id = 0;
        participant.TripId = tripId;
        participant.Name = participant.Name.Trim();

        _context.Participants.Add(participant);

        await _context.SaveChangesAsync();

        return Created(
            $"api/trips/{tripId}/participants/{participant.Id}",
            participant);
    }

    // DELETE: api/trips/1/participants/5
    [HttpDelete("{participantId:int}")]
    public async Task<IActionResult> DeleteParticipant(
        int tripId,
        int participantId)
    {
        var participant = await _context.Participants
            .FirstOrDefaultAsync(
                participant =>
                    participant.Id == participantId &&
                    participant.TripId == tripId);

        if (participant is null)
            return NotFound();

        _context.Participants.Remove(participant);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}