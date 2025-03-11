import React, { useEffect, useState } from 'react';
import { Alert, Table, Button, Container } from 'react-bootstrap';
import Navbar from '../../components/Navbar';

interface IBooking {
    id: string;
    hotelId: string;
    checkInDate: string;
    checkOutDate: string;
    userId: string;
}

const Booking: React.FC = () => {
    const [bookings, setBookings] = useState<IBooking[]>([]);
    const [error, setError] = useState<string>('');

    const fetchBookings = async () => {
        const token = localStorage.getItem('token');
        const role = localStorage.getItem('role');
        if (role === 'admin') {
            try {
                const response = await fetch('http://localhost:8081/booking', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                });
                const data = await response.json();
                console.log(data)
                if (data.success) {
                    setBookings(data.data);
                } else {
                    setError('Erreur lors de la récupération des réservations.');
                }
            } catch (error) {
                setError('Erreur réseau.');
            }
        } else if (role === 'user') {
            const userId = localStorage.getItem('currentUserId');
            try {
                const response = await fetch(`http://localhost:8081/booking/user/${userId}`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                });
                const data = await response.json();
                console.log(data)
                if (data.success) {
                    setBookings(data.data);
                } else {
                    setError('Erreur lors de la récupération des réservations.');
                }
            } catch (error) {
                setError('Erreur réseau.');
            }
        }
    };

    useEffect(() => {
        fetchBookings();
    }, []);

    return (
        <div>
            <Navbar/>
            <Container>
                <h2>Réservations</h2>

                {error && <Alert variant="danger">{error}</Alert>}

                <Table striped bordered hover>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Id de l'hôtel</th>
                            <th>Date d'arrivée</th>
                            <th>Date de départ</th>
                            <th>Utilisateur ID</th>
                        </tr>
                    </thead>
                    <tbody>
                        {bookings.length > 0 ? (
                            bookings.map((booking) => (
                                <tr key={booking.id}>
                                    <td>{booking.id}</td>
                                    <td>{booking.hotelId}</td>
                                    <td>{new Date(booking.checkInDate).toLocaleDateString()}</td>
                                    <td>{new Date(booking.checkOutDate).toLocaleDateString()}</td>
                                    <td>{booking.userId}</td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={5}>Aucune réservation disponible</td>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </Container>
        </div>
    );
};

export default Booking;
