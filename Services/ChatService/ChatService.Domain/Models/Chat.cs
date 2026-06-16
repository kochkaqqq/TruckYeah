using ChatService.Domain.Enums;

namespace ChatService.Domain.Models;

public class Chat
{
    public Guid Id { get; set; }
    public ChatContextType ContextType { get; set; }
    public Guid ContextId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
