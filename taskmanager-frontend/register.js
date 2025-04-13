// Set the base URL for API calls — only using localhost since Railway is no longer in use
const API_BASE_URL = 'https://localhost:7246'; // Your ASP.NET Core backend running locally

// Function to handle the registration process when the form is submitted
async function register(event) {
  event.preventDefault(); // Prevent the form from refreshing the page on submit

  // Collect the values entered by the user in the registration form
  const fullName = document.getElementById('fullname').value.trim(); // Full name input
  const username = document.getElementById('username').value.trim(); // Username input
  const password = document.getElementById('password').value; // Password input

  try {
    // Send a POST request to the API to register the user
    const response = await fetch(`${API_BASE_URL}/api/Users/register`, {
      method: 'POST', // HTTP method for sending data
      headers: { 'Content-Type': 'application/json' }, // Indicating the body of the request is JSON
      body: JSON.stringify({ fullName, username, password }) // Convert user data into a JSON string for the request
    });

    // Check if the response status is OK (200-299 range)
    if (response.ok) {
      alert('Registration successful! Please login.'); // Show success message
      window.location.href = 'index.html'; // Redirect to the login page after successful registration
    } else {
      // If the response is not OK, display an error message
      const error = await response.text(); // Read the error message from the response
      alert('Registration failed: ' + error); // Display the error message
    }
  } catch (err) {
    // Catch any errors that occur during the request and display a generic error message
    console.error('Registration error:', err); // Log the error to the console for debugging
    alert('Something went wrong. Try again.'); // Inform the user that something went wrong
  }
}
