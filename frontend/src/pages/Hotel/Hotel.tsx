import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Card, Alert, Image } from 'react-bootstrap';
import Navbar from '../../components/Navbar';
import CreateHotelModal from './CreateHotelModal';
import EditHotelModal from './EditHotelModal';
import CreateBooking from '../Booking/CreateBooking';
import { useNavigate } from 'react-router-dom';

interface IHotel {
  id: number;
  name: string;
  location: string;
  description: string;
  nightPrice: number;
  pictureList: { fileName: string }[];
}

const Hotel: React.FC = () => {
  const [hotels, setHotels] = useState<IHotel[]>([]);
  const [error, setError] = useState<string>('');
  const [isAdmin, setIsAdmin] = useState<boolean>(false);
  const navigate = useNavigate();

  const fetchHotels = async () => {
    try {
      const response = await fetch("http://localhost:8081/hotel", {
        headers: {
          "Authorization": `Bearer ${localStorage.getItem('token')}`
        },
      });

      if (!response.ok) {
        throw new Error("Échec lors de la récupération des hôtels");
      }

      const data = (await response.json()).data;
      setHotels(data);
    } catch (error: any) {
      setError(error.message);
    }
  };

  useEffect(() => {
    const role = localStorage.getItem("role");
    if (role === "admin") {
      setIsAdmin(true);
    }

    fetchHotels();
  }, []);

  const deleteHotel = async(hotelId: number) => {
    try {
      const response = await fetch(`http://localhost:8081/hotel/${hotelId}`, {
        method: 'DELETE',
        headers: {
          "Authorization": `Bearer ${localStorage.getItem('token')}`
        },
      });

      if (!response.ok) {
        throw new Error("Impossible de supprimer l'hôtel");
      }
      setHotels((prevHotels) => prevHotels.filter((hotel) => hotel.id !== hotelId));
    } catch (error: any) {
      setError(error.message);
    }
  };

  const handleHotelsUpdate = () => {
    fetchHotels();
  };

  return (
    <div>
      <Navbar />
      <Container className="my-5">
        {error && <Alert variant="danger">{error}</Alert>}

        {isAdmin && (
          <CreateHotelModal refreshHotels={handleHotelsUpdate} />
        )}
        {hotels.length === 0 ? (
          <Alert variant="info">Aucun hôtel disponible.</Alert>
        ) : (
          hotels.map((hotel) => (
            <Row key={hotel.id} className="mb-5">
              <Col md={12}>
                <Card>
                  <Card.Body>
                    <Card.Title>{hotel.name}</Card.Title>
                    <Card.Text>
                      <strong>Emplacement:</strong> {hotel.location}
                      <br />
                      <strong>Description:</strong> {hotel.description}
                      <br />
                      <strong>Prix par nuit:</strong> {hotel.nightPrice}€
                    </Card.Text>

                    <Row className="d-flex flex-nowrap">
                      {hotel.pictureList.map((picture, index) => (
                        <Col key={index} className="mb-2 me-2">
                          <Image
                            src={`http://localhost:8081/hotel/image/${picture.fileName}`}
                            alt={`Image de ${hotel.name}`}
                            width={150}
                            height={150}
                            thumbnail
                          />
                        </Col>
                      ))}
                    </Row>
                      <CreateBooking hotel={hotel} onBookingSuccess={() => navigate("/booking")}/>
                    {isAdmin && (
                      <div className="mt-3">
                        <EditHotelModal hotel={hotel} refreshHotels={handleHotelsUpdate} />
                        <Button variant="danger" onClick={() => deleteHotel(hotel.id)}>
                          Supprimer
                        </Button>
                      </div>
                    )}
                  </Card.Body>
                </Card>
              </Col>
            </Row>
          ))
        )}
      </Container>
    </div>
  );
};

export default Hotel;
