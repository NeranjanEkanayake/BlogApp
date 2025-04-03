using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IBlogService _blogService;
        
        public CommentController(IBlogService blogService, IUserService userService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int blogId, string content)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            var comment = new CommentsModel
            {
                BlogId = blogId,
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow //change this, this is just for ref
            };
            await _blogService.AddCommentAsync(comment);

            return RedirectToAction("Details","Blog", new {id = blogId});
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
