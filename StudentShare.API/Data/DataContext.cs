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
    }
}