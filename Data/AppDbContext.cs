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
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Bid> Bids { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>()
      .HasIndex(u => u.Email)
      .IsUnique(); // Email is often used for login — and should be unique

      modelBuilder.Entity<RefreshToken>()
      .HasIndex(rt => rt.TokenHash); // For fast lookup during refresh

      modelBuilder.Entity<EmailVerificationToken>()
      .HasIndex(evt => evt.Token); // For fast lookup during verification

      modelBuilder.Entity<Order>()
      .HasIndex(o => o.UserId); // Speed up lookups like: "all orders by user"

      modelBuilder.Entity<Bid>()
      .HasIndex(b => b.UserId); // Same reason: user activity
                                // .HasIndex(b => b.AuctionId); // Add when Auctions table exists


      modelBuilder.Entity<User>()
          .HasOne(u => u.Profile)
          .WithOne(p => p.User)
          .HasForeignKey<UserProfile>(p => p.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Order>()
          .HasOne(o => o.User)
          .WithMany()  // Optional: .WithMany(u => u.Orders) if you track in User model
          .HasForeignKey(o => o.UserId);

      modelBuilder.Entity<Bid>()
          .HasOne(b => b.User)
          .WithMany()  // Optional: .WithMany(u => u.Bids)
          .HasForeignKey(b => b.UserId);
    }
  }
}
