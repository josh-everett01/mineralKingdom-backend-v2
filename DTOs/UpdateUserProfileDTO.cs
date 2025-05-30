public class UpdateUserProfileDTO
{
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string? DisplayName { get; set; }
  public string? PhoneNumber { get; set; }

  public string? StreetAddress { get; set; }
  public string? City { get; set; }
  public string? State { get; set; }
  public string? PostalCode { get; set; }
  public string? Country { get; set; }
}
