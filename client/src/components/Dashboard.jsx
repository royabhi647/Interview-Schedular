import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  Box,
  Chip,
  CircularProgress,
  Alert,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from '@mui/material';
import axios from 'axios';
import VideocamIcon from '@mui/icons-material/Videocam';
import RefreshIcon from '@mui/icons-material/Refresh';
import InfoIcon from '@mui/icons-material/Info';

const API_BASE_URL = import.meta.env.VITE_APP_API_URL || 'http://localhost:5180';

const Dashboard = () => {
  const [interviews, setInterviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedInterview, setSelectedInterview] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  useEffect(() => {
    fetchInterviews();
  }, []);

  const fetchInterviews = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await axios.get(`${API_BASE_URL}/api/interview`);
      setInterviews(response.data);
    } catch (error) {
      console.error('Failed to fetch interviews:', error);
      setError('Failed to load interviews. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status) => {
    switch (status?.toLowerCase()) {
      case 'scheduled': return 'primary';
      case 'completed': return 'success';
      case 'cancelled': return 'error';
      default: return 'default';
    }
  };

  const formatDateTime = (dateString) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
      });
    } catch (error) {
      return 'Invalid date';
    }
  };

  const calculateDuration = (startTime, endTime) => {
    try {
      const start = new Date(startTime);
      const end = new Date(endTime);
      const diffInMinutes = Math.round((end - start) / (1000 * 60));
      return `${diffInMinutes} min`;
    } catch (error) {
      return 'N/A';
    }
  };

  const handleViewDetails = (interview) => {
    setSelectedInterview(interview);
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setSelectedInterview(null);
  };

  if (loading) {
    return (
      <Container maxWidth="lg" sx={{ py: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Typography variant="h4" component="h1" color="primary">
            Interview Dashboard
          </Typography>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={fetchInterviews}
            disabled={loading}
          >
            Refresh
          </Button>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {interviews.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 8 }}>
            <Typography variant="h6" color="text.secondary" gutterBottom>
              No interviews scheduled yet
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Schedule your first interview to see it here
            </Typography>
          </Box>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell><strong>Job Title</strong></TableCell>
                  <TableCell><strong>Candidate</strong></TableCell>
                  <TableCell><strong>Interviewer</strong></TableCell>
                  <TableCell><strong>Date & Time</strong></TableCell>
                  <TableCell><strong>Status</strong></TableCell>
                  <TableCell><strong>Actions</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {interviews.map((interview) => (
                  <TableRow key={interview.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight="500">
                        {interview.jobTitle}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Box>
                        <Typography variant="body2" fontWeight="500">
                          {interview.candidateName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {interview.candidateEmail}
                        </Typography>
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Box>
                        <Typography variant="body2" fontWeight="500">
                          {interview.interviewerName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {interview.interviewerEmail}
                        </Typography>
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">
                        {formatDateTime(interview.startTime)}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Duration: {calculateDuration(interview.startTime, interview.endTime)}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip 
                        label={interview.status} 
                        color={getStatusColor(interview.status)}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>
                      <Box sx={{ display: 'flex', gap: 1 }}>
                        {interview.googleMeetLink && (
                          <Button
                            href={interview.googleMeetLink}
                            target="_blank"
                            variant="contained"
                            size="small"
                            startIcon={<VideocamIcon />}
                            sx={{ minWidth: 100 }}
                          >
                            Join
                          </Button>
                        )}
                        <IconButton
                          onClick={() => handleViewDetails(interview)}
                          size="small"
                          color="primary"
                        >
                          <InfoIcon />
                        </IconButton>
                      </Box>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>

      {/* Interview Details Dialog */}
      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          Interview Details
        </DialogTitle>
        <DialogContent>
          {selectedInterview && (
            <Box sx={{ pt: 1 }}>
              <Typography variant="h6" gutterBottom color="primary">
                {selectedInterview.jobTitle}
              </Typography>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Candidate</Typography>
                <Typography variant="body1">
                  {selectedInterview.candidateName} ({selectedInterview.candidateEmail})
                </Typography>
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Interviewer</Typography>
                <Typography variant="body1">
                  {selectedInterview.interviewerName} ({selectedInterview.interviewerEmail})
                </Typography>
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Schedule</Typography>
                <Typography variant="body1">
                  {formatDateTime(selectedInterview.startTime)} - {formatDateTime(selectedInterview.endTime)}
                </Typography>
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Status</Typography>
                <Chip 
                  label={selectedInterview.status} 
                  color={getStatusColor(selectedInterview.status)}
                  size="small"
                />
              </Box>
              {selectedInterview.googleMeetLink && (
                <Box sx={{ mb: 2 }}>
                  <Typography variant="body2" color="text.secondary">Meeting Link</Typography>
                  <Button
                    href={selectedInterview.googleMeetLink}
                    target="_blank"
                    variant="outlined"
                    startIcon={<VideocamIcon />}
                    sx={{ mt: 1 }}
                  >
                    Join Google Meet
                  </Button>
                </Box>
              )}
              <Box>
                <Typography variant="body2" color="text.secondary">Created</Typography>
                <Typography variant="body1">
                  {formatDateTime(selectedInterview.createdAt)}
                </Typography>
              </Box>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Close</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Dashboard;