using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Like>().HasKey(key => new {key.LikerId, key.LikeeId});
            modelBuilder.Entity<Like>()
                .HasOne(like => like.Likee)
                .WithMany(user => user.Likers)
                .HasForeignKey(like => like.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(like => like.Liker)
                .WithMany(user => user.Likees)
                .HasForeignKey(like => like.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}