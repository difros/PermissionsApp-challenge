using AutoMapper;
using MediatR;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Application.Queries;
using PermissionsApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using PermissionsApp.Domain.Constants;

namespace PermissionsApp.Application.Handlers
{
    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IKafkaProducer _kafkaProducer;

        public GetPermissionsQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IKafkaProducer kafkaProducer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.PermissionRepository.GetAllWithIncludeAsync(p => p.PermissionType);
            
            // Send message to Kafka
            await _kafkaProducer.ProduceAsync(KafkaOperationType.Get);

            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }
    }
}
