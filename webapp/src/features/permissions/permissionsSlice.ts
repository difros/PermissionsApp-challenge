import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { Permission, RequestPermission, UpdatePermission } from '../../types';
import {
  getPermissions,
  getPermissionById,
  createPermission,
  updatePermission,
} from '../../services/api';

interface PermissionsState {
  items: Permission[];
  currentPermission: Permission | null;
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  error: string | null;
}

const initialState: PermissionsState = {
  items: [],
  currentPermission: null,
  status: 'idle',
  error: null,
};

export const fetchPermissions = createAsyncThunk(
  'permissions/fetchPermissions',
  async () => {
    
    const response = await getPermissions();
    if (response.isError) {
      throw new Error(response.errorMessage || 'Failed to fetch permissions');
    }
    return response.data;
  }
);

export const fetchPermissionById = createAsyncThunk(
  'permissions/fetchPermissionById',
  async (id: number) => {
    const response = await getPermissionById(id);
    if (response.isError) {
      throw new Error(response.errorMessage || 'Failed to fetch permission');
    }
    return response.data;
  }
);

export const addNewPermission = createAsyncThunk(
  'permissions/addNewPermission',
  async (permission: RequestPermission) => {
    const response = await createPermission(permission);
    if (response.isError) {
      throw new Error(response.errorMessage || 'Failed to create permission');
    }
    return response.data;
  }
);

export const modifyPermission = createAsyncThunk(
  'permissions/modifyPermission',
  async ({ id, permission }: { id: number; permission: UpdatePermission }) => {
    const response = await updatePermission(id, permission);
    if (response.isError) {
      throw new Error(response.errorMessage || 'Failed to update permission');
    }
    return response.data;
  }
);

const permissionsSlice = createSlice({
  name: 'permissions',
  initialState,
  reducers: {
    resetCurrentPermission: (state) => {
      state.currentPermission = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all permissions
      .addCase(fetchPermissions.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchPermissions.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = action.payload || [];
      })
      .addCase(fetchPermissions.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Failed to fetch permissions';
      })
      
      // Fetch single permission
      .addCase(fetchPermissionById.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchPermissionById.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.currentPermission = action.payload || null;
      })
      .addCase(fetchPermissionById.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Failed to fetch permission';
      })
      
      // Add new permission
      .addCase(addNewPermission.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(addNewPermission.fulfilled, (state, action) => {
        state.status = 'succeeded';
        if (action.payload) {
          state.items.push(action.payload);
        }
      })
      .addCase(addNewPermission.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Failed to create permission';
      })
      
      // Update permission
      .addCase(modifyPermission.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(modifyPermission.fulfilled, (state, action) => {
        state.status = 'succeeded';
        if (action.payload) {
          const index = state.items.findIndex(item => item.id === action.payload?.id);
          if (index !== -1) {
            state.items[index] = action.payload;
          }
          state.currentPermission = action.payload;
        }
      })
      .addCase(modifyPermission.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Failed to update permission';
      });
  },
});

export const { resetCurrentPermission } = permissionsSlice.actions;
export default permissionsSlice.reducer; 