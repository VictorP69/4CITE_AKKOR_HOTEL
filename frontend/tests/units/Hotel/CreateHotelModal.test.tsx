import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import CreateHotelModal from '../../../src/pages/Hotel/CreateHotelModal';
import { describe, it, expect, vi } from 'vitest';

const mockRefreshHotels = vi.fn();

describe('CreateHotelModal', () => {
  it('should display an error when the form is invalid', async () => {
    render(<CreateHotelModal refreshHotels={mockRefreshHotels} />);

    fireEvent.click(screen.getByRole('button', { name: /Créer un hôtel/i }));

    expect(screen.getByText('Créer un Hôtel')).toBeInTheDocument();

    fireEvent.click(screen.getByRole('button', { name: /Créer l'hôtel/i }));

    await waitFor(() => {
      expect(screen.getByText('Veuillez remplir tous les champs')).toBeInTheDocument();
    });
  });

  it('should add the hotel to the DOM after successful submission', async () => {
    render(<CreateHotelModal refreshHotels={mockRefreshHotels} />);

    fireEvent.click(screen.getByRole('button', { name: /Créer un hôtel/i }));

    expect(screen.getByText('Créer un Hôtel')).toBeInTheDocument();

    fireEvent.change(screen.getByRole('textbox', { name: /Nom de l'hôtel/i }), { target: { value: 'Hôtel Test' } });
    fireEvent.change(screen.getByRole('textbox', { name: /Emplacement/i }), { target: { value: 'Paris' } });
    fireEvent.change(screen.getByRole('textbox', { name: /Description/i }), { target: { value: 'Un bel hôtel à Paris' } });

    fireEvent.click(screen.getByRole('button', { name: /Créer l'hôtel/i }));

    await waitFor(() => {
        expect(screen.queryByText('Erreur lors de la création de l\'hôtel, veuillez remplir tous les champs')).not.toBeInTheDocument();
      });
  });

});
