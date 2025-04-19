import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { PermissionType } from '../../types';
import { getPermissionTypes } from '../../services/api';

interface PermissionTypesState {
  items: PermissionType[];
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  error: string | null;
}

const initialState: PermissionTypesState = {
  items: [],
  status: 'idle',
  error: null,
};

export const fetchPermissionTypes = createAsyncThunk(
  'permissionTypes/fetchPermissionTypes',
  async () => {
    const response = await getPermissionTypes();
    if (response.isError) {
      throw new Error(response.errorMessage || 'Failed to fetch permission types');
    }
    return response.data;
  }
);

const permissionTypesSlice = createSlice({
  name: 'permissionTypes',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchPermissionTypes.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchPermissionTypes.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = action.payload || [];
      })
      .addCase(fetchPermissionTypes.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Failed to fetch permission types';
      });
  },
});

export default permissionTypesSlice.reducer; 