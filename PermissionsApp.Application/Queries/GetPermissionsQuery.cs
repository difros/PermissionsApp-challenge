using MediatR;
using PermissionsApp.Application.DTOs;

namespace PermissionsApp.Application.Queries
{
    public class GetPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
    {
    }
}
