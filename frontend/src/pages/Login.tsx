import React, { useState } from "react";
import { Container, Alert } from "react-bootstrap";
import Form from "../components/Form";
import Input from "../components/Input";
import Button from "../components/Button";
import { useNavigate } from "react-router-dom";

const Login: React.FC = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const login = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!email || !password) {
        setError("Veuillez remplir tous les champs.");
        return;
    }
    setError(null);
    try {
        const response = await fetch("http://localhost:8081/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) {
            throw new Error("Échec de la connexion. Vérifiez vos informations.");
        }

        const data = await response.json();
        const { accessToken } = data;
        localStorage.setItem("token", accessToken);

        const userPromise = await fetch(`http://localhost:8081/user/email/${email}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${accessToken}`
            }
        });        
        const user = await userPromise.json();
        localStorage.setItem("currentUserId", user.data.id);
        localStorage.setItem("username", user.data.pseudo);
        localStorage.setItem("role", user.data.role == 1 ? 'admin' : 'user' );
        navigate("/");

    } catch (error: any) {
        setError(error.message);
    }
    };

    return (
        <Container className="d-flex justify-content-center align-items-center vh-100">
            <div className="w-100 p-4 shadow-lg bg-white rounded" style={{ maxWidth: "400px" }}>
            <h2 className="text-center mb-4">Connexion</h2>

            {error && <Alert variant="danger">{error}</Alert>}

            <Form onSubmit={login} dataTestId="login-form">
                <Input id="email" type="email" label="Email" value={email} onChange={(value) => setEmail(value)} />
                <Input id="password" type="password" label="Mot de passe" value={password} onChange={(value) => setPassword(value)} />
                <Button type="submit" text="Se connecter" variant="primary" onClick={() => login}/>
                <Button type="button" text="Créer un compte" variant="link" onClick={() => navigate("/register")} />
            </Form>
            </div>
        </Container>
    );
};

export default Login;
