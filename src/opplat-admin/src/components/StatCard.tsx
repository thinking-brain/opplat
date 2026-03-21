import React from 'react';
import { Card, CardContent, Stack, Typography } from '@mui/material';

interface StatCardProps {
  label: string;
  value: string | number;
  helper?: string;
}

export const StatCard: React.FC<StatCardProps> = ({ label, value, helper }) => (
  <Card variant="outlined" sx={{ height: '100%' }}>
    <CardContent>
      <Stack spacing={1}>
        <Typography variant="overline" color="text.secondary">
          {label}
        </Typography>
        <Typography variant="h4">{value}</Typography>
        {helper ? (
          <Typography variant="body2" color="text.secondary">
            {helper}
          </Typography>
        ) : null}
      </Stack>
    </CardContent>
  </Card>
);
