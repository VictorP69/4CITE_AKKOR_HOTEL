import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { MemoryRouter } from 'react-router-dom';
import CreateBooking from '../../../src/pages/Booking/CreateBooking';

describe('Modal to create booking', () => {
    const mockHotel = {
        id: 1, 
        name: "Hôtel Test", 
        location: "Paris", 
        description: "Un bel hôtel", 
        nightPrice: 100, 
        pictureList: [] 
    };

    it('should display the button "Réserver"', () => {
        render(
            <MemoryRouter>
                <CreateBooking hotel={mockHotel} onBookingSuccess={vi.fn()} />
            </MemoryRouter>
        );

        expect(screen.getByText("Réserver")).toBeInTheDocument();
    });

    it('should open the modal when clicking "Réserver"', () => {
        render(
            <MemoryRouter>
                <CreateBooking hotel={mockHotel} onBookingSuccess={vi.fn()} />
            </MemoryRouter>
        );

        fireEvent.click(screen.getByText("Réserver"));

        expect(screen.getByText("Réservation de l'hôtel - Hôtel Test")).toBeInTheDocument();
    });

    it('should display an error if departure date is before arrival date', async () => {
        render(
            <MemoryRouter>
                <CreateBooking hotel={mockHotel} onBookingSuccess={vi.fn()} />
            </MemoryRouter>
        );

        fireEvent.click(screen.getByText("Réserver"));

        const arrivalDateInput = screen.getByLabelText("Date d'arrivée");
        const departureDateInput = screen.getByLabelText("Date de départ");

        fireEvent.change(arrivalDateInput, { target: { value: "2025-03-18" } });
        fireEvent.change(departureDateInput, { target: { value: "2025-03-11" } });

        fireEvent.click(screen.getByText('Payer'));

        expect(await screen.findByText("La date de départ doit être après la date d'arrivée")).toBeInTheDocument();
    });

    it('should display an error if departure date is superior to current date', async () => {
        render(
            <MemoryRouter>
                <CreateBooking hotel={mockHotel} onBookingSuccess={vi.fn()} />
            </MemoryRouter>
        );

        fireEvent.click(screen.getByText("Réserver"));

        const arrivalDateInput = screen.getByLabelText("Date d'arrivée");
        const departureDateInput = screen.getByLabelText("Date de départ");

        fireEvent.change(arrivalDateInput, { target: { value: "2025-03-10" } });
        fireEvent.change(departureDateInput, { target: { value: "2025-12-20" } });

        fireEvent.click(screen.getByText('Payer'));

        expect(await screen.findByText("La date de départ ne peut pas être inférieur à la date d\'aujourd\'hui")).toBeInTheDocument();
    });
});