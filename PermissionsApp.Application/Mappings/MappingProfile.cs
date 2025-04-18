using AutoMapper;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Domain.Entities;

namespace PermissionsApp.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to DTO
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.PermissionTypeDescription, opt => opt.MapFrom(src => src.PermissionType != null ? src.PermissionType.Description : null));

            CreateMap<PermissionType, PermissionTypeDto>();

            // DTO to Domain
            CreateMap<RequestPermissionDto, Permission>();
            CreateMap<UpdatePermissionDto, Permission>();
        }
    }
}
