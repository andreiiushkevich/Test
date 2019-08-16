using Microsoft.EntityFrameworkCore;
using Test.Persistent.Domain;

namespace Test.Persistent.Services
{
    class OrganizationsRepository : Repository<Organization>, IOrganizationsRepository
    {
        public OrganizationsRepository(DbContext context) : base(context)
        {
        }
    }
}