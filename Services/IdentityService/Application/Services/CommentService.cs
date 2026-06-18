using Application.Interfaces;
using Application.Shared.Dtos.Requests.Comments;
using Application.Shared.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IDbContext _dbContext;

        public CommentService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment> AddComment(AddCommentDtoRequest request)
        {
            var newEntity = new Comment()
            {
                AutorId = request.AutorId,
                UserId = request.UserId,
                Rating = request.Rating,
                CommentText = new CommentText(request.CommentText),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Comments.AddAsync(newEntity);
            await _dbContext.SaveChangesAsync();

            return newEntity;
        }

        public async Task DeleteComment(Guid commentId)
        {
            var entity = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId)
                ?? throw new EntityNotFoundException(nameof(Comment), commentId.ToString());

            _dbContext.Comments.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<Comment>> GetUserComments(Guid userId)
        {
            return await _dbContext.Comments.AsNoTracking().Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<Comment> UpdateComment(UpdateCommentDtoRequest request)
        {
            var entity = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == request.Id)
                ?? throw new EntityNotFoundException(nameof(Comment), request.Id.ToString());

            entity.CommentText = new CommentText(request.CommentText);
            entity.Rating = request.Rating;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
