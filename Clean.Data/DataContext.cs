
using Clean.Core.Entities;
using Microsoft.EntityFrameworkCore;
using static Clean.Core.Entities.User;

namespace Clean.Data
{

    public class DataContext:DbContext
    {
        public DbSet<Cleaners> Cleaners { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<BlockedDate> BlockedDates { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Clean_DB;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // הגדרה לטבלת הלקוחות - ביטול מחיקה משורשרת
            modelBuilder.Entity<Customers>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // זה מה שפותר את השגיאה

            // הגדרה לטבלת המנקים - ביטול מחיקה משורשרת
            modelBuilder.Entity<Cleaners>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Bookings>(b =>
            {
                b.Property(x => x.Status).HasConversion<string>().HasDefaultValue(BookingStatus.Confirmed);
                b.Property(x => x.CancelledBy).HasConversion<string>();
                // SQL Server returns datetime2 without a time zone; mark it as UTC so the API serializes it with a "Z".
                b.Property(x => x.CancelledAt).HasConversion(
                    v => v,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            });

            modelBuilder.Entity<BlockedDate>()
                .HasIndex(b => new { b.CleanerId, b.Date })
                .IsUnique();

            modelBuilder.Entity<User>(b =>
            {
                b.Property(e => e.Role)
                    .HasConversion(
                        v => v.ToString(), 
                        v => Enum.Parse<eRole>(v)); 
            });


        }

    }
}