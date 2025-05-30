public class UserDTO
{
  public int Id { get; set; }
  public string Email { get; set; } = null!;
  public bool IsVerified { get; set; }
  public string Role { get; set; } = null!;
}
