describe('Login Page', () => {
  it('should login successfully with valid credentials', () => {
    cy.visit('http://localhost:8000/login');

    cy.get('input#email').type('admin@test.com'); 
    cy.get('input#password').type('AdminPassword123!');

    cy.get('form').submit();

    cy.url().should('include', '/');
    cy.get('div').should('contain', 'Réservations récentes');
  });

  it('should show an error message with invalid credentials', () => {
    cy.visit('http://localhost:8000/login');

    cy.get('input#email').type('invalid@example.com');
    cy.get('input#password').type('wrongpassword');

    cy.get('form').submit();

    cy.get('div').should('contain', 'Échec de la connexion. Vérifiez vos informations.');
  });
});
