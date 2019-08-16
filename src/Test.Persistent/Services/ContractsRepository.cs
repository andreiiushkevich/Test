using Microsoft.EntityFrameworkCore;
using Test.Persistent.Domain;

namespace Test.Persistent.Services
{
    class ContractsRepository : Repository<Contract>, IContractsRepository
    {
        public ContractsRepository(DbContext context) : base(context)
        {
        }
    }
}