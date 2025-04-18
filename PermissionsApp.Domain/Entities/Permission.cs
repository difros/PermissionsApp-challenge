namespace PermissionsApp.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeLastName { get; set; } = string.Empty;
        public int PermissionTypeId { get; set; }
        public virtual PermissionType? PermissionType { get; set; }
        public DateTime Date { get; set; }
    }
}
