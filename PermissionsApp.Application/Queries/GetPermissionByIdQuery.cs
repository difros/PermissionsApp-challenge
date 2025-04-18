using MediatR;
using PermissionsApp.Application.DTOs;

namespace PermissionsApp.Application.Queries
{
    public class GetPermissionByIdQuery : IRequest<PermissionDto>
    {
        public int Id { get; }
        public GetPermissionByIdQuery(int id)
        {
            Id = id;
        }
    }
}
