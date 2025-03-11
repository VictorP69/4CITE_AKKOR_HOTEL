import React from "react";
import { Button as BootstrapButton } from "react-bootstrap";

interface ButtonProps {
  text: string;
  type: "button" | "submit";
  variant: string;
  onClick: (event: React.MouseEvent<HTMLButtonElement>) => void;
}

const Button: React.FC<ButtonProps> = ({ text, type, variant, onClick }) => {
  return (
    <BootstrapButton variant={variant} type={type} className="w-100" onClick={onClick}>
      {text}
    </BootstrapButton>
  );
};

export default Button;