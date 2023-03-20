using JobBoard.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.Repositories
{
    public class JbDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<UserToOffer> UserToOffers { get; set; }

        public JbDbContext()
        {
            this.Users = this.Set<User>();
            this.Offers = this.Set<Offer>();
            this.UserToOffers = this.Set<UserToOffer>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost; Database=JbDB; User Id=stanislava; Password=12345;")
                          .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    Username = "admin",
                    Password = "12345",
                    FirstName = "Admin",
                    LastName = "Admin"
                });
        }
    }
}
