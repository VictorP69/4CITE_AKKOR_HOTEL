import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import EditHotelModal from '../../../src/pages/Hotel/EditHotelModal';

const mockRefreshHotels = vi.fn();

describe('EditHotelModal', () => {
  const hotel = {
    id: 1,
    name: 'Hotel Test',
    location: 'Paris',
    description: 'Un bel hôtel à Paris',
    nightPrice: 100,
    pictureList: [{ fileName: 'hotel1.jpg' }],
  };

  it('should render the modal with initial hotel data', () => {
    render(<EditHotelModal hotel={hotel} refreshHotels={mockRefreshHotels} />);
    fireEvent.click(screen.getByRole('button', { name: /Modifier/i }));
  
    expect(screen.getByText('Modifier l\'Hôtel')).toBeInTheDocument();
  
    expect(screen.getByDisplayValue('Hotel Test')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Paris')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Un bel hôtel à Paris')).toBeInTheDocument();
    expect(screen.getByDisplayValue('100')).toBeInTheDocument();
  });
  
  it('should allow updating the hotel information', async () => {
    global.fetch = vi.fn().mockResolvedValueOnce({ ok: true });
    render(<EditHotelModal hotel={hotel} refreshHotels={mockRefreshHotels} />);
  
    fireEvent.click(screen.getByRole('button', { name: /Modifier/i }));
  
    fireEvent.change(screen.getByLabelText(/Nom de l'hôtel/i), { target: { value: 'Hôtel Modifié' } });
    fireEvent.change(screen.getByLabelText(/Emplacement/i), { target: { value: 'Lyon' } });
    fireEvent.change(screen.getByLabelText(/Description/i), { target: { value: 'Un hôtel modifié à Lyon' } });
    fireEvent.change(screen.getByLabelText(/Prix par nuit/i), { target: { value: 120 } });
  
    fireEvent.click(screen.getByRole('button', { name: /Sauvegarder les modifications/i }));
  
    await waitFor(() => expect(mockRefreshHotels).toHaveBeenCalled());
  });
  
  it('should remove images from the list', () => {
    render(<EditHotelModal hotel={hotel} refreshHotels={mockRefreshHotels} />);
  
    fireEvent.click(screen.getByRole('button', { name: /Modifier/i }));
    expect(screen.getByAltText('hotel1.jpg')).toBeInTheDocument();
    fireEvent.click(screen.getByRole('button', { name: /X/i }));  
    expect(screen.queryByAltText('hotel1.jpg')).not.toBeInTheDocument();
  });
  
});
