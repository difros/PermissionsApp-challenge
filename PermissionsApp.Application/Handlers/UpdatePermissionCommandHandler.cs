using AutoMapper;
using MediatR;
using PermissionsApp.Application.Commands;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Domain.Constants;
using PermissionsApp.Domain.Interfaces;

namespace PermissionsApp.Application.Handlers
{
    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IKafkaProducer _kafkaProducer;

        public UpdatePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IElasticsearchService elasticsearchService,
            IKafkaProducer kafkaProducer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<PermissionDto> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var existingPermission = await _unitOfWork.PermissionRepository.GetByIdAsync(request.Permission.Id);

            if (existingPermission == null)
                throw new KeyNotFoundException($"Permission with id {request.Permission.Id} not found.");

            // Validate that the PermissionTypeId exists
            var permissionType = await _unitOfWork.PermissionTypeRepository.GetByIdAsync(request.Permission.PermissionTypeId);
            if (permissionType == null)
            {
                throw new ArgumentException($"Permission type with ID {request.Permission.PermissionTypeId} does not exist.");
            }

            _mapper.Map(request.Permission, existingPermission);

            await _unitOfWork.PermissionRepository.UpdateAsync(existingPermission);
            await _unitOfWork.SaveChangesAsync();

            // Update Elasticsearch
            await _elasticsearchService.IndexPermissionAsync(existingPermission);

            // Send message to Kafka
            await _kafkaProducer.ProduceAsync(KafkaOperationType.Modify);

            return _mapper.Map<PermissionDto>(existingPermission);
        }
    }
}
