namespace StudyHub.Core.DTOs;

public record LoginRequest(string UserName, string Password);
public record RegisterRequest(string UserName, string Password);
public record AuthResponse(string Token, string RefreshToken);