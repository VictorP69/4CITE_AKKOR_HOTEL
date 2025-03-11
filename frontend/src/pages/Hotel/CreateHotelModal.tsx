import React, { useState } from 'react';
import { Modal, Button, Form, Row, Col, Image, Alert } from 'react-bootstrap';

interface IHotelForm {
  name: string;
  location: string;
  description: string;
  nightPrice: number;
  pictures: File[];
}

interface ICreateHotelModalProps {
  refreshHotels: () => void;
}

const CreateHotelModal: React.FC<ICreateHotelModalProps> = ({refreshHotels}) => {
  const [show, setShow] = useState(false);
  const [error, setError] = useState("")
  const [hotelForm, setHotelForm] = useState<IHotelForm>({
    name: '',
    location: '',
    description: '',
    nightPrice: 0,
    pictures: [],
  });

  const handleShow = () => setShow(true);

  const handleClose = () => setShow(false);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setHotelForm((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const filesArray = Array.from(e.target.files);
      setHotelForm((prevState) => ({
        ...prevState,
        pictures: [...prevState.pictures, ...filesArray]
      }));
    }
  };

  const handleRemoveImage = (index: number) => {
    setHotelForm((prevState) => ({
      ...prevState,
      pictures: prevState.pictures.filter((_, i) => i !== index),
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append('name', hotelForm.name);
    formData.append('location', hotelForm.location);
    formData.append('description', hotelForm.description);
    formData.append('nightPrice', hotelForm.nightPrice.toString());

    hotelForm.pictures.forEach((file) => {
      formData.append('pictures', file);
    });
    if (!hotelForm.name || !hotelForm.location || !hotelForm.description || !hotelForm.nightPrice) {
      setError("Veuillez remplir tous les champs");
    }
    try {
      const response = await fetch('http://localhost:8081/hotel', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
        },
        body: formData,
      });

      if (response.ok) {
        handleClose();
        refreshHotels();
      } else {
        setError("Erreur lors de la création de l'hôtel, vérifiez les champs");
      }
    } catch (error) {
      console.error('Error during creation of hotel');
    }
  };

  return (
    <>
      <Button variant="success" onClick={handleShow} className="mb-4">
        Créer un hôtel
      </Button>

      <Modal show={show} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>Créer un Hôtel</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formHotelName" className="mb-3">
              <Form.Label>Nom de l'hôtel</Form.Label>
              <Form.Control
                type="text"
                name="name"
                value={hotelForm.name}
                onChange={handleInputChange}
                required
              />
            </Form.Group>

            <Form.Group controlId="formHotelLocation" className="mb-3">
              <Form.Label>Emplacement</Form.Label>
              <Form.Control
                type="text"
                name="location"
                value={hotelForm.location}
                onChange={handleInputChange}
                required
              />
            </Form.Group>

            <Form.Group controlId="formHotelDescription" className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control
                type="text"
                name="description"
                value={hotelForm.description}
                onChange={handleInputChange}
                required
                as="textarea"
              />
            </Form.Group>

            <Form.Group controlId="formHotelNightPrice" className="mb-3">
              <Form.Label>Prix par nuit (€)</Form.Label>
              <Form.Control
                type="number"
                name="nightPrice"
                value={hotelForm.nightPrice}
                onChange={handleInputChange}
                required
              />
            </Form.Group>

            <Form.Group controlId="formHotelPictures" className="mb-3">
              <Form.Label>Images</Form.Label>
              <Form.Control
                type="file"
                name="pictures"
                onChange={handleFileChange}
                multiple
                required
              />
              <Row className="mt-3">
                {hotelForm.pictures.map((file, index) => (
                  <Col key={index} xs={4} className="mb-2 position-relative">
                    <Image
                      src={URL.createObjectURL(file)}
                      alt={file.name}
                      thumbnail
                      width="100%"
                      height="auto"
                    />
                    <Button
                      variant="danger"
                      className="position-absolute top-0 end-0"
                      size="sm"
                      onClick={() => handleRemoveImage(index)}
                    >
                    </Button>
                  </Col>
                ))}
              </Row>
            </Form.Group>

            <Button variant="primary" type="submit">
              Créer l'hôtel
            </Button>

            {error && <Alert variant="danger">{error}</Alert>}
          </Form>
        </Modal.Body>
      </Modal>
    </>
  );
};

export default CreateHotelModal;
