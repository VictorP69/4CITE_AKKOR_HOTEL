import React, { useState } from "react";
import { Navbar, Nav, Button, Modal, Form, Dropdown } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";

const MyNavbar: React.FC = () => {
    const [showModal, setShowModal] = useState(false);
    const [username, setUsername] = useState<string>(localStorage.getItem('username') || '');
    const navigate = useNavigate();

    const handleModalClose = () => setShowModal(false);
    const handleModalShow = () => setShowModal(true);

    const handleProfileUpdate = async (e: React.FormEvent) => {
        console.log("unsername: " + username)
        e.preventDefault();
        try {
            const promise = await fetch(`http://localhost:8081/user/${localStorage.getItem('currentUserId')}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    "Authorization": `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify({
                    pseudo: username
                })
            });

            const response = await promise.json();

            if (!response.success) {
                throw new Error('Erreur lors de la mise à jour du profil');
            }
            console.log(response)
            localStorage.setItem('username', username);
            handleModalClose();
        } catch (error) {
            console.error('Erreur de mise à jour du profil:', error);
        }
    };

    const handleLogout = () => {
    localStorage.removeItem('username');
    localStorage.removeItem('currentUserId');
    localStorage.removeItem('role');
    localStorage.removeItem('token');
    navigate('/login');
    };

    return (
        <>
        <Navbar bg="primary" expand="lg" variant="dark">
            <Navbar.Brand as={Link} to="/" className="d-flex align-items-center" style={{ marginLeft: "10px" }}>
            <i className="bi bi-house-door-fill" style={{ fontSize: "30px", marginRight: '10px' }}></i>
            Akkor Hotel
            </Navbar.Brand>
            <Navbar.Collapse id="basic-navbar-nav" style={{ right: 0, left: 'auto' }}>
            <Nav className="ms-auto d-flex align-items-center">
                <Nav.Link as={Link} to="/hotel">Hôtels</Nav.Link>
                <Nav.Link as={Link} to="/booking">Réservations</Nav.Link>

                <Nav.Item>
                    <Dropdown align="end">
                        <Dropdown.Toggle variant="outline-light" id="dropdown-profile" className="d-flex align-items-center">
                            <span>{localStorage.getItem('username')}</span>
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            <Dropdown.Item onClick={handleModalShow}>Mettre à jour le profil</Dropdown.Item>
                            <Dropdown.Item onClick={handleLogout}>Déconnexion</Dropdown.Item>
                        </Dropdown.Menu>
                    </Dropdown>
                </Nav.Item>
            </Nav>
            </Navbar.Collapse>
        </Navbar>

        <Modal show={showModal} onHide={handleModalClose}>
            <Modal.Header closeButton>
            <Modal.Title>Mettre à jour le profil</Modal.Title>
            </Modal.Header>
            <Modal.Body>
            <Form onSubmit={handleProfileUpdate}>
                <Form.Group className="mb-3" controlId="formUsername">
                <Form.Label>Nom d'utilisateur</Form.Label>
                <Form.Control
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                </Form.Group>
                <Button variant="primary" type="submit">
                Mettre à jour
                </Button>
            </Form>
            </Modal.Body>
        </Modal>
        </>
    );
};

export default MyNavbar;
