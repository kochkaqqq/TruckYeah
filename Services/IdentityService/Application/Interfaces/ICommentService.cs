using Application.Shared.Dtos.Requests.Comments;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICommentService
    {
        Task<ICollection<Comment>> GetUserComments(Guid userId);
        Task<Comment> AddComment(AddCommentDtoRequest request);
        Task DeleteComment(Guid commentId);
        Task<Comment> UpdateComment(UpdateCommentDtoRequest request); 
    }
}
