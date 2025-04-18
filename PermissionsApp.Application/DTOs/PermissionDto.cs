using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsApp.Application.DTOs
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeLastName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int PermissionTypeId { get; set; }
        public string? PermissionTypeDescription { get; set; }

    }
}
