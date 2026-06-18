using System.Security.Claims;
using Application.Interfaces;
using Application.Shared.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationDtoRequest request) =>
        Ok(await userService.RegistrationUserAsync(request));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDtoRequest request) =>
        Ok(await userService.LoginUserAsync(request));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string token) =>
        Ok(await userService.RefreshTokenAsync(token));

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser() =>
        Ok(await userService.GetCurrentUserAsync(GetCurrentUserId()));

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserProfileRequest request) =>
        Ok(await userService.UpdateCurrentUserAsync(GetCurrentUserId(), request));

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPublicUser([FromRoute] Guid id) =>
        Ok(await userService.GetPublicUserAsync(id));

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User id claim is missing.");
    }
}
