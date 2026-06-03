using Domain.ValueObjects;

namespace Application.Shared.Dtos.Requests.Comments
{
    public class AddCommentDtoRequest
    {
        public Guid AutorId { get; set; } = default!;
        public Guid UserId { get; set; } = default!;
        public float Rating { get; set; }
        public string CommentText { get; set; } = null!;
    }
}
