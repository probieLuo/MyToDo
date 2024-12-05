using Microsoft.EntityFrameworkCore;

namespace MyToDo.Api.Context
{
    public class MyToDoContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Memo> Memos { get; set; }
        public MyToDoContext(DbContextOptions<MyToDoContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
