using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Models;
using MineralKingdomApi.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace MineralKingdomApi.Services;

public class AuthService : IAuthService
{
  private readonly AppDbContext _db;
  private readonly IConfiguration _config;
  private readonly IEmailService _emailService;
  private readonly IHttpContextAccessor _httpContext;

  public AuthService(AppDbContext db, IConfiguration config, IEmailService emailService, IHttpContextAccessor httpContext)
  {
    _db = db;
    _config = config;
    _emailService = emailService;
    _httpContext = httpContext;
  }

  public async Task<User> RegisterAsync(string email, string password)
  {
    if (string.IsNullOrWhiteSpace(email))
      throw new ArgumentException("Email is required", nameof(email));

    if (string.IsNullOrWhiteSpace(password))
      throw new ArgumentException("Password is required", nameof(password));
    if (await _db.Users.AnyAsync(u => u.Email == email))
      throw new Exception("User already exists");

    var user = new User
    {
      Email = email,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
      IsVerified = false
    };

    _db.Users.Add(user);

    var verificationToken = new EmailVerificationToken
    {
      Token = Guid.NewGuid().ToString(),
      ExpiresAt = DateTime.UtcNow.AddDays(1),
      User = user
    };

    _db.EmailVerificationTokens.Add(verificationToken);

    await _db.SaveChangesAsync();
    Console.WriteLine($"📬 Email going to: {user.Email}");

    await _emailService.SendVerificationEmailAsync(user.Email, verificationToken.Token);

    return user;
  }

  public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password)
  {
    var user = await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
      throw new Exception("Invalid credentials");
    if (!user.IsVerified)
      throw new Exception("Email not verified");

    var rawToken = Guid.NewGuid().ToString();
    var refreshToken = GenerateRefreshToken(rawToken);

    // Remove expired tokens and enforce a limit
    user.RefreshTokens.RemoveAll(t => t.Expires < DateTime.UtcNow);
    if (user.RefreshTokens.Count >= 5)
    {
      var oldest = user.RefreshTokens.OrderBy(t => t.CreatedAt).First();
      _db.RefreshTokens.Remove(oldest);
    }

    user.RefreshTokens.Add(refreshToken);
    await _db.SaveChangesAsync();

    var accessToken = GenerateJwtToken(user);
    return (accessToken, rawToken);
  }

  public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string incomingRawToken)
  {
    if (string.IsNullOrWhiteSpace(incomingRawToken))
      throw new Exception("Token is missing");

    var tokens = await _db.RefreshTokens
        .Include(t => t.User)
        .Where(t => !t.Revoked && t.Expires > DateTime.UtcNow)
        .ToListAsync();

    var storedToken = tokens.FirstOrDefault(t =>
        !string.IsNullOrEmpty(t.TokenHash) &&
        BCrypt.Net.BCrypt.Verify(incomingRawToken, t.TokenHash));

    if (storedToken == null)
      throw new Exception("Invalid or expired refresh token");

    storedToken.Revoked = true;

    var user = storedToken.User;

    var newRawToken = Guid.NewGuid().ToString();
    var newRefreshToken = GenerateRefreshToken(newRawToken);

    // Remove expired and enforce limit
    user.RefreshTokens.RemoveAll(t => t.Expires < DateTime.UtcNow);
    if (user.RefreshTokens.Count >= 5)
    {
      var oldest = user.RefreshTokens.OrderBy(t => t.CreatedAt).First();
      _db.RefreshTokens.Remove(oldest);
    }

    user.RefreshTokens.Add(newRefreshToken);
    await _db.SaveChangesAsync();

    var newAccessToken = GenerateJwtToken(user);
    return (newAccessToken, newRawToken);
  }

  public async Task LogoutAsync(string rawToken)
  {
    var tokens = await _db.RefreshTokens
        .Where(t => !t.Revoked)
        .ToListAsync();

    var token = tokens.FirstOrDefault(t => BCrypt.Net.BCrypt.Verify(rawToken, t.TokenHash));
    if (token == null)
      throw new Exception("Token not found");

    token.Revoked = true;
    await _db.SaveChangesAsync();
  }

  public async Task VerifyEmailAsync(string token)
  {
    var record = await _db.EmailVerificationTokens
        .Include(t => t.User)
        .FirstOrDefaultAsync(t => t.Token == token && !t.Used && t.ExpiresAt > DateTime.UtcNow);

    if (record == null)
      throw new Exception("Invalid or expired token");

    record.Used = true;
    record.User.IsVerified = true;

    await _db.SaveChangesAsync();
  }

  private string GenerateJwtToken(User user)
  {
    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new Exception("JWT key missing"));
    var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  private RefreshToken GenerateRefreshToken(string rawToken)
  {
    if (string.IsNullOrWhiteSpace(rawToken))
      throw new ArgumentException("Cannot generate a hashed token from an empty string");

    var context = _httpContext.HttpContext!;
    return new RefreshToken
    {
      TokenHash = BCrypt.Net.BCrypt.HashPassword(rawToken),
      Expires = DateTime.UtcNow.AddDays(7),
      CreatedAt = DateTime.UtcNow,
      IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
      UserAgent = context.Request.Headers["User-Agent"].ToString(),
      DeviceName = context.Request.Headers["Device-Name"].ToString() ?? "unknown"
    };
  }
}

