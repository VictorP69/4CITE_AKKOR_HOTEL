import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { MemoryRouter } from 'react-router-dom';
import Hotel from '../../../src/pages/Hotel/Hotel';

describe('Hotel Page', () => {
  
  const mockHotels = [
    {
      id: 1,
      name: 'Hôtel Test 1',
      location: 'Paris',
      description: 'Un bel hôtel à Paris',
      nightPrice: 100,
      pictureList: [{ fileName: 'image1.jpg' }]
    },
    {
      id: 2,
      name: 'Hôtel Test 2',
      location: 'Lyon',
      description: 'Un hôtel à Lyon',
      nightPrice: 80,
      pictureList: [{ fileName: 'image2.jpg' }]
    }
  ];

  beforeEach(() => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ data: mockHotels })
    });
    localStorage.setItem('role', 'admin');
    localStorage.setItem('token', 'fake-token');
  });

  it('should render the hotels correctly', async () => {
    render(
      <MemoryRouter>
        <Hotel />
      </MemoryRouter>
    );

    await waitFor(() => expect(screen.getByText('Hôtel Test 1')).toBeInTheDocument());
    expect(screen.getByText('Hôtel Test 2')).toBeInTheDocument();
  });

  it('should display a message when no hotels are available', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ data: [] })
    });

    render(
      <MemoryRouter>
        <Hotel />
      </MemoryRouter>
    );

    expect(await screen.findByText('Aucun hôtel disponible.')).toBeInTheDocument();
  });

  it('should display an error if hotel fetching fails', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false
    });

    render(
      <MemoryRouter>
        <Hotel />
      </MemoryRouter>
    );

    expect(await screen.findByText('Échec lors de la récupération des hôtels')).toBeInTheDocument();
  });

  it('should display CreateHotelModal for an admin', async () => {
    render(
      <MemoryRouter>
        <Hotel />
      </MemoryRouter>
    );

    expect(screen.getByText('Créer un hôtel')).toBeInTheDocument();
  });
});
