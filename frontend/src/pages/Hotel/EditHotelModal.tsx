import React, { useState, useEffect } from 'react';
import { Modal, Button, Form, Row, Col, Image, Alert } from 'react-bootstrap';

interface IHotelForm {
  id: number;
  name: string;
  location: string;
  description: string;
  nightPrice: number;
  pictureList: { fileName: string }[];
}

interface IEditHotelModalProps {
  hotel: IHotelForm;
  refreshHotels: () => void;
}

const EditHotelModal: React.FC<IEditHotelModalProps> = ({ hotel, refreshHotels }) => {
  const [show, setShow] = useState(false);
  const [error, setError] = useState("");
  const [name, setName] = useState(hotel.name);
  const [location, setLocation] = useState(hotel.location);
  const [description, setDescription] = useState(hotel.description);
  const [nightPrice, setNightPrice] = useState(hotel.nightPrice);
  const [pictureList, setPictureList] = useState<{ fileName: string }[]>(hotel.pictureList);
  const [files, setFiles] = useState<File[]>([]);

  useEffect(() => {
    setName(hotel.name);
    setLocation(hotel.location);
    setDescription(hotel.description);
    setNightPrice(hotel.nightPrice);
    setPictureList(hotel.pictureList);
  }, [hotel]);

  const handleShow = () => setShow(true);

  const handleClose = () => setShow(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formData = new FormData();

    if (name !== hotel.name) formData.append('name', name);
    if (location !== hotel.location) formData.append('location', location);
    if (description !== hotel.description) formData.append('description', description);
    if (nightPrice !== hotel.nightPrice) formData.append('nightPrice', nightPrice.toString());

    files.forEach(file => {
      formData.append('Pictures', file);
    });

    try {
      const response = await fetch(`http://localhost:8081/hotel/${hotel.id}`, {
        method: 'PATCH',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
        },
        body: formData
      });

      if (response.ok) {
        handleClose();
        refreshHotels();
      } else {
        setError("Erreur lors de la mise à jour de l'hôtel.");
      }
    } catch (error) {
      console.error('Error during update of hotel', error);
      setError("Une erreur s'est produite.");
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const filesArray = Array.from(e.target.files);
      setFiles(filesArray);
    }
  };

  const handleRemoveImage = (index: number) => {
    setPictureList(pictureList.filter((_, i) => i !== index));
  };

  return (
    <>
      <Button variant="warning" onClick={handleShow}>
        Modifier
      </Button>

      <Modal show={show} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>Modifier l'Hôtel</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formHotelName" className="mb-3">
              <Form.Label>Nom de l'hôtel</Form.Label>
              <Form.Control
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </Form.Group>

            <Form.Group controlId="formHotelLocation" className="mb-3">
              <Form.Label>Emplacement</Form.Label>
              <Form.Control
                type="text"
                value={location}
                onChange={(e) => setLocation(e.target.value)}
              />
            </Form.Group>

            <Form.Group controlId="formHotelDescription" className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </Form.Group>

            <Form.Group controlId="formHotelNightPrice" className="mb-3">
              <Form.Label>Prix par nuit (€)</Form.Label>
              <Form.Control
                type="number"
                value={nightPrice}
                onChange={(e) => setNightPrice(Number(e.target.value))}
              />
            </Form.Group>

            <Form.Group controlId="formHotelPictures" className="mb-3">
              <Form.Label>Images</Form.Label>
              <Form.Control
                type="file"
                onChange={handleFileChange}
                multiple
              />
              <Row className="mt-3">
                {pictureList.map((file, index) => (
                  <Col key={index} xs={4} className="mb-2 position-relative">
                    <Image
                      src={`http://localhost:8081/hotel/image/${file.fileName}`}
                      alt={file.fileName}
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
                      X
                    </Button>
                  </Col>
                ))}
              </Row>
            </Form.Group>

            <Button variant="primary" type="submit">
              Sauvegarder les modifications
            </Button>

            {error && <Alert variant="danger">{error}</Alert>}
          </Form>
        </Modal.Body>
      </Modal>
    </>
  );
};

export default EditHotelModal;