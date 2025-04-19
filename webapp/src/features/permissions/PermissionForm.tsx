import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Button,
  Container,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  TextField,
  Typography,
  Alert,
  Stack,
  SelectChangeEvent,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import {
  fetchPermissionById,
  addNewPermission,
  modifyPermission,
  resetCurrentPermission,
} from './permissionsSlice';
import { fetchPermissionTypes } from '../permissionTypes/permissionTypesSlice';
import LoadingSpinner from '../../components/LoadingSpinner';
import ErrorDisplay from '../../components/ErrorDisplay';
import { Permission, RequestPermission, UpdatePermission } from '../../types';

const PermissionForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const isEditMode = id !== undefined && id !== 'new';
  const permissionId = isEditMode ? parseInt(id as string, 10) : 0;
  const [hasRequestedPermission, setHasRequestedPermission] = useState(false);

  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { currentPermission, status, error } = useAppSelector((state) => state.permissions);
  const { items: permissionTypes, status: typesStatus } = useAppSelector(
    (state) => state.permissionTypes
  );

  const [formData, setFormData] = useState<Partial<Permission>>({
    employeeName: '',
    employeeLastName: '',
    permissionTypeId: 0,
    date: new Date().toISOString(),
  });
  const [formErrors, setFormErrors] = useState<Record<string, string>>({});
  const [apiError, setApiError] = useState<string | null>(null);

  useEffect(() => {
    if (permissionTypes.length === 0 && typesStatus === 'idle') {
      dispatch(fetchPermissionTypes());
    }
  }, [dispatch, permissionTypes.length, typesStatus]);

  useEffect(() => {
    if (isEditMode && !currentPermission && !hasRequestedPermission) {
      setHasRequestedPermission(true);
      dispatch(fetchPermissionById(permissionId));
    }
  }, [dispatch, isEditMode, permissionId, currentPermission, hasRequestedPermission]);

  useEffect(() => {
    if (isEditMode && currentPermission) {
      setFormData({
        id: currentPermission.id,
        employeeName: currentPermission.employeeName,
        employeeLastName: currentPermission.employeeLastName,
        permissionTypeId: currentPermission.permissionTypeId,
        date: currentPermission.date,
      });
    }
  }, [isEditMode, currentPermission]);

  // Cleanup effect
  useEffect(() => {
    return () => {
      if (isEditMode) {
        dispatch(resetCurrentPermission());
      }
    };
  }, [dispatch, isEditMode]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | { name?: string; value: unknown }>) => {
    const { name, value } = e.target;
    if (name) {
      setFormData({
        ...formData,
        [name]: value,
      });
      
      // Clear error for this field
      if (formErrors[name]) {
        setFormErrors({
          ...formErrors,
          [name]: '',
        });
      }
    }
  };

  const handleSelectChange = (e: SelectChangeEvent<number>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name as string]: value,
    });
    
    // Clear error for this field
    if (name && formErrors[name]) {
      setFormErrors({
        ...formErrors,
        [name]: '',
      });
    }
  };

  const handleDateChange = (date: Date | null) => {
    if (date) {
      setFormData({
        ...formData,
        date: date.toISOString(),
      });
      
      // Clear date error
      if (formErrors.date) {
        setFormErrors({
          ...formErrors,
          date: '',
        });
      }
    }
  };

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.employeeName?.trim()) {
      errors.employeeName = 'Name is required';
    } else if (formData.employeeName.length > 100) {
      errors.employeeName = 'Name cannot exceed 100 characters';
    }

    if (!formData.employeeLastName?.trim()) {
      errors.employeeLastName = 'Last name is required';
    } else if (formData.employeeLastName.length > 100) {
      errors.employeeLastName = 'Last name cannot exceed 100 characters';
    }

    if (!formData.permissionTypeId || formData.permissionTypeId <= 0) {
      errors.permissionTypeId = 'You must select a permission type';
    }

    if (!formData.date) {
      errors.date = 'Date is required';
    }

    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }
    
    // Clear previous API error
    setApiError(null);

    try {
      if (isEditMode && formData.id) {
        const updateData: UpdatePermission = {
          id: formData.id,
          employeeName: formData.employeeName || '',
          employeeLastName: formData.employeeLastName || '',
          date: formData.date || new Date().toISOString(),
          permissionTypeId: formData.permissionTypeId || 0,
        };

        await dispatch(
          modifyPermission({ id: formData.id, permission: updateData })
        ).unwrap();
      } else {
        const createData: RequestPermission = {
          employeeName: formData.employeeName || '',
          employeeLastName: formData.employeeLastName || '',
          permissionTypeId: formData.permissionTypeId || 0,
        };

        await dispatch(addNewPermission(createData)).unwrap();
      }

      navigate('/permissions');
    } catch (err: unknown) {
      console.error('Failed to save permission:', err);
      
      if (err && typeof err === 'object' && 'message' in err) {
        setApiError((err as { message: string }).message);
      } else {
        setApiError('Error saving permission');
      }
    }
  };

  const handleCancel = () => {
    navigate('/permissions');
  };

  if ((isEditMode && status === 'loading') || typesStatus === 'loading') {
    return <LoadingSpinner />;
  }

  if ((isEditMode && status === 'failed') || typesStatus === 'failed') {
    return <ErrorDisplay message={error || 'Error loading data'} />;
  }

  return (
    <Container maxWidth="md">
      <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
        <Typography variant="h5" gutterBottom>
          {isEditMode ? 'Edit Permission' : 'Request Permission'}
        </Typography>

        {apiError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {apiError}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
          <Stack spacing={3}>
            <Box sx={{ display: 'flex', gap: 2 }}>
              <TextField
                name="employeeName"
                label="Employee Name"
                fullWidth
                value={formData.employeeName || ''}
                onChange={handleInputChange}
                error={!!formErrors.employeeName}
                helperText={formErrors.employeeName}
              />
              <TextField
                name="employeeLastName"
                label="Employee Last Name"
                fullWidth
                value={formData.employeeLastName || ''}
                onChange={handleInputChange}
                error={!!formErrors.employeeLastName}
                helperText={formErrors.employeeLastName}
              />
            </Box>
            
            <Box sx={{ display: 'flex', gap: 2 }}>
              <FormControl fullWidth error={!!formErrors.permissionTypeId}>
                <InputLabel>Permission Type</InputLabel>
                <Select
                  name="permissionTypeId"
                  value={formData.permissionTypeId || ''}
                  onChange={handleSelectChange}
                  label="Permission Type"
                >
                  <MenuItem value="">
                    <em>Select a type</em>
                  </MenuItem>
                  {permissionTypes.map((type) => (
                    <MenuItem key={type.id} value={type.id}>
                      {type.description}
                    </MenuItem>
                  ))}
                </Select>
                {formErrors.permissionTypeId && (
                  <Alert severity="error" sx={{ mt: 1 }}>
                    {formErrors.permissionTypeId}
                  </Alert>
                )}
              </FormControl>
              
              {isEditMode && (
                <Box sx={{ flex: 1 }}>
                  <LocalizationProvider dateAdapter={AdapterDateFns}>
                    <DatePicker
                      label="Date"
                      value={formData.date ? new Date(formData.date) : null}
                      onChange={handleDateChange}
                      slotProps={{
                        textField: {
                          fullWidth: true,
                          error: !!formErrors.date,
                          helperText: formErrors.date,
                        },
                      }}
                    />
                  </LocalizationProvider>
                </Box>
              )}
            </Box>
            
            <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2, mt: 3 }}>
              <Button variant="outlined" onClick={handleCancel}>
                Cancel
              </Button>
              <Button type="submit" variant="contained" color="primary">
                {isEditMode ? 'Update' : 'Create'}
              </Button>
            </Box>
          </Stack>
        </Box>
      </Paper>
    </Container>
  );
};

export default PermissionForm; 