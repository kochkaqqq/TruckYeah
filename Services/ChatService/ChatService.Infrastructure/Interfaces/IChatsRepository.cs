using ChatService.Domain.Enums;
using ChatService.Domain.Models;

namespace ChatService.Infrastructure.Interfaces;

public interface IChatsRepository
{
    Task<Chat?> FindByContextAndParticipants(ChatContextType contextType, Guid contextId, Guid firstUserId, Guid secondUserId);
    Task<Chat?> GetById(Guid chatId);
    Task<ChatSummary?> GetSummary(Guid chatId, Guid userId);
    Task<List<ChatSummary>> GetSummaries(Guid userId);
    Task<Guid> Create(Chat chat);
    Task<List<ChatMessage>> GetMessages(Guid chatId, int page, int pageSize);
    Task<Guid> AddMessage(Guid chatId, ChatMessage message, Guid recipientUserId);
    Task MarkRead(Guid chatId, Guid userId, DateTime readAt);
    Task<int> GetUnreadCount(Guid userId);
}
