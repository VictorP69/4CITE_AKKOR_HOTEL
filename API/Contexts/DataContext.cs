using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace API.Contexts
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DataContext() { }
        public DbSet<Hotel> Hotel => Set<Hotel>();
        public DbSet<HotelBlob> HotelBlob => Set<HotelBlob>();
        public DbSet<Booking> Booking => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HotelBlob>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<HotelBlob>()
                .HasOne(b => b.Hotel)
                .WithMany(h => h.PictureList)
                .HasForeignKey(b => b.HotelId);
        }
    }
}
