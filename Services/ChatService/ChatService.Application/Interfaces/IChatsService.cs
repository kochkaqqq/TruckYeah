using ChatService.Domain.Enums;
using ChatService.Domain.Models;

namespace ChatService.Application.Interfaces;

public interface IChatsService
{
    Task<ChatSummary> CreateOrGetChatAsync(ChatContextType contextType, Guid contextId, Guid currentUserId, Guid recipientUserId);
    Task<List<ChatSummary>> GetMyChatsAsync(Guid userId);
    Task<List<ChatMessage>> GetMessagesAsync(Guid chatId, Guid userId, int? page, int? pageSize);
    Task<ChatMessage> SendMessageAsync(Guid chatId, Guid senderUserId, string text);
    Task MarkReadAsync(Guid chatId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
}
