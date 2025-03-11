import React from "react";

interface FormProps {
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  children: React.ReactNode;
  dataTestId: string
}

const Form: React.FC<FormProps> = ({ onSubmit, children, dataTestId }) => {
  return (
    <form onSubmit={onSubmit} data-testid={dataTestId}>
      {children}
    </form>
  );
};

export default Form;