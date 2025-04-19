using MediatR;
using Microsoft.AspNetCore.Mvc;
using PermissionsApp.Application.Commands;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Application.Queries;

namespace PermissionsApp.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto<IEnumerable<PermissionDto>>>> GetPermissions()
        {
            try
            {
                var query = new GetPermissionsQuery();
                var result = await _mediator.Send(query);
                return Ok(ResultDto<IEnumerable<PermissionDto>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<IEnumerable<PermissionDto>>.Failure(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto<PermissionDto>>> GetPermission(int id)
        {
            try
            {
                var query = new GetPermissionByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound(ResultDto<PermissionDto>.Failure($"Permission with ID {id} not found"));

                return Ok(ResultDto<PermissionDto>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<PermissionDto>.Failure(ex.Message));
            }
        }

        [HttpPost("request")]
        public async Task<ActionResult<ResultDto<PermissionDto>>> CreatePermission(RequestPermissionDto pCreatePermissionDto)
        {
            try
            {
                if (pCreatePermissionDto.EmployeeName == String.Empty || 
                    pCreatePermissionDto.EmployeeName == null || 
                    pCreatePermissionDto.EmployeeName.Length > 100 ||
                    pCreatePermissionDto.EmployeeLastName == String.Empty ||
                    pCreatePermissionDto.EmployeeLastName == null ||
                    pCreatePermissionDto.EmployeeLastName.Length > 100)
                    return BadRequest(ResultDto<PermissionDto>.Failure("Invalid request data"));

                var command = new CreatePermissionCommand(pCreatePermissionDto);
                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetPermission), 
                    new { id = result.Id }, 
                    ResultDto<PermissionDto>.Success(result));
            }
            catch (ApplicationException ex)
            {
                return Conflict(ResultDto<PermissionDto>.Failure(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResultDto<PermissionDto>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<PermissionDto>.Failure(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResultDto<PermissionDto>>> UpdatePermission(int id, UpdatePermissionDto pUpdatePermissionDto)
        {
            if (id != pUpdatePermissionDto.Id)
                return BadRequest(ResultDto<PermissionDto>.Failure("ID in URL does not match ID in request body"));
            
            if (pUpdatePermissionDto.EmployeeName == String.Empty ||
                    pUpdatePermissionDto.EmployeeName == null ||
                    pUpdatePermissionDto.EmployeeName.Length > 100 ||
                    pUpdatePermissionDto.EmployeeLastName == String.Empty ||
                    pUpdatePermissionDto.EmployeeLastName == null ||
                    pUpdatePermissionDto.EmployeeLastName.Length > 100)
                return BadRequest(ResultDto<PermissionDto>.Failure("Invalid request data"));

            try
            {
                var command = new UpdatePermissionCommand(pUpdatePermissionDto);
                var result = await _mediator.Send(command);
                return Ok(ResultDto<PermissionDto>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResultDto<PermissionDto>.Failure(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResultDto<PermissionDto>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<PermissionDto>.Failure(ex.Message));
            }
        }
    }
}
