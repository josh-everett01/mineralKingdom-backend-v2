namespace MineralKingdomApi.Models.Auth;

public class EmailVerificationToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; } = false;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
