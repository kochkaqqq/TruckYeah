using Domain.ValueObjects;

namespace Application.Shared.Dtos.Requests.Comments
{
    public class UpdateCommentDtoRequest
    {
        public Guid Id { get; set; }
        public Guid AutorId { get; set; } = default!;
        public Guid UserId { get; set; } = default!;
        public float Rating { get; set; }
        public string CommentText { get; set; } = null!;
    }
}
