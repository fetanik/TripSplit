using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TripSplit.Api.DTOs;
using TripSplit.Api.Models;

namespace TripSplit.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly IConfiguration
        _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>>
        Register(RegisterRequest request)
    {
        var displayName =
            request.DisplayName?.Trim()
            ?? string.Empty;

        var email =
            request.Email?.Trim()
                .ToLowerInvariant()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return BadRequest(
                "Введіть ім'я користувача.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(
                "Введіть електронну адресу.");
        }

        if (string.IsNullOrWhiteSpace(
                request.Password))
        {
            return BadRequest(
                "Введіть пароль.");
        }

        var existingUser =
            await _userManager
                .FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return BadRequest(
                "Користувач із такою електронною адресою вже існує.");
        }

        var user =
            new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName
            };

        var result =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!result.Succeeded)
        {
            var errors =
                string.Join(
                    Environment.NewLine,
                    result.Errors.Select(
                        error =>
                            error.Description));

            return BadRequest(errors);
        }

        return Ok(
            CreateAuthResponse(user));
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>>
        Login(LoginRequest request)
    {
        var email =
            request.Email?.Trim()
                .ToLowerInvariant()
            ?? string.Empty;

        var user =
            await _userManager
                .FindByEmailAsync(email);

        if (user is null)
        {
            return Unauthorized(
                "Неправильна електронна адреса або пароль.");
        }

        var passwordValid =
            await _userManager
                .CheckPasswordAsync(
                    user,
                    request.Password);

        if (!passwordValid)
        {
            return Unauthorized(
                "Неправильна електронна адреса або пароль.");
        }

        return Ok(
            CreateAuthResponse(user));
    }

    private AuthResponse CreateAuthResponse(
        ApplicationUser user)
    {
        return new AuthResponse
        {
            UserId = user.Id,

            DisplayName =
                user.DisplayName,

            Email =
                user.Email
                ?? string.Empty,

            Token =
                CreateToken(user)
        };
    }

    private string CreateToken(
        ApplicationUser user)
    {
        var jwtKey =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key was not configured.");

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? "TripSplit.Api";

        var audience =
            _configuration["Jwt:Audience"]
            ?? "TripSplit.Mobile";

        var signingKey =
            new SymmetricSecurityKey(
                Convert.FromBase64String(
                    jwtKey));

        var credentials =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new(
                    ClaimTypes.Email,
                    user.Email
                    ?? string.Empty),

                new(
                    ClaimTypes.Name,
                    user.DisplayName)
            };

        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires:
                    DateTime.UtcNow.AddDays(7),
                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}