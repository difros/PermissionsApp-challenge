import React from 'react';
import { Alert, Box } from '@mui/material';

interface ErrorDisplayProps {
  message: string;
}

const ErrorDisplay: React.FC<ErrorDisplayProps> = ({ message }) => {
  return (
    <Box sx={{ p: 2 }}>
      <Alert severity="error">{message}</Alert>
    </Box>
  );
};

export default ErrorDisplay; 