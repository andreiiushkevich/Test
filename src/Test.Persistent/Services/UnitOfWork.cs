using Test.Persistent.Context;

namespace Test.Persistent.Services
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly TestContext _context;

        public UnitOfWork(TestContext context,
            IContractsRepository contractsRepository,
            IOrganizationsRepository organizationsRepository)
        {
            _context = context;
            ContractsRepository = contractsRepository;
            OrganizationsRepository = organizationsRepository;
        }

        public IContractsRepository ContractsRepository { get; }
        public IOrganizationsRepository OrganizationsRepository { get; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}