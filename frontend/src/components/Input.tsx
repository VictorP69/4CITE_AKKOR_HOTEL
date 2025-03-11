import React from "react";
import { Form as BootstrapForm } from "react-bootstrap";

interface InputProps {
  label: string;
  type: string;
  value: string;
  onChange: (value: string) => void;
  id: string
}

const Input: React.FC<InputProps> = ({ label, type, value, onChange, id }) => {
  return (
    <BootstrapForm.Group className="mb-3">
      <BootstrapForm.Label htmlFor={id}>{label}</BootstrapForm.Label>
      <BootstrapForm.Control
        id={id}
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        required
      />
    </BootstrapForm.Group>
  );
};

export default Input;