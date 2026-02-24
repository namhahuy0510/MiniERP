using Microsoft.EntityFrameworkCore;
using MiniERP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MiniERP.Data
{
    public class MiniERPContext : IdentityDbContext<ApplicationUser>
    {
        public MiniERPContext(DbContextOptions<MiniERPContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Attendance> Attendance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Attendance>().ToTable("Attendance");

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .IsRequired();
        }
    }
}
