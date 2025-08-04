import React from 'react';
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';

const Navbar = () => {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <AppBar position="static" elevation={2}>
      <Toolbar>
        <CalendarTodayIcon sx={{ mr: 2 }} />
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Interview Scheduler
        </Typography>
        <Box>
          <Button 
            color="inherit" 
            onClick={() => navigate('/')}
            variant={location.pathname === '/' ? 'outlined' : 'text'}
            sx={{ mr: 1 }}
          >
            Schedule Interview
          </Button>
          <Button 
            color="inherit" 
            onClick={() => navigate('/dashboard')}
            variant={location.pathname === '/dashboard' ? 'outlined' : 'text'}
          >
            Dashboard
          </Button>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;