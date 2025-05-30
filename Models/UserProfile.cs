using MineralKingdomApi.Models;

public class UserProfile
{
  public int Id { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string? DisplayName { get; set; }
  public string? PhoneNumber { get; set; }
  // Shipping Address (nullable until required)
  public string? StreetAddress { get; set; }
  public string? City { get; set; }
  public string? State { get; set; }
  public string? PostalCode { get; set; }
  public string? Country { get; set; }
  // Relationship to User
  public int UserId { get; set; }
  public required User User { get; set; }
}
