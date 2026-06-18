using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ModeratorService.WebApi.Controllers;

[ApiController]
[Route("api/moderator")]
public sealed class ModeratorController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public Task<IActionResult> Login([FromBody] JsonElement body) =>
        ForwardAsync("identity", HttpMethod.Post, "api/users/login", body);

    [Authorize(Roles = "Moderator")]
    [HttpPost("logout")]
    public Task<IActionResult> Logout([FromBody] JsonElement body) =>
        ForwardAsync("identity", HttpMethod.Post, "api/users/logout", body);

    [Authorize(Roles = "Moderator")]
    [HttpGet("users")]
    public Task<IActionResult> GetUsers() =>
        ForwardAsync("identity", HttpMethod.Get, "api/admin/users");

    [Authorize(Roles = "Moderator")]
    [HttpPost("users/{id:guid}/block")]
    public Task<IActionResult> BlockUser(Guid id) =>
        ForwardAsync("identity", HttpMethod.Post, $"api/admin/users/{id}/block");

    [Authorize(Roles = "Moderator")]
    [HttpPost("users/{id:guid}/unblock")]
    public Task<IActionResult> UnblockUser(Guid id) =>
        ForwardAsync("identity", HttpMethod.Post, $"api/admin/users/{id}/unblock");

    [Authorize(Roles = "Moderator")]
    [HttpGet("cargos")]
    public Task<IActionResult> GetCargos() =>
        ForwardAsync("listing", HttpMethod.Get, "api/admin/cargos");

    [Authorize(Roles = "Moderator")]
    [HttpGet("trucks")]
    public Task<IActionResult> GetTrucks() =>
        ForwardAsync("listing", HttpMethod.Get, "api/admin/trucks");

    [Authorize(Roles = "Moderator")]
    [HttpPost("cargos/{id:guid}/approve")]
    public Task<IActionResult> ApproveCargo(Guid id) =>
        ForwardAsync("listing", HttpMethod.Post, $"api/admin/cargos/{id}/approve");

    [Authorize(Roles = "Moderator")]
    [HttpPost("cargos/{id:guid}/reject")]
    public Task<IActionResult> RejectCargo(Guid id, [FromBody] JsonElement body) =>
        ForwardAsync("listing", HttpMethod.Post, $"api/admin/cargos/{id}/reject", body);

    [Authorize(Roles = "Moderator")]
    [HttpPost("trucks/{id:guid}/approve")]
    public Task<IActionResult> ApproveTruck(Guid id) =>
        ForwardAsync("listing", HttpMethod.Post, $"api/admin/trucks/{id}/approve");

    [Authorize(Roles = "Moderator")]
    [HttpPost("trucks/{id:guid}/reject")]
    public Task<IActionResult> RejectTruck(Guid id, [FromBody] JsonElement body) =>
        ForwardAsync("listing", HttpMethod.Post, $"api/admin/trucks/{id}/reject", body);

    private async Task<IActionResult> ForwardAsync(
        string clientName,
        HttpMethod method,
        string path,
        JsonElement? body = null)
    {
        var client = httpClientFactory.CreateClient(clientName);
        using var request = new HttpRequestMessage(method, path);
        var authorization = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorization);
        }

        if (body.HasValue)
        {
            request.Content = new StringContent(
                body.Value.GetRawText(),
                Encoding.UTF8,
                "application/json");
        }

        using var response = await client.SendAsync(request, HttpContext.RequestAborted);
        var content = await response.Content.ReadAsStringAsync(HttpContext.RequestAborted);
        if (string.IsNullOrEmpty(content))
        {
            return StatusCode((int)response.StatusCode);
        }

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json"
        };
    }
}
