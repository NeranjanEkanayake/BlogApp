using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommonData.Services;
using CommonData.Models;
using System.Security.Claims;
using System.Reflection.Metadata;
using CommonData.DTO;

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
                blogs = new List<BlogDTO>();
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
        public async Task<IActionResult> Edit(int id, BlogDTO blogDTO)
        {
            if (id != blogDTO.BlogId) 
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
                    blogDTO.UserId = existingBlog.UserId;
                   

                    await _blogService.UpdateBlogAsync(blogDTO);
                    return RedirectToAction("Details", new { id = blogDTO.BlogId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating blog: " + ex.Message);
                }
            }
            return View(blogDTO);
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
