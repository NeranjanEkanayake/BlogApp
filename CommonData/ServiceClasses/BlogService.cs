using CommonData.Data;
using CommonData.DTO;
using CommonData.Models;
using CommonData.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.ServiceClasses
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _appDbContext;

        public BlogService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<BlogDTO>> GetAllAsync()
        {
            return await _appDbContext.Blogs.Select(blog => new BlogDTO()
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
            }).ToListAsync();
        }

        public async Task<BlogDTO?> GetBlogIdAsync(int id)
        {
            return await _appDbContext.Blogs.Include(b => b.Author).Select(blog => new BlogDTO()
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
            }).FirstOrDefaultAsync(b => b.BlogId == id);
        }

        public async Task AddBlogAsync(BlogModel blog)
        {
            _appDbContext.Blogs.Add(blog);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateBlogAsync(BlogDTO blogModel)
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
            return await _appDbContext.Comments.Where(c => c.BlogId == blogId).ToListAsync();
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
