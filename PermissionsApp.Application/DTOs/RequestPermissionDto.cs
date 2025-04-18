namespace PermissionsApp.Application.DTOs
{
    public class RequestPermissionDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeLastName { get; set; } = string.Empty;
        public int PermissionTypeId { get; set; }
    }
}
