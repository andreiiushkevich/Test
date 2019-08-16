using AutoMapper;
using Test.Common.Dtos;
using Test.Persistent.Domain;

namespace Test.Persistent.Mapping
{
    internal class ContractProfile : Profile
    {
        public ContractProfile()
        {
            CreateMap<ContractDto, Contract>();
        }
    }
}