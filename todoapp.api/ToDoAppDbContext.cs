using Microsoft.EntityFrameworkCore;

public class ToDoAppDbContext : DbContext
{
    public ToDoAppDbContext(DbContextOptions<ToDoAppDbContext> options) : base(options)
    {

    }

    // db sets
    public DbSet<ToDoItem> ToDoItems { get; set; }
}