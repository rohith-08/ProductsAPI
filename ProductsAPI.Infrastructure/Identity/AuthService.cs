using Microsoft.EntityFrameworkCore;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Domain.Exceptions;
using ProductsAPI.Infrastructure.Data;

namespace ProductsAPI.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;

    public AuthService(ApplicationDbContext context, JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new DomainException("Email is already registered.");

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new DomainException("Username is already taken.");

        var user = new AppUser
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User",
            CreatedOn = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new DomainException("Invalid email or password.");

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new DomainException("Invalid or expired refresh token.");

        return await GenerateTokensAsync(user);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null)
            throw new DomainException("Invalid refresh token.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateTokensAsync(AppUser user)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiryMinutes = 60;

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Username = user.Username,
            Role = user.Role
        };
    }
}

