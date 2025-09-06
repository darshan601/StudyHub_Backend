
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudyHub.Core.DTOs;
using StudyHub.Core.Entities;
using StudyHub.Core.Interfaces;
using StudyHub.Core.Security;

namespace StudyHub.Core.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        var user = await _userRepository.GetByUsernameAsync(req.UserName);

        if (user == null) return null;

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash)) return null;
        
        //replaced with hashed password verification
        // if (user.PasswordHash != req.Password) return null;

        var token = GenerateJwtToken(user);
        return new AuthResponse(token, Guid.NewGuid().ToString());

    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        var exists = await _userRepository.GetByUsernameAsync(req.UserName);
        if(exists is not null) throw new InvalidOperationException("Username taken");
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = req.UserName,
            PasswordHash = PasswordHasher.Hash(req.Password),
            Role = "student"
        };

        await _userRepository.AddAsync(user);

        var token = GenerateJwtToken(user);

        return new AuthResponse(token, Guid.NewGuid().ToString());
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            // Add explicit unique_name (JWT registered name) so clients see a short 'unique_name' claim
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            
            // Also add a short 'name' key (some clients expect 'name')
            new Claim("name", user.UserName),
            
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            // new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}