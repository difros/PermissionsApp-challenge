import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  Container,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import { Add, Edit } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { fetchPermissions } from './permissionsSlice';
import { fetchPermissionTypes } from '../permissionTypes/permissionTypesSlice';
import LoadingSpinner from '../../components/LoadingSpinner';
import ErrorDisplay from '../../components/ErrorDisplay';

const PermissionsList: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { items: permissions, status, error } = useAppSelector((state) => state.permissions);
  const { items: permissionTypes } = useAppSelector((state) => state.permissionTypes);

  useEffect(() => {
    if (status === 'idle') {
      dispatch(fetchPermissions());
    }
    if (permissionTypes.length === 0) {
      dispatch(fetchPermissionTypes());
    }
  }, [status, dispatch, permissionTypes.length]);

  const handleAddNew = () => {
    navigate('/permissions/new');
  };

  const handleEdit = (id: number) => {
    navigate(`/permissions/${id}/edit`);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (status === 'loading') {
    return <LoadingSpinner />;
  }

  if (status === 'failed') {
    return <ErrorDisplay message={error || 'Error loading permissions'} />;
  }

  return (
    <Container>
      <Box sx={{ mt: 4, mb: 4, display: 'flex', justifyContent: 'space-between' }}>
        <Typography variant="h4">Permissions</Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<Add />}
          onClick={handleAddNew}
        >
          Request Permission
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Last Name</TableCell>
              <TableCell>Date</TableCell>
              <TableCell>Permission Type</TableCell>
              <TableCell>Update</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {permissions.map((permission) => (
              <TableRow key={permission.id}>
                <TableCell>{permission.id}</TableCell>
                <TableCell>{permission.employeeName}</TableCell>
                <TableCell>{permission.employeeLastName}</TableCell>
                <TableCell>{formatDate(permission.date)}</TableCell>
                <TableCell>{permission.permissionTypeDescription}</TableCell>
                <TableCell>
                  <Button
                    size="small"
                    startIcon={<Edit />}
                    onClick={() => handleEdit(permission.id)}
                  >
                    Edit
                  </Button>
                </TableCell>
              </TableRow>
            ))}
            {permissions.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  No permissions registered
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>
  );
};

export default PermissionsList; 