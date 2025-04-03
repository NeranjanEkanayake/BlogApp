using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogApp.Services;
using BlogApp.Models;
using System.Security.Claims;
using System.Reflection.Metadata;

namespace BlogApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogService.GetAllAsync();
            if (blogs == null)
            {
                blogs = new List<BlogModel>();
            }
            return View(blogs);
        }

        // GET: BLogController/Create
        [Authorize(Roles = "Admin")]
        public IActionResult CreateBlog()
        {
            return View();
        }

        // POST: BLogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(BlogModel blogModel)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    Console.WriteLine("user id: "+User.FindFirstValue(ClaimTypes.NameIdentifier));
                    blogModel.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    blogModel.CreatedDate = DateTime.UtcNow;
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Console.WriteLine($"Blog model UId: {blogModel.UserId}");
                            await _blogService.AddBlogAsync(blogModel);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("Error:", "Model errors");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }                                     
                }
                return View(blogModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(blogModel);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var comments = await _blogService.GetBlogWithCommentsAsync(id);
            //var blogs = await _blogService.GetBlogIdAsync(id);
            if (comments == null)
            {
                return NotFound();
            }
            
            //ViewBag.Comments = comments;
            return View(comments);
        }

        // GET: BLogController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogService.GetBlogIdAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            if (blog.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
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
            if (id != blogModel.BlogId) 
            { 
                return NotFound(); 
            }
            var existingBlog = await _blogService.GetBlogIdAsync(id);
            if (existingBlog == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve original values that shouldn't change
                    blogModel.UserId = existingBlog.UserId;
                    blogModel.CreatedDate = existingBlog.CreatedDate;

                    await _blogService.UpdateBlogAsync(blogModel);
                    return RedirectToAction("Details", new { id = blogModel.BlogId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating blog: " + ex.Message);
                }
            }
            return View(blogModel);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogService.GetBlogIdAsync((int)id);

            if(blog == null)
            {
                return NotFound();
            }
            try
            {
                await _blogService.DeleteBlogAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting: "+ex);
                return RedirectToAction(nameof(Index));
            }
            
        }
    }
}
