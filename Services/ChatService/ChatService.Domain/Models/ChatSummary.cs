using ChatService.Domain.Enums;

namespace ChatService.Domain.Models;

public class ChatSummary
{
    public Guid Id { get; set; }
    public ChatContextType ContextType { get; set; }
    public Guid ContextId { get; set; }
    public Guid OtherParticipantUserId { get; set; }
    public string? LastMessageText { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
}
