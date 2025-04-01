using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogApp.Services;
using BlogApp.Models;
using System.Security.Claims;
using System.Reflection.Metadata;

namespace BlogApp.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _blogService.GetAllAsync();
            return View(blogs);
        }

        // GET: BLogController/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: BLogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BlogModel blogModel)
        {
            if (ModelState.IsValid)
            {
                blogModel.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                blogModel.CreatedDate = DateTime.Now;
                await _blogService.AddBlogAsync(blogModel);
                return RedirectToAction("Index");
            }
            return View(blogModel);
        }


        
        public async Task<IActionResult> Details(int id)
        {
            var blogs = await _blogService.GetBlogIdAsync(id);
            if(blogs == null)
            {
                return NotFound();
            }
            var comments = await _blogService.GetCommentsByBlogIdAsync(id);
            ViewBag.Comments = comments;
            return View(blogs);
        }

        // GET: BLogController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogService.GetBlogIdAsync(id);

            if (blog == null || blog.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }
            return View(blog);
        }

        // POST: BLogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, BlogModel blogModel)
        {
            if (id != blogModel.BlogId) return NotFound();

            if (ModelState.IsValid)
            {
                await _blogService.UpdateBlogAsync(blogModel);
                return RedirectToAction(nameof(Index));
            }
            return View(blogModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: BLogController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogService.GetBlogIdAsync(id);
            if (blog == null || blog.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }
            return View(blog);
        }

        // POST: BLogController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _blogService.DeleteBlogAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
