using AutoMapper;
using MediatR;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Application.Queries;
using PermissionsApp.Domain.Constants;
using PermissionsApp.Domain.Interfaces;

namespace PermissionsApp.Application.Handlers
{
    public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, PermissionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IKafkaProducer _kafkaProducer;

        public GetPermissionByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IKafkaProducer kafkaProducer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<PermissionDto> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permission = await _unitOfWork.PermissionRepository.GetByIdWithIncludeAsync(request.Id, p => p.PermissionType);

            if (permission == null)
                return null;

            // Send message to Kafka
            await _kafkaProducer.ProduceAsync($"{KafkaOperationType.Get}-{request.Id}");

            return _mapper.Map<PermissionDto>(permission);
        }
    }
}
