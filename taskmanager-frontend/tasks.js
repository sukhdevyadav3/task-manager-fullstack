const API_BASE_URL = window.location.hostname === 'localhost'
  ? 'https://localhost:7246'
  : 'https://task-manager-fullstack-production.up.railway.app';

const apiUrl = `${API_BASE_URL}/api/Tasks`;
const token = localStorage.getItem('token');
let editingTaskId = null;

if (!token) {
  alert('Please log in');
  window.location.href = 'index.html';
}

async function fetchTasks() {
  const title = document.getElementById('searchTitle').value.trim();
  const category = document.getElementById('filterCategory').value;
  const status = document.getElementById('filterStatus').value;

  let query = [];
  if (title) query.push(`search=${encodeURIComponent(title)}`);
  if (category) query.push(`category=${encodeURIComponent(category)}`);
  if (status) query.push(`isCompleted=${status}`);
  const queryString = query.length > 0 ? `?${query.join('&')}` : '';

  const response = await fetch(`${apiUrl}${queryString}`, {
    headers: { Authorization: `Bearer ${token}` }
  });

  if (!response.ok) {
    alert('Failed to load tasks');
    return;
  }

  const tasks = await response.json();
  const list = document.getElementById('taskList');
  list.innerHTML = '';

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
    div.addEventListener('click', () => fillForm(task));
    list.appendChild(div);
  });
}

function fillForm(task) {
  document.getElementById('title').value = task.title;
  document.getElementById('description').value = task.description;
  document.getElementById('category').value = task.category;
  document.getElementById('isCompleted').checked = task.isCompleted;
  document.getElementById('dateOfCompletion').value = task.dateOfCompletion?.split('T')[0] || '';
  editingTaskId = task.id;
  document.getElementById('submitButton').innerText = 'Update Task';
}

async function addOrUpdateTask() {
  const task = {
    title: document.getElementById('title').value,
    description: document.getElementById('description').value,
    category: document.getElementById('category').value,
    isCompleted: document.getElementById('isCompleted').checked,
    dateOfCompletion: document.getElementById('dateOfCompletion').value
      ? new Date(document.getElementById('dateOfCompletion').value).toISOString()
      : null
  };

  const url = editingTaskId ? `${apiUrl}/${editingTaskId}` : apiUrl;
  const method = editingTaskId ? 'PUT' : 'POST';

  const response = await fetch(url, {
    method,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(task)
  });

  if (response.ok) {
    resetForm();
    fetchTasks();
  } else {
    alert('Failed to save task');
  }
}

function resetForm() {
  document.getElementById('title').value = '';
  document.getElementById('description').value = '';
  document.getElementById('category').value = 'Personal';
  document.getElementById('isCompleted').checked = false;
  document.getElementById('dateOfCompletion').value = '';
  editingTaskId = null;
  document.getElementById('submitButton').innerText = 'Add Task';
}

async function deleteTask(id) {
  const response = await fetch(`${apiUrl}/${id}`, {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` }
  });

  if (response.ok) {
    fetchTasks();
    if (editingTaskId === id) resetForm();
  } else {
    alert('Delete failed');
  }
}

function logout() {
  localStorage.removeItem('token');
  window.location.href = 'index.html';
}

fetchTasks();
