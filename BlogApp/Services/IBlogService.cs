using BlogApp.Data;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services
{
    public interface IBlogService
    {
        Task<List<BlogModel>> GetAllAsync();
        Task<BlogModel?> GetBlogIdAsync(int id);
        Task AddBlogAsync(BlogModel blog);
        Task UpdateBlogAsync(BlogModel blog);
        Task DeleteBlogAsync(int id);

        Task<List<CommentsModel>> GetCommentsByBlogIdAsync(int blogId);
        Task<BlogModel> GetBlogWithCommentsAsync(int blogId);
        Task AddCommentAsync(CommentsModel comment);
    }

    public class BlogService : IBlogService
    {
        private readonly AppDbContext _appDbContext;

        public BlogService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<BlogModel>> GetAllAsync()
        {
            return await _appDbContext.Blogs.Include(b => b.Author).ToListAsync();
        }

        public async Task<BlogModel?> GetBlogIdAsync(int id)
        {
            return await _appDbContext.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b=>b.BlogId == id);
        }

        public async Task AddBlogAsync(BlogModel blog)
        {
            _appDbContext.Blogs.Add(blog);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateBlogAsync(BlogModel blogModel)
        {
            var existingBlog = await _appDbContext.Blogs.FindAsync(blogModel.BlogId);
            if (existingBlog != null)
            {
                existingBlog.Title = blogModel.Title;
                existingBlog.Description = blogModel.Description;
                await _appDbContext.SaveChangesAsync();
            }           
        }

        public async Task DeleteBlogAsync(int id)
        {
            var blog = await _appDbContext.Blogs.FindAsync(id);
            if (blog != null)
            {
                _appDbContext.Blogs.Remove(blog);
                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<List<CommentsModel>> GetCommentsByBlogIdAsync(int blogId)
        {
            return await _appDbContext.Comments.Where(c=>c.BlogId == blogId).ToListAsync();
        }

        public async Task<BlogModel> GetBlogWithCommentsAsync(int blogId)
        {
            return await _appDbContext.Blogs
                .Include(b => b.Comments)
                .ThenInclude(c => c.Author) 
                .FirstOrDefaultAsync(b => b.BlogId == blogId);
        }

        public async Task AddCommentAsync(CommentsModel comment)
        {
            _appDbContext.Comments.Add(comment);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
