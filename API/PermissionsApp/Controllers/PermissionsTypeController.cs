using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Domain.Interfaces;

namespace PermissionsApp.Controllers
{
    [ApiController]
    [Route("api/permissions-type")]
    public class PermissionsTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PermissionsTypeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionTypeDto>>> GetPermissionTypes()
        {
            var permissionTypes = await _unitOfWork.PermissionTypeRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PermissionTypeDto>>(permissionTypes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionTypeDto>> GetPermissionType(int id)
        {
            var permissionType = await _unitOfWork.PermissionTypeRepository.GetByIdAsync(id);

            if (permissionType == null)
                return NotFound();

            return Ok(_mapper.Map<PermissionTypeDto>(permissionType));
        }
    }
}
