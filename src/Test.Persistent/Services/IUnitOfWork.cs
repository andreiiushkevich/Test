using System;

namespace Test.Persistent.Services
{
    interface IUnitOfWork : IDisposable
    {
        IContractsRepository ContractsRepository { get; }
        IOrganizationsRepository OrganizationsRepository { get; }
        int Complete();
    }
}