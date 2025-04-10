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
        private readonly MongoCommentService _mongoCommentService;

        public BlogService(AppDbContext appDbContext, MongoCommentService mongoCommentService)
        {
            _appDbContext = appDbContext;
            _mongoCommentService = mongoCommentService;
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

        public async Task<BlogWithCommentDTO> GetBlogWithCommentsAsync(int blogId)
        {
            var blog = await _appDbContext.Blogs
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
                return null;

            var mongoComments = await _mongoCommentService.GetCommentsByBlogIdAsync(blogId);

            var userIds = mongoComments.Select(c => c.UserId).Distinct().ToList();

            var users = await _appDbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var dto = new BlogWithCommentDTO
            {
                Id = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
                Comments = mongoComments.Select(c => new CommentDTO
                {
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    Author = users.TryGetValue(c.UserId, out var user)
                        ? new UserViewDTO
                        {
                            Id = user.Id,
                            Name = user.Name,
                            UserName = user.UserName
                        }
                        : null
                }).ToList()
            };

            return dto;
        }

        ////For MVC project
        //public async Task<BlogModel> GetBlogAndComModelAsync(int blogId)
        //{
        //    var blog = await _appDbContext.Blogs.Include(b => b.Author).Select(blog => new BlogModel()
        //    {
        //        BlogId = blog.BlogId,
        //        Title = blog.Title,
        //        Description = blog.Description,
        //        Comments = blog.Comments

        //    }).FirstOrDefaultAsync(b => b.BlogId == blogId);
        //    //var saf=  await _appDbContext.Blogs
        //    //     .Include(b => b.Comments)
        //    //     .ThenInclude(c => c.Author)
        //    //     .FirstOrDefaultAsync(b => b.BlogId == blogId);
        //    return blog;
        //}

        public async Task AddCommentAsync(CommentsModel comment)
        {
            _appDbContext.Comments.Add(comment);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
