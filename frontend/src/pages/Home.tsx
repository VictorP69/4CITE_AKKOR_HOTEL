import React from 'react';
import { Container, Row, Col, Button, Card } from 'react-bootstrap';
import Navbar from '../components/Navbar';
import { useNavigate } from 'react-router-dom';

const Home: React.FC = () => {
    const navigate = useNavigate();

    const goToHotels = () => navigate('/hotel');
    const goToReservations = () => navigate('/reservation');

    return (
    <div>
        <Navbar />

        <Container className='mt-5'>
            <Row>
                <Col>
                <Card className="mb-3 d-flex" style={{ height: '100%' }}>
                    <Card.Body className="d-flex flex-column">
                    <Card.Title>Réservations récentes</Card.Title>
                    <Card.Text>
                        Consultez vos réservations récentes et gérez-les facilement.
                    </Card.Text>
                    <Button onClick={goToReservations} variant="primary" className="mt-auto">
                        Voir mes réservations
                    </Button>
                    </Card.Body>
                </Card>
                </Col>
                <Col>
                <Card className="mb-3 d-flex" style={{ height: '100%' }}>
                    <Card.Body className="d-flex flex-column">
                    <Card.Title>Hôtels disponibles</Card.Title>
                    <Card.Text>
                        Explorez les hôtels autour du monde et trouvez celui qui vous correspond.
                    </Card.Text>
                    <Button onClick={goToHotels} variant="primary" className="mt-auto">
                        Voir les hôtels
                    </Button>
                    </Card.Body>
                </Card>
                </Col>
            </Row>
        </Container>
    </div>
    );
};

export default Home;
