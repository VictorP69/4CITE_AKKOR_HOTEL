using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DataContext() { }
        public DbSet<Hotel> Hotel => Set<Hotel>();
    }
}
