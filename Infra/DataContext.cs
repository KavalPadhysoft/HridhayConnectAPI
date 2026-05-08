using HridhayConnect_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HridhayConnect_API.Infra
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Role> Roles { get; set; }
     

        // name this DbSet something sensible for code readability
        public DbSet<MenuAccessDto> MenuAccesses { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MenuAccessDto>().HasNoKey();
        }
    }
}
