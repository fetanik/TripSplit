using Microsoft.AspNetCore.Identity;

namespace TripSplit.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}