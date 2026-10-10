using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Data;
using TripSplit.Api.DTOs;
using TripSplit.Api.Models;

namespace TripSplit.Api.Controllers;

[ApiController]
[Route("api/trips/{tripId:int}/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpensesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/trips/2/expenses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetExpenses(
        int tripId)
    {
        var tripExists =
            await _context.Trips.AnyAsync(trip => trip.Id == tripId);

        if (!tripExists)
            return NotFound("Подорож не знайдено.");

        var expenses = await _context.Expenses
            .Where(expense => expense.TripId == tripId)
            .OrderByDescending(expense => expense.Date)
            .ThenByDescending(expense => expense.Id)
            .ToListAsync();

        if (expenses.Count == 0)
            return Ok(new List<ExpenseResponse>());

        var participants = await _context.Participants
            .Where(participant => participant.TripId == tripId)
            .ToDictionaryAsync(
                participant => participant.Id,
                participant => participant.Name);

        var expenseIds = expenses
            .Select(expense => expense.Id)
            .ToList();

        var shares = await _context.ExpenseShares
            .Where(share => expenseIds.Contains(share.ExpenseId))
            .ToListAsync();

        var result = expenses.Select(expense =>
            new ExpenseResponse
            {
                Id = expense.Id,
                TripId = expense.TripId,
                Title = expense.Title,
                Amount = expense.Amount,
                PaidByParticipantId = expense.PaidByParticipantId,

                PaidByName =
                    participants.GetValueOrDefault(
                        expense.PaidByParticipantId,
                        "Невідомий учасник"),

                Date = expense.Date,
                Note = expense.Note,

                Shares = shares
                    .Where(share => share.ExpenseId == expense.Id)
                    .Select(share =>
                        new ExpenseShareResponse
                        {
                            ParticipantId = share.ParticipantId,

                            ParticipantName =
                                participants.GetValueOrDefault(
                                    share.ParticipantId,
                                    "Невідомий учасник"),

                            ShareAmount = share.ShareAmount
                        })
                    .ToList()
            })
            .ToList();

        return Ok(result);
    }

    // POST: api/trips/2/expenses
    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateExpense(
        int tripId,
        CreateExpenseRequest request)
    {
        var tripExists =
            await _context.Trips.AnyAsync(trip => trip.Id == tripId);

        if (!tripExists)
            return NotFound("Подорож не знайдено.");

        var title = request.Title.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Назва витрати є обов'язковою.");

        var amount = decimal.Round(
            request.Amount,
            2,
            MidpointRounding.AwayFromZero);

        if (amount <= 0)
            return BadRequest("Сума витрати повинна бути більшою за нуль.");

        var participantIds = request.ParticipantIds
            .Distinct()
            .ToList();

        if (participantIds.Count == 0)
            return BadRequest(
                "Оберіть хоча б одного учасника для розподілу витрати.");

        var tripParticipants = await _context.Participants
            .Where(participant => participant.TripId == tripId)
            .ToListAsync();

        var payer = tripParticipants
            .FirstOrDefault(
                participant =>
                    participant.Id == request.PaidByParticipantId);

        if (payer is null)
            return BadRequest(
                "Платник не належить до цієї подорожі.");

        var validParticipantIds = tripParticipants
            .Select(participant => participant.Id)
            .ToHashSet();

        if (participantIds.Any(
                participantId =>
                    !validParticipantIds.Contains(participantId)))
        {
            return BadRequest(
                "Один або кілька учасників не належать до цієї подорожі.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var expense = new Expense
        {
            TripId = tripId,
            Title = title,
            Amount = amount,
            PaidByParticipantId = request.PaidByParticipantId,
            Date = request.Date,
            Note = request.Note.Trim()
        };

        _context.Expenses.Add(expense);

        await _context.SaveChangesAsync();

        var shareAmounts =
            SplitAmount(amount, participantIds.Count);

        var shares = new List<ExpenseShare>();

        for (var i = 0; i < participantIds.Count; i++)
        {
            shares.Add(new ExpenseShare
            {
                ExpenseId = expense.Id,
                ParticipantId = participantIds[i],
                ShareAmount = shareAmounts[i]
            });
        }

        _context.ExpenseShares.AddRange(shares);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        var participantNames = tripParticipants
            .ToDictionary(
                participant => participant.Id,
                participant => participant.Name);

        var result = new ExpenseResponse
        {
            Id = expense.Id,
            TripId = expense.TripId,
            Title = expense.Title,
            Amount = expense.Amount,
            PaidByParticipantId = expense.PaidByParticipantId,
            PaidByName = payer.Name,
            Date = expense.Date,
            Note = expense.Note,

            Shares = shares.Select(share =>
                new ExpenseShareResponse
                {
                    ParticipantId = share.ParticipantId,

                    ParticipantName =
                        participantNames[share.ParticipantId],

                    ShareAmount = share.ShareAmount
                })
                .ToList()
        };

        return Created(
            $"api/trips/{tripId}/expenses/{expense.Id}",
            result);
    }

    // DELETE: api/trips/2/expenses/1
    [HttpDelete("{expenseId:int}")]
    public async Task<IActionResult> DeleteExpense(
        int tripId,
        int expenseId)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(
                expense =>
                    expense.Id == expenseId &&
                    expense.TripId == tripId);

        if (expense is null)
            return NotFound();

        _context.Expenses.Remove(expense);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static List<decimal> SplitAmount(
        decimal amount,
        int participantCount)
    {
        var totalCents = (long)decimal.Round(
            amount * 100m,
            0,
            MidpointRounding.AwayFromZero);

        var baseCents = totalCents / participantCount;
        var remainder = totalCents % participantCount;

        var result = new List<decimal>();

        for (var i = 0; i < participantCount; i++)
        {
            var cents =
                baseCents + (i < remainder ? 1 : 0);

            result.Add(cents / 100m);
        }

        return result;
    }
}