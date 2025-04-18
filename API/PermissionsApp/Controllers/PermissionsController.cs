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
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
            var query = new GetPermissionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermission(int id)
        {
            var query = new GetPermissionByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto pCreatePermissionDto)
        {
            var command = new CreatePermissionCommand(pCreatePermissionDto);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPermission), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PermissionDto>> UpdatePermission(int pId, UpdatePermissionDto pUpdatePermissionDto)
        {
            if (pId != pUpdatePermissionDto.Id)
                return BadRequest();

            var command = new UpdatePermissionCommand(pUpdatePermissionDto);

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            } catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
