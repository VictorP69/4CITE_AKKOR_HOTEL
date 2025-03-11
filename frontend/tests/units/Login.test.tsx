import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import Login from '../../src/pages/Login.tsx';
import { useNavigate } from 'react-router-dom';
import React from 'react';

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: vi.fn(),
  };
});

describe('Login', () => {
  it('should render the login form with email and password inputs', () => {
    render(<Login />);

    expect(screen.getByText(/Email/i)).toBeInTheDocument();
    expect(screen.getByText(/Mot de passe/i)).toBeInTheDocument();
    expect(screen.getByText(/Se connecter/i)).toBeInTheDocument();
  });

  it('should show an error message when fields are empty and the form is submitted', async () => {
    render(<Login />);
    fireEvent.submit(screen.getByTestId('login-form'));
    expect(await screen.findByText('Veuillez remplir tous les champs.')).toBeInTheDocument();
  });

  it('should show an error message when login fails', async () => {
    render(<Login />);

    global.fetch = vi.fn().mockResolvedValueOnce({ ok: false });

    fireEvent.change(screen.getByLabelText(/Email/i), { target: { value: 'test@example.com' } });
    fireEvent.change(screen.getByLabelText(/Mot de passe/i), { target: { value: 'wrongpassword' } });
    fireEvent.submit(screen.getByTestId('login-form'));

    expect(await screen.findByText('Échec de la connexion. Vérifiez vos informations.')).toBeInTheDocument();
  });
});
