using System.Security.Claims;
using ChatService.Application.Interfaces;
using ChatService.WebApi.Contracts;
using ChatService.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatsService _chatsService;

    public ChatsController(IChatsService chatsService)
    {
        _chatsService = chatsService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> CreateOrGetChat([FromBody] CreateChatRequest request)
    {
        try
        {
            var chat = await _chatsService.CreateOrGetChatAsync(
                request.ContextType,
                request.ContextId,
                GetCurrentUserId(),
                request.RecipientUserId);

            return Ok(chat.MapToResponse());
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatResponse>>> GetMyChats()
    {
        try
        {
            var chats = await _chatsService.GetMyChatsAsync(GetCurrentUserId());
            return Ok(chats.Select(c => c.MapToResponse()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [HttpGet("{chatId:guid}/messages")]
    public async Task<ActionResult<List<ChatMessageResponse>>> GetMessages(Guid chatId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        try
        {
            var messages = await _chatsService.GetMessagesAsync(chatId, GetCurrentUserId(), page, pageSize);
            return Ok(messages.Select(m => m.MapToResponse()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [HttpPost("{chatId:guid}/messages")]
    public async Task<ActionResult<ChatMessageResponse>> SendMessage(Guid chatId, [FromBody] SendMessageRequest request)
    {
        try
        {
            var message = await _chatsService.SendMessageAsync(chatId, GetCurrentUserId(), request.Text);
            return Ok(message.MapToResponse());
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [HttpPost("{chatId:guid}/read")]
    public async Task<ActionResult> MarkRead(Guid chatId)
    {
        try
        {
            await _chatsService.MarkReadAsync(chatId, GetCurrentUserId());
            return NoContent();
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadCountResponse>> GetUnreadCount()
    {
        try
        {
            return Ok(new UnreadCountResponse
            {
                UnreadCount = await _chatsService.GetUnreadCountAsync(GetCurrentUserId())
            });
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User id claim is missing.");
    }

    private ActionResult ToActionResult(Exception ex)
    {
        return ex switch
        {
            ArgumentException => BadRequest(ex.Message),
            InvalidOperationException => BadRequest(ex.Message),
            KeyNotFoundException => NotFound(ex.Message),
            UnauthorizedAccessException => Forbid(),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal server error")
        };
    }
}
