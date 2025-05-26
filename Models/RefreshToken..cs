namespace MineralKingdomApi.Models;

public class RefreshToken
{
  public int Id { get; set; }

  public string TokenHash { get; set; } = string.Empty;  // Hashed version of the token

  public DateTime Expires { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public string IpAddress { get; set; } = string.Empty;

  public string UserAgent { get; set; } = string.Empty;

  public string DeviceName { get; set; } = string.Empty;

  public bool Revoked { get; set; } = false;

  public int UserId { get; set; }

  public User User { get; set; } = null!;
}

