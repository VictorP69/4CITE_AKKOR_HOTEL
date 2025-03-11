import React, { useState } from "react";
import { Container, Alert } from "react-bootstrap";
import Form from "../components/Form";
import Input from "../components/Input";
import Button from "../components/Button";
import { useNavigate } from "react-router-dom";

const Login: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [username, setUsername] = useState("");
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const register = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!email || !password || !username) {
      setError("Veuillez remplir tous les champs.");
      return;
    }

    setError(null);
    try {
        const response = await fetch("http://localhost:8081/user/register", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ 
            email: email, 
            password: password,  
            pseudo: username
        }),
        });
        if (!response.ok) {
          throw new Error("Création de compte échouée. Vérifiez vos informations.");
        }
  
        const data = await response.json();
        const { token, role } = data;
  
        localStorage.setItem("token", token);
        localStorage.setItem("role", role);
  
        navigate("/login");
  
      } catch (error: any) {
        console.log(error)
        setError("Une erreur est survenue pendant la création du compte.");
      }
  };

  return (
    <Container className="d-flex justify-content-center align-items-center vh-100">
      <div className="w-100 p-4 shadow-lg bg-white rounded" style={{ maxWidth: "400px" }}>
        <h2 className="text-center mb-4">Créer un compte</h2>

        {error && <Alert variant="danger">{error}</Alert>}

        <Form onSubmit={register} dataTestId="test">
            <Input type="text" label="Nom d'utilisateur" id="register-username" value={username} onChange={(value) => setUsername(value)} />
            <Input type="email" label="Email" id="register-email" value={email} onChange={(value) => setEmail(value)} />
            <Input type="password" label="Mot de passe" id="register-password" value={password} onChange={(value) => setPassword(value)} />

            <Button type="submit" text="Créer un compte" variant="primary" onClick={(e) => register(e as unknown as React.FormEvent<HTMLFormElement>)} />
            <Button type="button" text="Se connecter" variant="link" onClick={() => navigate("/login")} />
        </Form>
      </div>
    </Container>
  );
};

export default Login;
