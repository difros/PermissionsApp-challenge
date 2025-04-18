using AutoMapper;
using MediatR;
using PermissionsApp.Application.Commands;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Domain.Entities;
using PermissionsApp.Domain.Interfaces;

namespace PermissionsApp.Application.Handlers
{
    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IKafkaProducer _kafkaProducer;

        public CreatePermissionCommandHandler(
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

        public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            Permission permission = _mapper.Map<Permission>(request.Permission);

            await _unitOfWork.PermissionRepository.AddAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            // Perform indexation on Elasticsearch
            await _elasticsearchService.IndexPermissionAsync(permission);

            // Send message to Kafka
            await _kafkaProducer.ProduceAsync("request");

            return _mapper.Map<PermissionDto>(permission);
        }
    }
}
