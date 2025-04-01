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
        
        public CommentController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int blogId, string content)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new CommentsModel
            {
                Id = blogId,
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
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
