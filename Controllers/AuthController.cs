using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.Models.Auth;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
  {
    try
    {
      var user = await _authService.RegisterAsync(request.Email, request.Password);
      return Ok(new { user.Id, user.Email });
    }
    catch (Exception ex)
    {
      return BadRequest(new { error = ex.Message });
    }
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
  {
    try
    {
      var tokens = await _authService.LoginAsync(request.Email, request.Password);
      return Ok(new
      {
        accessToken = tokens.AccessToken,
        refreshToken = tokens.RefreshToken
      });
    }
    catch (Exception ex)
    {
      return Unauthorized(new { error = ex.Message });
    }
  }

  [HttpGet("verify")]
  public async Task<IActionResult> VerifyEmail([FromQuery] string token)
  {
    try
    {
      await _authService.VerifyEmailAsync(token);
      return Ok(new { message = "Email verified successfully. You can now log in." });
    }
    catch (Exception ex)
    {
      return BadRequest(new { error = ex.Message });
    }
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO dto)
  {
    try
    {
      var tokens = await _authService.RefreshTokenAsync(dto.RefreshToken);
      return Ok(new
      {
        accessToken = tokens.AccessToken,
        refreshToken = tokens.RefreshToken
      });
    }
    catch (Exception ex)
    {
      return Unauthorized(new { error = ex.Message });
    }
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO dto)
  {
    try
    {
      await _authService.LogoutAsync(dto.RefreshToken);
      return Ok(new { message = "Logged out successfully" });
    }
    catch (Exception ex)
    {
      return BadRequest(new { error = ex.Message });
    }
  }
}
