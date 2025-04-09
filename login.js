
async function login(event) {
  event.preventDefault();
  const username = document.getElementById('username').value.trim();
  const password = document.getElementById('password').value;

  try {
    const response = await fetch('https://localhost:7246/api/Users/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });

    if (response.ok) {
      const data = await response.json();
      localStorage.setItem('token', data.token);
      window.location.href = 'tasks.html';
    } else {
      alert('Invalid credentials');
    }
  } catch (err) {
    console.error('Login error:', err);
    alert('Something went wrong. Please try again.');
  }
}

