using PermissionsApp.Domain.Entities;

namespace PermissionsApp.Domain.Interfaces
{
    public interface IElasticsearchService
    {
        Task IndexPermissionAsync(Permission permission);
        Task InitializeIndexAsync();
        Task<IEnumerable<Permission>> SearchPermissionsAsync(string searchTerm);
    }
}
