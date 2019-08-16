using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Test.Common.Contracts;
using Test.Common.Dtos;
using Test.Persistent.Domain;

namespace Test.Persistent.Services
{
    internal class ContractConsumer : IConsumer<ContractDto>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ContractConsumer(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<ContractDto> context)
        {
            var contractDto = context.Message;
            var organization = _unitOfWork.OrganizationsRepository.Get(contractDto.ContractorId);

            if (organization == null)
            {
                await context.RespondAsync<IOperationResult<OperationStatus>>(new OperationResult<OperationStatus>(
                    new OperationStatus
                    {
                        Success = false,
                        Message =
                            $"Failed to add a new order. No contractor with id: {contractDto.ContractorId} in the database."
                    }));

                return;
            }

            var contract = _mapper.Map<Contract>(context.Message);

            _unitOfWork.ContractsRepository.Add(contract);
            _unitOfWork.Complete();

            await context.RespondAsync<IOperationResult<OperationStatus>>(new OperationResult<OperationStatus>(
                new OperationStatus
                {
                    Success = true,
                    Message = $"New contract with id: {contract.ContractId} was successfully added."
                }));
        }
    }
}