using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Models;
using MineralKingdomApi.Models.Auth;


namespace MineralKingdomApi.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

  }
}
