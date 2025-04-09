async function register(event) {
    event.preventDefault(); // prevent form reload

    const fullName = document.getElementById('fullname').value.trim();
    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value;

    try {
      const response = await fetch('https://localhost:7246/api/Users/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fullName, username, password })
      });

      if (response.ok) {
        alert('Registration successful! Please login.');
        window.location.href = 'login.html';
      } else {
        const error = await response.text();
        alert('Registration failed: ' + error);
      }
    } catch (err) {
      console.error('Registration error:', err);
      alert('Something went wrong. Try again.');
    }
  }