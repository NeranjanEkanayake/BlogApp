using CommonData.Data;
using CommonData.DTO;
using CommonData.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonData.Services
{
    public interface IBlogService
    {
        Task<List<BlogDTO>> GetAllAsync();
        Task<BlogDTO?> GetBlogIdAsync(int id);
        Task AddBlogAsync(BlogModel blog);
        Task UpdateBlogAsync(BlogDTO blog);
        Task DeleteBlogAsync(int id);

        Task<List<CommentsModel>> GetCommentsByBlogIdAsync(int blogId);
        Task<BlogWithCommentDTO> GetBlogWithCommentsAsync(int blogId);
        Task<BlogModel>GetBlogAndComModelAsync(int blogId);
        Task AddCommentAsync(CommentsModel comment);
    }
}
