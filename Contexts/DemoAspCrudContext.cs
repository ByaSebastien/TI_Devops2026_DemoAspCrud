using Microsoft.EntityFrameworkCore;
using TI_Devops2026_DemoAspCrud.Entities;

namespace TI_Devops2026_DemoAspCrud.Contexts
{
    public class DemoAspCrudContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>(); 
        public DbSet<Category> Categories => Set<Category>(); 

        public DemoAspCrudContext(DbContextOptions<DemoAspCrudContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoAspCrudContext).Assembly);
        }
    }
}
