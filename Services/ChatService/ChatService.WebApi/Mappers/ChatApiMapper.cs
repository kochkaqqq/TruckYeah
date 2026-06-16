using ChatService.Domain.Models;
using ChatService.WebApi.Contracts;

namespace ChatService.WebApi.Mappers;

public static class ChatApiMapper
{
    public static ChatResponse MapToResponse(this ChatSummary summary)
    {
        return new ChatResponse
        {
            Id = summary.Id,
            ContextType = summary.ContextType,
            ContextId = summary.ContextId,
            OtherParticipantUserId = summary.OtherParticipantUserId,
            LastMessageText = summary.LastMessageText,
            LastMessageAt = summary.LastMessageAt,
            UnreadCount = summary.UnreadCount
        };
    }

    public static ChatMessageResponse MapToResponse(this ChatMessage message)
    {
        return new ChatMessageResponse
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderUserId = message.SenderUserId,
            Text = message.Text,
            CreatedAt = message.CreatedAt,
            ReadAt = message.ReadAt,
            IsDeleted = message.IsDeleted
        };
    }
}
