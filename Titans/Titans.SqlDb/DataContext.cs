namespace Titans.SqlDb;
using Microsoft.EntityFrameworkCore;
using Titans.SqlDb.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DataContext() { }
    public DbSet<User> Users => Set<User>();
}