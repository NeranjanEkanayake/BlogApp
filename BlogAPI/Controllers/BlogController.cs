using CommonData.Models;
using CommonData.Services;
using Microsoft.AspNetCore.Mvc;
using CommonData.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        public readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<BlogDTO>> GetBlogs()
        {
            var blogs = await _blogService.GetAllAsync();


            return Ok(blogs);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogDTO>> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);
        }

        [HttpGet("blogwithComments/{id}")]
        [Authorize]
        public async Task<ActionResult> GetBlogAndComments(int id)
        {
            var blogs = await _blogService.GetBlogWithCommentsAsync(id);
            if(blogs == null)
            {
                return NotFound();
            }
            return Ok(blogs);
        }

        
        [HttpPost("createblog")]
        [Authorize]
        public async Task<ActionResult> CreateBlog(BlogDTO blogModel)
        {
            var blog = new BlogModel
            {
                Title = blogModel.Title,
                Description = blogModel.Description,
                UserId = blogModel.UserId
               
            };
            
            await _blogService.AddBlogAsync(blog);
            return Ok(blog);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("editblog/{id}")]
        public async Task<ActionResult> EditBlog(int id, BlogDTO blogDTO)
        {
            var blog = await _blogService.GetBlogIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            blogDTO.UserId = blog.UserId;

            await _blogService.UpdateBlogAsync(blogDTO);
                        
            return Ok(blog);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteBlog(int id)
        {
            var blog = await _blogService.GetBlogIdAsync(id);
            if(blog == null)
            {
                return NotFound();
            }
            await _blogService.DeleteBlogAsync(id);
            return Ok(blog);
        }
    }
}
