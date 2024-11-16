using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        // Constructor with the correct type for DbContextOptions
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Correctly named DbSet for AppUser
        public DbSet<AppUser>? Users { get; set; } 
    }
}
