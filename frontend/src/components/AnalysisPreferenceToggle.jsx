import React, { useState, useEffect, useCallback } from 'react';
import { Switch, FormControlLabel, Typography, Box, Alert } from '@mui/material';
import { getAnalysisPreference, setAnalysisPreference } from '../Services/api';

const AnalysisPreferenceToggle = () => {
    const [useOpenAI, setUseOpenAI] = useState(true);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);

    const fetchPreference = useCallback(async () => {
        try {
            const preference = await getAnalysisPreference();
            setUseOpenAI(preference);
            setError(null);
        } catch (err) {
            setError('Failed to fetch analysis preference');
            console.error('Error fetching preference:', err);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchPreference();
    }, [fetchPreference]);

    const handleToggle = async (event) => {
        const newValue = event.target.checked;
        setLoading(true);
        try {
            await setAnalysisPreference(newValue);
            setUseOpenAI(newValue);
            setError(null);
        } catch (err) {
            const errorMessage = err.response?.data?.title || err.message || 'Failed to update analysis preference';
            setError(errorMessage);
            console.error('Error updating preference:', err);
            // Revert the toggle if the update failed
            setUseOpenAI(!newValue);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return <Typography>Loading...</Typography>;
    }

    return (
        <Box sx={{ p: 2 }}>
            <FormControlLabel
                control={
                    <Switch
                        checked={useOpenAI}
                        onChange={handleToggle}
                        color="primary"
                        disabled={loading}
                    />
                }
                label={
                    <Typography>
                        Use {useOpenAI ? 'OpenAI' : 'Hardcoded'} Analysis
                    </Typography>
                }
            />
            {error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    {error}
                </Alert>
            )}
            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                {useOpenAI
                    ? 'Using OpenAI for sophisticated text analysis. This requires API credits.'
                    : 'Using hardcoded analysis system. No API credits required.'}
            </Typography>
        </Box>
    );
};

export default AnalysisPreferenceToggle; 