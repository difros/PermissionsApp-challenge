using MediatR;
using PermissionsApp.Application.DTOs;

namespace PermissionsApp.Application.Commands
{
    public class CreatePermissionCommand : IRequest<PermissionDto>
    {
        public CreatePermissionDto Permission { get; }

        public CreatePermissionCommand(CreatePermissionDto pPermission)
        {
            Permission = pPermission;
        }
    }
}
