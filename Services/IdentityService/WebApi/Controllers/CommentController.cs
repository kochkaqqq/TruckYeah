using Application.Interfaces;
using Application.Shared.Dtos.Requests.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDtoRequest request)
        {
            var result = await _commentService.AddComment(request);
            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDtoRequest request)
        {
            var result = await _commentService.UpdateComment(request);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
        {
            await _commentService.DeleteComment(id);
            return Ok();
        }

        [Authorize]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserComments([FromRoute] Guid userId)
        {
            var result = await _commentService.GetUserComments(userId);
            return Ok(result);
        }
    }
}
