using ChatService.Application.Interfaces;
using ChatService.Domain.Enums;
using ChatService.Domain.Models;
using ChatService.Infrastructure.Interfaces;

namespace ChatService.Application.Services;

public class ChatsService : IChatsService
{
    private const int MaxMessageLength = 4000;
    private readonly IChatsRepository _repository;

    public ChatsService(IChatsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChatSummary> CreateOrGetChatAsync(ChatContextType contextType, Guid contextId, Guid currentUserId, Guid recipientUserId)
    {
        if (contextId == Guid.Empty)
            throw new ArgumentException("Context id is required.");
        if (currentUserId == Guid.Empty)
            throw new UnauthorizedAccessException("Current user id is missing.");
        if (recipientUserId == Guid.Empty)
            throw new ArgumentException("Recipient user id is required.");
        if (currentUserId == recipientUserId)
            throw new InvalidOperationException("Cannot create a chat with yourself.");

        var existing = await _repository.FindByContextAndParticipants(contextType, contextId, currentUserId, recipientUserId);
        if (existing is not null)
        {
            return await GetExistingSummary(existing.Id, currentUserId);
        }

        var chatId = Guid.NewGuid();
        var chat = new Chat
        {
            Id = chatId,
            ContextType = contextType,
            ContextId = contextId,
            CreatedAt = DateTime.UtcNow,
            Participants =
            [
                new ChatParticipant { ChatId = chatId, UserId = currentUserId, UnreadCount = 0 },
                new ChatParticipant { ChatId = chatId, UserId = recipientUserId, UnreadCount = 0 }
            ]
        };

        await _repository.Create(chat);
        return await GetExistingSummary(chatId, currentUserId);
    }

    public Task<List<ChatSummary>> GetMyChatsAsync(Guid userId)
    {
        EnsureUser(userId);
        return _repository.GetSummaries(userId);
    }

    public async Task<List<ChatMessage>> GetMessagesAsync(Guid chatId, Guid userId, int? page, int? pageSize)
    {
        await EnsureParticipant(chatId, userId);
        var normalizedPage = Math.Max(page.GetValueOrDefault(1), 1);
        var normalizedPageSize = Math.Clamp(pageSize.GetValueOrDefault(50), 1, 100);
        return await _repository.GetMessages(chatId, normalizedPage, normalizedPageSize);
    }

    public async Task<ChatMessage> SendMessageAsync(Guid chatId, Guid senderUserId, string text)
    {
        var chat = await EnsureParticipant(chatId, senderUserId);
        var trimmedText = text.Trim();

        if (string.IsNullOrWhiteSpace(trimmedText))
            throw new ArgumentException("Message text is required.");
        if (trimmedText.Length > MaxMessageLength)
            throw new ArgumentException($"Message text cannot be longer than {MaxMessageLength} characters.");

        var recipient = chat.Participants.FirstOrDefault(p => p.UserId != senderUserId)
            ?? throw new InvalidOperationException("Chat must have a recipient.");

        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderUserId = senderUserId,
            Text = trimmedText,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddMessage(chatId, message, recipient.UserId);
        return message;
    }

    public async Task MarkReadAsync(Guid chatId, Guid userId)
    {
        await EnsureParticipant(chatId, userId);
        await _repository.MarkRead(chatId, userId, DateTime.UtcNow);
    }

    public Task<int> GetUnreadCountAsync(Guid userId)
    {
        EnsureUser(userId);
        return _repository.GetUnreadCount(userId);
    }

    private async Task<ChatSummary> GetExistingSummary(Guid chatId, Guid userId)
    {
        return await _repository.GetSummary(chatId, userId)
            ?? throw new KeyNotFoundException("Chat was not found.");
    }

    private async Task<Chat> EnsureParticipant(Guid chatId, Guid userId)
    {
        EnsureUser(userId);
        var chat = await _repository.GetById(chatId)
            ?? throw new KeyNotFoundException("Chat was not found.");

        if (!chat.Participants.Any(p => p.UserId == userId))
            throw new UnauthorizedAccessException("Only chat participants can perform this action.");

        return chat;
    }

    private static void EnsureUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("Current user id is missing.");
    }
}
