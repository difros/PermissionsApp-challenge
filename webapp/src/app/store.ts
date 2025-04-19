import { configureStore } from '@reduxjs/toolkit';
import permissionsReducer from '../features/permissions/permissionsSlice';
import permissionTypesReducer from '../features/permissionTypes/permissionTypesSlice';

export const store = configureStore({
  reducer: {
    permissions: permissionsReducer,
    permissionTypes: permissionTypesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch; 