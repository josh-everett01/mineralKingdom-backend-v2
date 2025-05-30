using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using System.Security.Claims;

namespace MineralKingdomApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfileController : ControllerBase
{
  private readonly AppDbContext _context;

  public UserProfileController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet("me")]
  public async Task<IActionResult> GetProfile()
  {
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    if (profile == null) return NotFound(new { error = "Profile not found." });

    var dto = new UserProfileDTO
    {
      FirstName = profile.FirstName,
      LastName = profile.LastName,
      DisplayName = profile.DisplayName,
      PhoneNumber = profile.PhoneNumber,
      StreetAddress = profile.StreetAddress,
      City = profile.City,
      State = profile.State,
      PostalCode = profile.PostalCode,
      Country = profile.Country
    };

    return Ok(dto);
  }

  [HttpPost("update")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDTO updateDto)
  {
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    if (profile == null) return NotFound(new { error = "Profile not found." });

    profile.FirstName = updateDto.FirstName;
    profile.LastName = updateDto.LastName;
    profile.DisplayName = updateDto.DisplayName;
    profile.PhoneNumber = updateDto.PhoneNumber;
    profile.StreetAddress = updateDto.StreetAddress;
    profile.City = updateDto.City;
    profile.State = updateDto.State;
    profile.PostalCode = updateDto.PostalCode;
    profile.Country = updateDto.Country;

    await _context.SaveChangesAsync();

    return Ok(new { message = "Profile updated successfully." });
  }
}
