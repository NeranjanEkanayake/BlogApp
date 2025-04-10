using CommonData.MongoModels;
using CommonData.ServiceClasses;
using CommonData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly MongoCommentService _mongoCommentService;
        
        public CommentController(IBlogService blogService, MongoCommentService mongoCommentService,IUserService userService)
        {
            _mongoCommentService = mongoCommentService;
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
            
            var comment = new CommentsMongo
            {
                BlogId = blogId,
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow //change this, this is just for ref
            };
            await _mongoCommentService.CommentsAsync(comment);

            return RedirectToAction("Details","Blog", new {id = blogId});
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
