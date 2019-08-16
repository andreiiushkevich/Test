using System;

namespace Test.Common.Dtos
{
    public class ContractDto
    {
        public DateTime CreationDate { get; set; }
        public string Number { get; set; }
        public decimal? Price { get; set; }
        public long ConsumerId { get; set; }
        public long ContractorId { get; set; }
    }
}