using PermissionsApp.Domain.Entities;

namespace PermissionsApp.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPermissionRepository PermissionRepository { get; }
        IRepository<PermissionType> PermissionTypeRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
