namespace MineralKingdomApi.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string token);
}
