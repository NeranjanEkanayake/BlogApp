using CommonData.MongoModels;
using CommonData.ServiceClasses;
using CommonData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly MongoCommentService _commentService;


        public CommentController(IBlogService blogService, MongoCommentService commentService)
        {
            _blogService = blogService;
            _commentService = commentService;
        }

        //add comment
        [HttpPost("addComment")]
        [Authorize]
        public async Task<ActionResult> AddComment(int blogId, string content)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }
            var comment = new CommentsMongo
            {
                UserId = userId,
                Content = content,
                BlogId = blogId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _commentService.CommentsAsync(comment);

            return Ok(comment);
        }
    }
}
