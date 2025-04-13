// Set the base URL for API calls — only using localhost since Railway is no longer in use
const API_BASE_URL = 'https://localhost:7246'; // Your ASP.NET Core backend running locally

// API endpoint for task-related operations
const apiUrl = `${API_BASE_URL}/api/Tasks`;

// Retrieve the JWT token from local storage
const token = localStorage.getItem('token');

// Variable to track the task being edited (if any)
let editingTaskId = null;

// Check if the user is authenticated by verifying the token in local storage
if (!token) {
  alert('Please log in');
  window.location.href = 'index.html'; // Redirect to login page if token is missing
}

// Fetch tasks from the backend API, supporting search and filters
async function fetchTasks() {
  const title = document.getElementById('searchTitle').value.trim(); // Get title search input
  const category = document.getElementById('filterCategory').value; // Get category filter
  const status = document.getElementById('filterStatus').value; // Get status filter (Completed/Incomplete)

  let query = []; // Prepare query string based on filters
  if (title) query.push(`search=${encodeURIComponent(title)}`); // Add title search filter
  if (category) query.push(`category=${encodeURIComponent(category)}`); // Add category filter
  if (status) query.push(`isCompleted=${status}`); // Add status filter
  const queryString = query.length > 0 ? `?${query.join('&')}` : ''; // Combine query parameters

  // Fetch tasks from the API with filters applied
  const response = await fetch(`${apiUrl}${queryString}`, {
    headers: { Authorization: `Bearer ${token}` } // Pass JWT token in the Authorization header
  });

  if (!response.ok) {
    alert('Failed to load tasks'); // Handle error if fetching fails
    return;
  }

  const tasks = await response.json(); // Parse the tasks response from the backend
  const list = document.getElementById('taskList');
  list.innerHTML = ''; // Clear the task list before rendering new tasks

  // Render each task as a list item
  tasks.forEach(task => {
    const div = document.createElement('div');
    div.className = 'task-item';
    div.innerHTML = `
      <strong>${task.title}</strong><br/>
      ${task.description}<br/>
      <small>${task.category} - ${task.isCompleted ? '✅ Completed' : '❌ Incomplete'}</small><br/>
      <small>Date: ${task.dateOfCompletion ? new Date(task.dateOfCompletion).toLocaleDateString() : 'N/A'}</small><br/>
      <button class="btn btn-sm btn-danger mt-2" onclick="deleteTask('${task.id}')">Delete</button>
    `;
    div.addEventListener('click', () => fillForm(task)); // Fill the form with task data for editing
    list.appendChild(div); // Append task to the task list
  });
}

// Populate the task form with task data for editing
function fillForm(task) {
  document.getElementById('title').value = task.title;
  document.getElementById('description').value = task.description;
  document.getElementById('category').value = task.category;
  document.getElementById('isCompleted').checked = task.isCompleted;
  document.getElementById('dateOfCompletion').value = task.dateOfCompletion?.split('T')[0] || ''; // Format date
  editingTaskId = task.id; // Set the task ID to indicate we're editing
  document.getElementById('submitButton').innerText = 'Update Task'; // Change button text to 'Update Task'
}

// Add a new task or update an existing task
async function addOrUpdateTask() {
  const task = {
    title: document.getElementById('title').value,
    description: document.getElementById('description').value,
    category: document.getElementById('category').value,
    isCompleted: document.getElementById('isCompleted').checked,
    dateOfCompletion: document.getElementById('dateOfCompletion').value
      ? new Date(document.getElementById('dateOfCompletion').value).toISOString() // Format date for API
      : null
  };

  // Determine URL and HTTP method based on whether we are adding or updating a task
  const url = editingTaskId ? `${apiUrl}/${editingTaskId}` : apiUrl;
  const method = editingTaskId ? 'PUT' : 'POST';

  const response = await fetch(url, {
    method,
    headers: {
      'Content-Type': 'application/json', // Specify JSON content type
      Authorization: `Bearer ${token}` // Pass JWT token in Authorization header
    },
    body: JSON.stringify(task) // Send task data in the request body
  });

  if (response.ok) {
    resetForm(); // Reset form after successful task creation/update
    fetchTasks(); // Refresh the task list
  } else {
    alert('Failed to save task'); // Handle error if task save fails
  }
}

// Reset the form fields after task creation/update
function resetForm() {
  document.getElementById('title').value = '';
  document.getElementById('description').value = '';
  document.getElementById('category').value = 'Personal'; // Reset to default category
  document.getElementById('isCompleted').checked = false;
  document.getElementById('dateOfCompletion').value = '';
  editingTaskId = null; // Reset editing task ID
  document.getElementById('submitButton').innerText = 'Add Task'; // Reset button text
}

// Delete a task by ID
async function deleteTask(id) {
  const response = await fetch(`${apiUrl}/${id}`, {
    method: 'DELETE', // Use DELETE method to remove task
    headers: { Authorization: `Bearer ${token}` } // Pass JWT token in Authorization header
  });

  if (response.ok) {
    fetchTasks(); // Refresh the task list after deletion
    if (editingTaskId === id) resetForm(); // Reset form if deleting the task being edited
  } else {
    alert('Delete failed'); // Handle error if delete fails
  }
}

// Logout function to clear the token and redirect to the login page
function logout() {
  localStorage.removeItem('token'); // Remove JWT token from local storage
  window.location.href = 'index.html'; // Redirect to login page
}

// Initialize task fetching when the page loads
fetchTasks();
