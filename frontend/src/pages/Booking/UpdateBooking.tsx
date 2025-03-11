import React, { useState } from 'react';
import { Modal, Button, Form, Row, Col, Image, Alert } from 'react-bootstrap';

interface IUpdateBookingForm {
    checkInDate: string;
    checkOutDate: string;
}
interface IHotel {
    id: number;
    name: string;
    location: string;
    description: string;
    nightPrice: number;
    pictureList: { fileName: string }[];
}

interface IBookingModalProps {
    hotel: IHotel;
    onBookingSuccess: () => void;
}

const CreateBooking: React.FC<IBookingModalProps> = ({ hotel, onBookingSuccess }) => {
    const [show, setShow] = useState(false);
    const [error, setError] = useState<string>('');
    const [bookingForm, setBookingForm] = useState<IUpdateBookingForm>({
        checkInDate: '',
        checkOutDate: ''
});

const handleShow = () => setShow(true);

const handleClose = () => setShow(false);

const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setBookingForm((prevState) => ({
        ...prevState,
        [name]: value,
    }));
};

const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const arrivalDate = new Date(bookingForm.checkInDate);
    const departureDate = new Date(bookingForm.checkOutDate);

    if (arrivalDate < new Date) {
        setError('La date d\'arrivée doit être supérieur à la date d\'aujourd\'hui');
        return;
    }
    if (departureDate <= arrivalDate) {
      setError('La date de départ doit être après la date d\'arrivée');
      return;
    }

    try {
        const response = await fetch('http://localhost:8081/booking', {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(bookingForm),
        });
        const data = await response.json()
        console.log(response);
        if (data.success) {
            onBookingSuccess();
            handleClose();
        } else {
            setError('Erreur lors de la réservation');
        }
    } catch (error) {
        setError('Erreur réseau');
    }
};  
const calculateNights = () => {
    const arrivalDate = new Date(bookingForm.checkInDate);
    const departureDate = new Date(bookingForm.checkOutDate);
    const timeDiff = departureDate.getTime() - arrivalDate.getTime();
    const days = timeDiff / (1000 * 3600 * 24);
    return days > 0 ? days : 0;
};

const calculateTotalPrice = () => {
    const nights = calculateNights();
    return nights * hotel.nightPrice;
};

return (
<>
    <Button variant="primary" onClick={handleShow}>
    Réserver
    </Button>

    <Modal show={show} onHide={handleClose}>
    <Modal.Header closeButton>
        <Modal.Title>Réservation de l'hôtel - {hotel.name}</Modal.Title>
    </Modal.Header>
    <Modal.Body>
        {error && <Alert variant="danger">{error}</Alert>}
        <Form onSubmit={handleSubmit}>
        <Form.Group controlId="formArrivalDate" className="mb-3">
            <Form.Label>Date d'arrivée</Form.Label>
            <Form.Control
            type="date"
            name="checkInDate"
            value={bookingForm.checkInDate}
            onChange={handleInputChange}
            required
            />
        </Form.Group>

        <Form.Group controlId="formDepartureDate" className="mb-3">
            <Form.Label>Date de départ</Form.Label>
            <Form.Control
            type="date"
            name="checkOutDate"
            value={bookingForm.checkOutDate}
            onChange={handleInputChange}
            required
            />
        </Form.Group>

        <h4>Total: {calculateTotalPrice()}€</h4>

        <Button variant="success" type="submit">
            Payer {calculateTotalPrice()}€
        </Button>
        </Form>
    </Modal.Body>
    </Modal>
</>
);
};

export default CreateBooking;
