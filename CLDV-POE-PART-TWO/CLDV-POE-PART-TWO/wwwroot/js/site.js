// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//function showMessage() {
//    document.getElementById('submit-message').style.display = 'block';

//}


function validateForm(event) {
    event.preventDefault();

   
    const name = document.getElementById('name').value.trim();
    const email = document.getElementById('email').value.trim();
    const message = document.getElementById('message').value.trim();

  
    if (name === '' || email === '' || message === '') {
        alert('Please fill in all fields.');
        return false;
    }

    showMessage();
    return true;
}


function showMessage() {
    document.getElementById('submit-message').style.display = 'block';
    setTimeout(function () {
  
       
        document.getElementById('contact-form').reset();
        document.getElementById('submit-message').style.display = 'none';
    }, 2000); 
}