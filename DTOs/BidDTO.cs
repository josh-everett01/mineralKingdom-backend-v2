public class BidDTO
{
  public int Id { get; set; }
  public decimal Amount { get; set; }
  public DateTime PlacedAt { get; set; }

  // Public info about the bidder
  public int UserId { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string DisplayName { get; set; } = "Anonymous";
}
