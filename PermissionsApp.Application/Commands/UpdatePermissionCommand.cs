using MediatR;
using PermissionsApp.Application.DTOs;

namespace PermissionsApp.Application.Commands
{
    public class UpdatePermissionCommand : IRequest<PermissionDto>
    {
        public UpdatePermissionDto Permission { get; }

        public UpdatePermissionCommand(UpdatePermissionDto pPermission)
        {
            Permission = pPermission;
        }
    }
}
