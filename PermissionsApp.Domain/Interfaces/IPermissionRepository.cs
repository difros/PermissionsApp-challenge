using PermissionsApp.Domain.Entities;

namespace PermissionsApp.Domain.Interfaces
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission?> GetByEmployeeNameAndLastNameAsync(string pEmployeeName, string pEmployeeLastName);
    }
}