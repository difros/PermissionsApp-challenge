using PermissionsApp.Domain.Entities;
using PermissionsApp.Domain.Interfaces;
using PermissionsApp.Infraestructure.Persistence;

namespace PermissionsApp.Infraestructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IPermissionRepository _permissionRepository;
        private IRepository<PermissionType> _permissionTypeRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPermissionRepository PermissionRepository =>
            _permissionRepository ??= new PermissionRepository(_context);

        public IRepository<PermissionType> PermissionTypeRepository =>
            _permissionTypeRepository ??= new Repository<PermissionType>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
