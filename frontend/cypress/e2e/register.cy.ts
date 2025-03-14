describe('Register Page', () => {  
    it('should show an error message with invalid credentials (empty fields)', () => {
      cy.visit('http://localhost:8000/register');
  
      cy.get('form').submit();
  
      cy.get('div').should('contain', 'Veuillez remplir tous les champs.');
    });
});