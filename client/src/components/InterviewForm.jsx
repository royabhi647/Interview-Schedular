import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  TextField,
  Button,
  Typography,
  Box,
  Grid,
  Alert,
  CircularProgress,
  Card,
  CardContent
} from '@mui/material';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import axios from 'axios';
import GoogleIcon from '@mui/icons-material/Google';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';

dayjs.extend(utc);
dayjs.extend(timezone);

const API_BASE_URL = import.meta.env.VITE_APP_API_URL || 'http://localhost:5180';

const InterviewForm = () => {
  const [formData, setFormData] = useState({
    jobTitle: '',
    candidateName: '',
    candidateEmail: '',
    interviewerName: '',
    interviewerEmail: '',
    startTime: dayjs().add(1, 'day').hour(10).minute(0),
    endTime: dayjs().add(1, 'day').hour(11).minute(0)
  });

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [authLoading, setAuthLoading] = useState(true);

  useEffect(() => {
    checkAuthStatus();
    handleAuthCallback();
  }, []);

  const checkAuthStatus = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/Auth/status`);
      setIsAuthenticated(response.data.isAuthenticated);
    } catch (error) {
      console.error('Failed to check auth status:', error);
      setIsAuthenticated(false);
    } finally {
      setAuthLoading(false);
    }
  };

  const handleAuthCallback = () => {
    const urlParams = new URLSearchParams(window.location.search);
    const authStatus = urlParams.get('auth');
    
    if (authStatus === 'success') {
      setMessage({ type: 'success', text: 'Google authentication successful!' });
      setIsAuthenticated(true);
      // Clean up URL
      window.history.replaceState({}, document.title, window.location.pathname);
    } else if (authStatus === 'error') {
      const errorMessage = urlParams.get('message') || 'Authentication failed';
      setMessage({ type: 'error', text: `Authentication failed: ${decodeURIComponent(errorMessage)}` });
      // Clean up URL
      window.history.replaceState({}, document.title, window.location.pathname);
    }
  };

  const handleInputChange = (field) => (event) => {
    setFormData({
      ...formData,
      [field]: event.target.value
    });
  };

  const handleDateChange = (field) => (newValue) => {
    setFormData({
      ...formData,
      [field]: newValue
    });
    
    // Auto-adjust end time when start time changes
    if (field === 'startTime' && newValue) {
      setFormData(prev => ({
        ...prev,
        [field]: newValue,
        endTime: newValue.add(1, 'hour')
      }));
    }
  };

  const handleGoogleAuth = async () => {
    try {
      setMessage({ type: '', text: '' });
      const response = await axios.get(`${API_BASE_URL}/api/Auth/google-login`);
      window.location.href = response.data.authUrl;
    } catch (error) {
      setMessage({ type: 'error', text: 'Failed to initiate Google authentication' });
    }
  };

  const validateForm = () => {
    if (formData.endTime.isBefore(formData.startTime)) {
      setMessage({ type: 'error', text: 'End time must be after start time' });
      return false;
    }
    
    if (formData.startTime.isBefore(dayjs())) {
      setMessage({ type: 'error', text: 'Interview must be scheduled for future time' });
      return false;
    }

    return true;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    
    if (!validateForm()) return;
    
    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      const response = await axios.post(`${API_BASE_URL}/api/Interview`, {
        ...formData,
        startTime: formData.startTime.utc().toISOString(),
        endTime: formData.endTime.utc().toISOString()
      });

      setMessage({ type: 'success', text: 'Interview scheduled successfully! Notifications have been sent to all participants.' });
      
      // Reset form
      setFormData({
        jobTitle: '',
        candidateName: '',
        candidateEmail: '',
        interviewerName: '',
        interviewerEmail: '',
        startTime: dayjs().add(1, 'day').hour(10).minute(0),
        endTime: dayjs().add(1, 'day').hour(11).minute(0)
      });
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to schedule interview';
      setMessage({ type: 'error', text: errorMessage });
    } finally {
      setLoading(false);
    }
  };

  if (authLoading) {
    return (
      <Container maxWidth="md" sx={{ py: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center" color="primary">
          Schedule New Interview
        </Typography>
        
        {!isAuthenticated ? (
          <Card sx={{ mb: 3, bgcolor: 'primary.50' }}>
            <CardContent sx={{ textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>
                Google Calendar Integration Required
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Please connect your Google account to automatically create calendar events and Google Meet links
              </Typography>
              <Button 
                variant="contained" 
                onClick={handleGoogleAuth}
                size="large"
                startIcon={<GoogleIcon />}
                sx={{ minWidth: 200 }}
              >
                Connect Google Calendar
              </Button>
            </CardContent>
          </Card>
        ) : (
          <Alert 
            severity="success" 
            icon={<CheckCircleIcon />}
            sx={{ mb: 3 }}
          >
            Google Calendar connected successfully! You can now schedule interviews.
          </Alert>
        )}

        {message.text && (
          <Alert severity={message.type} sx={{ mb: 3 }}>
            {message.text}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Job Title"
                placeholder="e.g., Senior Software Engineer"
                value={formData.jobTitle}
                onChange={handleInputChange('jobTitle')}
                required
                disabled={!isAuthenticated}
              />
            </Grid>
            
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Candidate's Name"
                placeholder="e.g., John Doe"
                value={formData.candidateName}
                onChange={handleInputChange('candidateName')}
                required
                disabled={!isAuthenticated}
              />
            </Grid>
            
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Candidate's Email"
                type="email"
                placeholder="candidate@example.com"
                value={formData.candidateEmail}
                onChange={handleInputChange('candidateEmail')}
                required
                disabled={!isAuthenticated}
              />
            </Grid>
            
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Interviewer's Name"
                placeholder="e.g., Jane Smith"
                value={formData.interviewerName}
                onChange={handleInputChange('interviewerName')}
                required
                disabled={!isAuthenticated}
              />
            </Grid>
            
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Interviewer's Email"
                type="email"
                placeholder="interviewer@company.com"
                value={formData.interviewerEmail}
                onChange={handleInputChange('interviewerEmail')}
                required
                disabled={!isAuthenticated}
              />
            </Grid>
            
            <Grid item xs={12} sm={6}>
              <DateTimePicker
                label="Start Time"
                value={formData.startTime}
                onChange={handleDateChange('startTime')}
                disabled={!isAuthenticated}
                renderInput={(params) => <TextField {...params} fullWidth required />}
                minDateTime={dayjs()}
              />
            </Grid>
            
            <Grid item xs={12} sm={6}>
              <DateTimePicker
                label="End Time"
                value={formData.endTime}
                onChange={handleDateChange('endTime')}
                disabled={!isAuthenticated}
                renderInput={(params) => <TextField {...params} fullWidth required />}
                minDateTime={formData.startTime}
              />
            </Grid>
            
            <Grid item xs={12}>
              <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
                <Button
                  type="submit"
                  variant="contained"
                  size="large"
                  disabled={loading || !isAuthenticated}
                  sx={{ minWidth: 200, height: 48 }}
                >
                  {loading ? (
                    <CircularProgress size={24} color="inherit" />
                  ) : (
                    'Schedule Interview'
                  )}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </form>
      </Paper>
    </Container>
  );
};

export default InterviewForm;