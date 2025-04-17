using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Models;

namespace TaskManagementBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
