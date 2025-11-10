
using Clean.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clean.Data
{

    public class DataContext:DbContext
    {
        public DbSet<Cleaners> Cleaners { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Schedule> Schedule { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Clean_DB");
        }

    }
}