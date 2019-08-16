using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Persistent.Domain;

namespace Test.Persistent.Context.Configuration
{
    internal class OrganizationsDbConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.OrganizationId);
            builder.HasMany<Contract>()
                .WithOne()
                .HasForeignKey(c => c.ContractorId)
                .IsRequired();
        }
    }
}