using CommonData.Models;
using CommonData.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        public readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogModel>>> GetBlogs()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogModel>> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);
        }

        [HttpPost("createblog")]
        public async Task<ActionResult> CreateBlog(BlogModel blogModel)
        {
            var blog = new BlogModel
            {
                Title = blogModel.Title,
                Description = blogModel.Description,
                UserId = blogModel.UserId,
                CreatedDate = DateTime.Now,
            };
            
            await _blogService.AddBlogAsync(blog);
            return Ok(new
            {
                Message= "Created Blog successfully"
            });
        }

        //[HttpPut("editblog{id}")]

    }
}
