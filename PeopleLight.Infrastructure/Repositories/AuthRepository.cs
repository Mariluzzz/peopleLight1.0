using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeopleLight.Application.DTOs.Auth;
using PeopleLight.Application.Interfaces;
using PeopleLight.Domain.Entities;
using PeopleLight.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PeopleLight.Infrastructure.Repositories;
public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthRepository(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task RegisterAsync(RegisterUserDto request)
    {
        var mensagem = await UserExistsAsync(request.Username, request.Email);
        if (!string.IsNullOrWhiteSpace(mensagem))
            throw new Exception(mensagem);

        var hash = ComputeSha256Hash(request.Password);
        var user = new User(request.Username, hash, request.Email);
        _context.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var hash = ComputeSha256Hash(request.Password);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == hash);

        if (user == null)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos");

        return GenerateJwtToken(user);
    }

    private LoginResponseDto GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("id", user.Id.ToString())
        };

        var expire = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"]));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expire,
            signingCredentials: creds
        );

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Username = user.Username,
            Expiration = expire
        };
    }

    private static string ComputeSha256Hash(string rawData)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes);
    }

    private async Task<string> UserExistsAsync(string username, string email)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username)) return "Username já está em uso";
        if (await _context.Users.AnyAsync(u => u.Email == email)) return "Email já está em uso";

        return "";
    }

}