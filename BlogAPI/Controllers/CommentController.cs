using CommonData.DTO;
using CommonData.Models;
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
        private readonly IUserService _userService;


        public CommentController(IBlogService blogService, MongoCommentService commentService, IUserService userService)
        {
            _blogService = blogService;
            _commentService = commentService;
            _userService = userService;
        }

        //add comment
        [HttpPost("addComment")]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> AddComment([FromBody] AddCommentDTO commentDTO)
        {
            var userId = User.FindFirstValue("userId");
            Console.WriteLine("User Id in the API");
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            var comment = new CommentsMongo
            {
                UserId = userId,
                Content = commentDTO.Content,
                BlogId = commentDTO.BlogId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            try
            {
                await _commentService.CommentsAsync(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Ok(comment);
        }
    }
}
