import React from 'react';
import { Container, Typography, Paper, Box } from '@mui/material';
import AnalysisPreferenceToggle from '../components/AnalysisPreferenceToggle';

const Settings: React.FC = () => {
    return (
        <Container maxWidth="md" sx={{ mt: 4 }}>
            <Typography variant="h4" component="h1" gutterBottom>
                Settings
            </Typography>
            
            <Paper sx={{ p: 3, mb: 3 }}>
                <Typography variant="h6" gutterBottom>
                    Analysis Settings
                </Typography>
                <AnalysisPreferenceToggle />
            </Paper>
            
            {/* Add other settings sections here */}
        </Container>
    );
};

export default Settings; 