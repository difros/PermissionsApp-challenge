﻿using AutoMapper;
using MediatR;
using PermissionsApp.Application.Commands;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Domain.Constants;
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
            // Validate if a permission with the same employee name and last name already exists
            var existingPermission = await _unitOfWork.PermissionRepository.GetByEmployeeNameAndLastNameAsync(
                request.Permission.EmployeeName, 
                request.Permission.EmployeeLastName);
                
            if (existingPermission != null)
            {
                throw new ApplicationException("A permission for this employee already exists.");
            }

            // Validate that the PermissionTypeId exists
            var permissionType = await _unitOfWork.PermissionTypeRepository.GetByIdAsync(request.Permission.PermissionTypeId);
            if (permissionType == null)
            {
                throw new ArgumentException($"Permission type with ID {request.Permission.PermissionTypeId} does not exist.");
            }


            Permission permission = _mapper.Map<Permission>(request.Permission);
            
            // Set the date to current date
            permission.Date = DateTime.Now;

            await _unitOfWork.PermissionRepository.AddAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            // Perform indexation on Elasticsearch
            await _elasticsearchService.IndexPermissionAsync(permission);

            // Send message to Kafka
            await _kafkaProducer.ProduceAsync(KafkaOperationType.Request);

            return _mapper.Map<PermissionDto>(permission);
        }
    }
}
