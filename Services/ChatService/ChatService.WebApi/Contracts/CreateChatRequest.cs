using ChatService.Domain.Enums;

namespace ChatService.WebApi.Contracts;

public class CreateChatRequest
{
    public ChatContextType ContextType { get; set; }
    public Guid ContextId { get; set; }
    public Guid RecipientUserId { get; set; }
}
