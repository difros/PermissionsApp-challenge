using Microsoft.EntityFrameworkCore;
using PermissionsApp.Domain.Entities;
using PermissionsApp.Domain.Interfaces;
using PermissionsApp.Infraestructure.Persistence;

namespace PermissionsApp.Infraestructure.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Permission?> GetByEmployeeNameAndLastNameAsync(string pEmployeeName, string pEmployeeLastName)
        {
            return await _dbSet.FirstOrDefaultAsync(p => 
                p.EmployeeName == pEmployeeName && 
                p.EmployeeLastName == pEmployeeLastName);
        }
    }
}