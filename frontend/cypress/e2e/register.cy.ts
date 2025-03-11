describe('Register Page', () => {
    it('should register successfully with valid credentials', () => {
      cy.visit('http://localhost:8000/register');
  
      cy.get('input#register-username').type('newuser' + Math.floor((Math.random() * 100) + 1));
      cy.get('input#register-email').type('newuser@test.com');
      cy.get('input#register-password').type('NewPassword123!');
  
      cy.get('form').submit();
      cy.wait(3000);
      cy.url().should('include', '/login');
    });
  
    it('should show an error message with invalid credentials (empty fields)', () => {
      cy.visit('http://localhost:8000/register');
  
      cy.get('form').submit();
  
      cy.get('div').should('contain', 'Veuillez remplir tous les champs.');
    });
});