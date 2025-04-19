import axios from 'axios';
import { Permission, PermissionType, RequestPermission, ResultDto, UpdatePermission } from '../types';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api';

console.log('API URL configured as:', API_URL);

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Añadir interceptores para debugging
apiClient.interceptors.request.use(
  (config) => {
    console.log('Request:', config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => {
    console.error('Request error:', error);
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    console.error('Response error:', error.message);
    if (error.response) {
      console.error('Response data:', error.response.data);
      console.error('Response status:', error.response.status);
      console.error('Response headers:', error.response.headers);
      
      if (error.response.data && error.response.data.isError) {
        error.message = error.response.data.errorMessage || error.message;
      }
    } else if (error.request) {
      // The request was made but no response was received
      console.error('No response received:', error.request);
    }
    return Promise.reject(error);
  }
);

// Permission Types API
export const getPermissionTypes = async (): Promise<ResultDto<PermissionType[]>> => {
  try {
    const response = await apiClient.get<ResultDto<PermissionType[]>>('/permissions-type');
    return response.data;
  } catch (error) {
    console.error('Error fetching permission types:', error);
    throw error;
  }
};

export const getPermissionTypeById = async (id: number): Promise<ResultDto<PermissionType>> => {
  try {
    const response = await apiClient.get<ResultDto<PermissionType>>(`/permissions-type/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching permission type with id ${id}:`, error);
    throw error;
  }
};

// Permissions API
export const getPermissions = async (): Promise<ResultDto<Permission[]>> => {
  try {
    const response = await apiClient.get<ResultDto<Permission[]>>('/permissions');
    return response.data;
  } catch (error) {
    console.error('Error fetching permissions:', error);
    throw error;
  }
};

export const getPermissionById = async (id: number): Promise<ResultDto<Permission>> => {
  try {
    const response = await apiClient.get<ResultDto<Permission>>(`/permissions/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching permission with id ${id}:`, error);
    throw error;
  }
};

export const createPermission = async (permission: RequestPermission): Promise<ResultDto<Permission>> => {
  try {
    const response = await apiClient.post<ResultDto<Permission>>('/permissions/request', permission);
    return response.data;
  } catch (error) {
    console.error('Error creating permission:', error);
    throw error;
  }
};

export const updatePermission = async (id: number, permission: UpdatePermission): Promise<ResultDto<Permission>> => {
  try {
    const response = await apiClient.put<ResultDto<Permission>>(`/permissions/${id}`, permission);
    return response.data;
  } catch (error) {
    console.error(`Error updating permission with id ${id}:`, error);
    throw error;
  }
}; 