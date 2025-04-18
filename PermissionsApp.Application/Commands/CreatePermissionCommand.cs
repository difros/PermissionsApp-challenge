using MediatR;
using PermissionsApp.Application.DTOs;

namespace PermissionsApp.Application.Commands
{
    public class CreatePermissionCommand : IRequest<PermissionDto>
    {
        public RequestPermissionDto Permission { get; }

        public CreatePermissionCommand(RequestPermissionDto pPermission)
        {
            Permission = pPermission;
        }
    }
}
