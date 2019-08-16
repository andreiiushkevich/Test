using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Test.Receiver.Hubs
{
    public class ContractHub : Hub
    {
        public Task ContractProcessed(string message)
        {
            return Clients.All.SendAsync(message);
        }
    }
}