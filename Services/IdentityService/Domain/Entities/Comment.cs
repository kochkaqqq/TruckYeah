using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid AutorId { get; set; }
        public Guid UserId { get; set; }
        public float Rating { get; set; }
        public CommentText CommentText { get; set; } = null!;
    }
}
