
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {   
        }

        public DbSet<Value> Values{ get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<Like>  Like { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
            .HasKey(k => new {k.LikeeId, k.LikerId});

            builder.Entity<Like>()
            .HasOne(l => l.Likee)
            .WithMany(l => l.Likers)
            .HasForeignKey(l => l.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey(u => u.LikerId)
            .OnDelete(DeleteBehavior.Restrict);
        }
        
    }
}