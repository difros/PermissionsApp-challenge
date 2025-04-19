export interface PermissionType {
  id: number;
  description: string;
}

export interface Permission {
  id: number;
  employeeName: string;
  employeeLastName: string;
  date: string;
  permissionTypeId: number;
  permissionTypeDescription?: string;
}

export interface RequestPermission {
  employeeName: string;
  employeeLastName: string;
  permissionTypeId: number;
}

export interface UpdatePermission {
  id: number;
  employeeName: string;
  employeeLastName: string;
  date: string;
  permissionTypeId: number;
}

export interface ResultDto<T> {
  isError: boolean;
  data?: T;
  errorMessage?: string;
} 