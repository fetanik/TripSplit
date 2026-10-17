using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Data;
using TripSplit.Api.DTOs;

namespace TripSplit.Api.Controllers;

[ApiController]
[Route("api/trips/{tripId:int}/balances")]
public class BalancesController : ControllerBase
{
    private readonly AppDbContext _context;

    public BalancesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/trips/2/balances
    [HttpGet]
    public async Task<ActionResult<BalanceResponse>> GetBalances(
        int tripId)
    {
        var tripExists =
            await _context.Trips.AnyAsync(
                trip => trip.Id == tripId);

        if (!tripExists)
            return NotFound("Подорож не знайдено.");

        var participants = await _context.Participants
            .Where(participant => participant.TripId == tripId)
            .OrderBy(participant => participant.Name)
            .ToListAsync();

        var expenses = await _context.Expenses
            .Where(expense => expense.TripId == tripId)
            .ToListAsync();

        var expenseIds = expenses
            .Select(expense => expense.Id)
            .ToList();

        var shares = expenseIds.Count == 0
            ? new List<Models.ExpenseShare>()
            : await _context.ExpenseShares
                .Where(share =>
                    expenseIds.Contains(share.ExpenseId))
                .ToListAsync();

        var balances =
            new List<ParticipantBalanceResponse>();

        foreach (var participant in participants)
        {
            var paid = expenses
                .Where(expense =>
                    expense.PaidByParticipantId ==
                    participant.Id)
                .Sum(expense => expense.Amount);

            var owed = shares
                .Where(share =>
                    share.ParticipantId ==
                    participant.Id)
                .Sum(share => share.ShareAmount);

            var balance = paid - owed;

            balances.Add(
                new ParticipantBalanceResponse
                {
                    ParticipantId = participant.Id,
                    ParticipantName = participant.Name,
                    Paid = decimal.Round(paid, 2),
                    Owed = decimal.Round(owed, 2),
                    Balance = decimal.Round(balance, 2)
                });
        }

        var settlements =
            CalculateSettlements(balances);

        return Ok(
            new BalanceResponse
            {
                TotalExpenses =
                    decimal.Round(
                        expenses.Sum(
                            expense => expense.Amount),
                        2),

                Balances = balances,

                Settlements = settlements
            });
    }

    private static List<SettlementResponse>
        CalculateSettlements(
            List<ParticipantBalanceResponse> balances)
    {
        var creditors = balances
            .Where(balance => balance.Balance > 0)
            .Select(balance => new BalanceItem
            {
                ParticipantId =
                    balance.ParticipantId,

                ParticipantName =
                    balance.ParticipantName,

                Amount =
                    balance.Balance
            })
            .OrderByDescending(item => item.Amount)
            .ToList();

        var debtors = balances
            .Where(balance => balance.Balance < 0)
            .Select(balance => new BalanceItem
            {
                ParticipantId =
                    balance.ParticipantId,

                ParticipantName =
                    balance.ParticipantName,

                Amount =
                    -balance.Balance
            })
            .OrderByDescending(item => item.Amount)
            .ToList();

        var settlements =
            new List<SettlementResponse>();

        var debtorIndex = 0;
        var creditorIndex = 0;

        while (
            debtorIndex < debtors.Count &&
            creditorIndex < creditors.Count)
        {
            var debtor =
                debtors[debtorIndex];

            var creditor =
                creditors[creditorIndex];

            var amount =
                Math.Min(
                    debtor.Amount,
                    creditor.Amount);

            amount =
                decimal.Round(amount, 2);

            if (amount > 0)
            {
                settlements.Add(
                    new SettlementResponse
                    {
                        FromParticipantId =
                            debtor.ParticipantId,

                        FromParticipantName =
                            debtor.ParticipantName,

                        ToParticipantId =
                            creditor.ParticipantId,

                        ToParticipantName =
                            creditor.ParticipantName,

                        Amount = amount
                    });
            }

            debtor.Amount -= amount;
            creditor.Amount -= amount;

            if (debtor.Amount == 0)
                debtorIndex++;

            if (creditor.Amount == 0)
                creditorIndex++;
        }

        return settlements;
    }

    private class BalanceItem
    {
        public int ParticipantId { get; set; }

        public string ParticipantName { get; set; }
            = string.Empty;

        public decimal Amount { get; set; }
    }
}