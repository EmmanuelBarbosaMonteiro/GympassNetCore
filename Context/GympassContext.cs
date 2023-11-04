using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGympass.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiGympass.Context
{
    public class GympassContext : DbContext
    {
        public GympassContext(DbContextOptions<GympassContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        
    }
}