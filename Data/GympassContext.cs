using ApiGympass.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiGympass.Data
{
    public class GympassContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public GympassContext(DbContextOptions<GympassContext> options) : base(options)
        {
        }

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<CheckIn>()
                .HasKey(checkIn => new { checkIn.UserId, checkIn.GymId });

            builder.Entity<CheckIn>()
                .HasOne(checkIn => checkIn.User)
                .WithMany(user => user.CheckIns)
                .HasForeignKey(checkIn => checkIn.UserId);

            builder.Entity<CheckIn>()
                .HasOne(checkIn => checkIn.Gym)
                .WithMany(gym => gym.CheckIns)
                .HasForeignKey(checkIn => checkIn.GymId);
        }
        
    }
}