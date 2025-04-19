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
        public async Task<ActionResult<ResultDto<IEnumerable<PermissionTypeDto>>>> GetPermissionTypes()
        {
            try
            {
                var permissionTypes = await _unitOfWork.PermissionTypeRepository.GetAllAsync();
                var mappedTypes = _mapper.Map<IEnumerable<PermissionTypeDto>>(permissionTypes);
                return Ok(ResultDto<IEnumerable<PermissionTypeDto>>.Success(mappedTypes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<IEnumerable<PermissionTypeDto>>.Failure(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto<PermissionTypeDto>>> GetPermissionType(int id)
        {
            try
            {
                var permissionType = await _unitOfWork.PermissionTypeRepository.GetByIdAsync(id);

                if (permissionType == null)
                    return NotFound(ResultDto<PermissionTypeDto>.Failure($"Permission Type with ID {id} not found"));

                var mappedType = _mapper.Map<PermissionTypeDto>(permissionType);
                return Ok(ResultDto<PermissionTypeDto>.Success(mappedType));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<PermissionTypeDto>.Failure(ex.Message));
            }
        }
    }
}
