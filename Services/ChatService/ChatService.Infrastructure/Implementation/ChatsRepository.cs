using ChatService.Domain.Enums;
using ChatService.Domain.Models;
using ChatService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Implementation;

public class ChatsRepository : IChatsRepository
{
    private readonly ChatServiceDbContext _dbContext;

    public ChatsRepository(ChatServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Chat?> FindByContextAndParticipants(ChatContextType contextType, Guid contextId, Guid firstUserId, Guid secondUserId)
    {
        return await _dbContext.Chats
            .Include(c => c.Participants)
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.ContextType == contextType &&
                c.ContextId == contextId &&
                c.Participants.Count == 2 &&
                c.Participants.Any(p => p.UserId == firstUserId) &&
                c.Participants.Any(p => p.UserId == secondUserId));
    }

    public async Task<Chat?> GetById(Guid chatId)
    {
        return await _dbContext.Chats
            .Include(c => c.Participants)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    public async Task<ChatSummary?> GetSummary(Guid chatId, Guid userId)
    {
        return await BuildSummaryQuery(userId)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    public async Task<List<ChatSummary>> GetSummaries(Guid userId)
    {
        return await BuildSummaryQuery(userId)
            .OrderByDescending(c => c.LastMessageAt ?? DateTime.MinValue)
            .ToListAsync();
    }

    public async Task<Guid> Create(Chat chat)
    {
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();
        return chat.Id;
    }

    public async Task<List<ChatMessage>> GetMessages(Guid chatId, int page, int pageSize)
    {
        var messages = await _dbContext.ChatMessages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return messages
            .OrderBy(m => m.CreatedAt)
            .ToList();
    }

    public async Task<Guid> AddMessage(Guid chatId, ChatMessage message, Guid recipientUserId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var chat = await _dbContext.Chats
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == chatId)
            ?? throw new KeyNotFoundException("Chat was not found.");

        var recipient = chat.Participants.FirstOrDefault(p => p.UserId == recipientUserId)
            ?? throw new UnauthorizedAccessException("Recipient is not a chat participant.");

        chat.LastMessageAt = message.CreatedAt;
        recipient.UnreadCount++;

        await _dbContext.ChatMessages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return message.Id;
    }

    public async Task MarkRead(Guid chatId, Guid userId, DateTime readAt)
    {
        var participant = await _dbContext.ChatParticipants
            .FirstOrDefaultAsync(p => p.ChatId == chatId && p.UserId == userId)
            ?? throw new UnauthorizedAccessException("Only chat participants can mark chat as read.");

        participant.LastReadAt = readAt;
        participant.UnreadCount = 0;

        await _dbContext.ChatMessages
            .Where(m => m.ChatId == chatId && m.SenderUserId != userId && m.ReadAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.ReadAt, readAt));

        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCount(Guid userId)
    {
        return await _dbContext.ChatParticipants
            .Where(p => p.UserId == userId)
            .SumAsync(p => p.UnreadCount);
    }

    private IQueryable<ChatSummary> BuildSummaryQuery(Guid userId)
    {
        return _dbContext.Chats
            .AsNoTracking()
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Select(c => new ChatSummary
            {
                Id = c.Id,
                ContextType = c.ContextType,
                ContextId = c.ContextId,
                OtherParticipantUserId = c.Participants
                    .Where(p => p.UserId != userId)
                    .Select(p => p.UserId)
                    .FirstOrDefault(),
                LastMessageText = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.IsDeleted ? null : m.Text)
                    .FirstOrDefault(),
                LastMessageAt = c.LastMessageAt,
                UnreadCount = c.Participants
                    .Where(p => p.UserId == userId)
                    .Select(p => p.UnreadCount)
                    .FirstOrDefault()
            });
    }
}
