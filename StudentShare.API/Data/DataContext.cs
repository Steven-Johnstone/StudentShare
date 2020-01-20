using Microsoft.EntityFrameworkCore;
using StudentShare.API.Models;

namespace StudentShare.API.Data
{
    public class DataContext : DbContext // DataContext inherits from DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){     
        }

        public DbSet<Value> Values {get; set;}
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) // this section of code is used to allow entity framework to set up a many to many relationship
        {
            builder.Entity<Like>()
            .HasKey(k => new {k.LikerId, k.LikeeId});
            builder.Entity<Like>()
            .HasOne(u => u.Likee)
            .WithMany(u => u.Likers)
            .HasForeignKey(u => u.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey(u => u.LikerId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}