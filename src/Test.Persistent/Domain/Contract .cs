using System;

namespace Test.Persistent.Domain
{
    class Contract
    {
        public long ContractId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Number { get; set; }
        public decimal? Price { get; set; }
        public long ConsumerId { get; set; }
        public long ContractorId { get; set; }
    }
}