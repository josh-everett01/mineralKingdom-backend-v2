using MineralKingdomApi.Models.Auth;

namespace MineralKingdomApi.Models;

public class User
{
  public int Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public string Role { get; set; } = "User";

  public bool IsVerified { get; set; } = false;

  public List<RefreshToken> RefreshTokens { get; set; } = new();

  public List<EmailVerificationToken> EmailVerificationTokens { get; set; } = new();
}
