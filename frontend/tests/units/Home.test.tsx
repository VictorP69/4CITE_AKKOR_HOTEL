import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import { BrowserRouter as Router, useNavigate } from 'react-router-dom';
import Home from '../../src/pages/Home.tsx';

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
    ...actual,
    useNavigate: vi.fn(),
    };
});

describe('Home', () => {
    it('should render the cards with correct content', () => {
        render(
            <Router>
                <Home />
            </Router>
        );

        expect(screen.getByText('Réservations récentes')).toBeInTheDocument();
        expect(screen.getByText('Consultez vos réservations récentes et gérez-les facilement.')).toBeInTheDocument();
        expect(screen.getByText('Voir mes réservations')).toBeInTheDocument();

        expect(screen.getByText('Hôtels disponibles')).toBeInTheDocument();
        expect(screen.getByText('Explorez les hôtels autour du monde et trouvez celui qui vous correspond.')).toBeInTheDocument();
        expect(screen.getByText('Voir les hôtels')).toBeInTheDocument();
    });

    it('should navigate to /reservation when "Voir mes réservations" is clicked', () => {
        const mockNavigate = vi.fn();
        vi.mocked(useNavigate).mockReturnValue(mockNavigate);

        render(
            <Router>
                <Home />
            </Router>
        );

        fireEvent.click(screen.getByText('Voir mes réservations'));
        expect(mockNavigate).toHaveBeenCalledWith('/reservation');
    });

    it('should navigate to /hotel when "Voir les hôtels" is clicked', () => {
        const mockNavigate = vi.fn();
        vi.mocked(useNavigate).mockReturnValue(mockNavigate);

        render(
            <Router>
                <Home />
            </Router>
        );

        fireEvent.click(screen.getByText('Voir les hôtels'));
        expect(mockNavigate).toHaveBeenCalledWith('/hotel');
    });
});
