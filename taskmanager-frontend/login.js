// Set the base URL for API calls — only using localhost since Railway is no longer in use
const API_BASE_URL = 'https://localhost:7246'; // Your ASP.NET Core backend running locally

// This function handles the login form submission
async function login(event) {
  event.preventDefault(); // Prevent the form from submitting in the traditional way (which would reload the page)

  // Retrieve the entered username and password from the input fields
  const username = document.getElementById('username').value.trim();
  const password = document.getElementById('password').value;

  try {
    // Make a POST request to the API's login endpoint with the username and password
    const response = await fetch(`${API_BASE_URL}/api/Users/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' }, // Indicate the request body is in JSON format
      body: JSON.stringify({ username, password }) // Convert credentials to JSON and send in request body
    });

    // Check if login was successful
    if (response.ok) {
      const data = await response.json(); // Parse the JSON response from the API
      localStorage.setItem('token', data.token); // Save the JWT token in localStorage for authenticated requests
      window.location.href = 'tasks.html'; // Redirect to the tasks page after successful login
    } else {
      // Show an error if the login failed due to incorrect credentials
      alert('Invalid credentials');
    }
  } catch (err) {
    // Log any unexpected error (like server down, CORS issues, etc.)
    console.error('Login error:', err);
    alert('Something went wrong. Please try again.');
  }
}
