import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { MemoryRouter, useNavigate } from 'react-router-dom';
import Booking from '../../../src/pages/Booking/Booking';

const mockBookings = [
{ id: '1', hotelId: '101', checkInDate: '2025-03-11', checkOutDate: '2025-03-14', userId: 'a123' },
{ id: '2', hotelId: '102', checkInDate: '2024-11-11', checkOutDate: '2024-11-16', userId: 'a123' },
{ id: '3', hotelId: '103', checkInDate: '2025-03-12', checkOutDate: '2025-03-15', userId: 'z546' },
{ id: '4', hotelId: '104', checkInDate: '2025-03-13', checkOutDate: '2025-03-16', userId: 'c652' },
];

vi.spyOn(global.Storage.prototype, 'getItem').mockImplementation((key) => {
    if (key === 'token') {
        return 'fake-token';
    }
    if (key === 'role') {
        return 'user';
    }
    if (key === 'currentUserId') {
        return 'a123';
    }
        return null;
});

global.fetch = vi.fn().mockImplementation(async (url) => {
    const userId = localStorage.getItem('currentUserId');
    if (url.includes(`user/${userId}`)) {
        return {
            json: () => Promise.resolve({ success: true, data: mockBookings.filter((booking) => booking.userId === userId) }),
        };
    }
    return {
        json: () => Promise.resolve({ success: true, data: mockBookings }),
    };
});

describe('Booking Page', () => {
    it('should display only user bookings (role: user)', async () => {
        render(
            <MemoryRouter>
                <Booking />
            </MemoryRouter>
        );
        await waitFor(() => {
            expect(screen.getByText('11/03/2025')).toBeInTheDocument(); // vérification des checkin date dans le dom
            expect(screen.getByText('11/11/2024')).toBeInTheDocument();
            expect(screen.queryByText('12/03/2025')).not.toBeInTheDocument();
        });
    });

    it('should display all bookings (role: admin)', async () => {
        vi.spyOn(global.Storage.prototype, 'getItem').mockImplementation((key) => {
            if (key === 'token') return 'fake-token';
            if (key === 'role') return 'admin';
            if (key === 'currentUserId') return null;
            return null;
        });
        render(
            <MemoryRouter>
                <Booking />
            </MemoryRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('11/03/2025')).toBeInTheDocument();
            expect(screen.getByText('11/11/2024')).toBeInTheDocument();
            expect(screen.getByText('12/03/2025')).toBeInTheDocument();
            expect(screen.getByText('13/03/2025')).toBeInTheDocument();
        });
    });

    // it('should redirect to login page when user not logged in and refresh', async () => {
    //     vi.spyOn(global.Storage.prototype, 'clear').mockImplementation(() => {
    //     });

    //     const { rerender } = render(
    //         <MemoryRouter initialEntries={['/booking']}>
    //             <Booking />
    //         </MemoryRouter>
    //     );
    
    //     rerender(
    //         <MemoryRouter initialEntries={['/booking']}>
    //             <Booking />
    //         </MemoryRouter>
    //     );

    //     await waitFor(() => {
    //         expect(screen.getByText('Connexion')).toBeInTheDocument();
    //     });
    // });
});