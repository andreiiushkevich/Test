using Microsoft.EntityFrameworkCore;
using Test.Persistent.Context.Configuration;
using Test.Persistent.Domain;

namespace Test.Persistent.Context
{
    class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {

        }

        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ContractsDbConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationsDbConfiguration());

            modelBuilder.Entity<Organization>().HasData(
                new Organization {OrganizationId = 1, FullName = "Organization 1", Name = "Organization 1" },
                new Organization {OrganizationId = 2, FullName = "Organization 2", Name = "Organization 2" },
                new Organization {OrganizationId = 3, FullName = "Organization 3", Name = "Organization 3" }
            );
        }
    }
}