import React from 'react';
import { Provider } from 'react-redux';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { CssBaseline, ThemeProvider, createTheme, Container, Box, AppBar, Toolbar, Typography } from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { store } from './app/store';
import PermissionsList from './features/permissions/PermissionsList';
import PermissionForm from './features/permissions/PermissionForm';
import './App.css';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <CssBaseline />
          <Router>
            <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
              <AppBar position="static">
                <Toolbar>
                  <Typography variant="h6" component="div">
                    Permissions Application
                  </Typography>
                </Toolbar>
              </AppBar>
              <Container component="main" sx={{ mt: 4, mb: 4, flex: '1 0 auto' }}>
                <Routes>
                  <Route path="/" element={<Navigate replace to="/permissions" />} />
                  <Route path="/permissions" element={<PermissionsList />} />
                  <Route path="/permissions/new" element={<PermissionForm />} />
                  <Route path="/permissions/:id/edit" element={<PermissionForm />} />
                </Routes>
              </Container>
              <Box component="footer" sx={{ py: 3, bgcolor: 'grey.200' }}>
                <Container maxWidth="sm">
                  <Typography variant="body2" color="text.secondary" align="center">
                    Permissions App © {new Date().getFullYear()} <em>by Diego Rossi</em>
                  </Typography>
                </Container>
              </Box>
            </Box>
          </Router>
        </LocalizationProvider>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
