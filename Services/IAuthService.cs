namespace MineralKingdomApi.Services;

using MineralKingdomApi.Models;

public interface IAuthService
{
  Task<User> RegisterAsync(string email, string password);
  Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password);
  Task VerifyEmailAsync(string token);

  Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
  Task LogoutAsync(string refreshToken);
}
