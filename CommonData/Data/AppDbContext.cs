using CommonData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommonData.Data
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BlogModel> Blogs { get; set; }

        public DbSet<CommentsModel> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserModel>().ToTable("Users");
            builder.Entity<BlogModel>().ToTable("Blogs");
            builder.Entity<CommentsModel>().ToTable("Comments");

            builder.Entity<BlogModel>()
                .HasMany(b=>b.Comments)
                .WithOne(b=>b.Blog)
                .HasForeignKey(b=>b.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentsModel>()
                .HasOne(c=>c.Author)
                .WithMany()
                .HasForeignKey(c=>c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
