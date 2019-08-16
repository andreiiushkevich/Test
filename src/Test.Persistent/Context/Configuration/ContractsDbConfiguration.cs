using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Persistent.Domain;

namespace Test.Persistent.Context.Configuration
{
    internal class ContractsDbConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.HasKey(c => c.ContractId);
        }
    }
}