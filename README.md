# 📝 Task Manager - Full Stack Project

This is a full-stack **Task Manager** web application built using:

- **Backend:** ASP.NET Core Web API with JWT Authentication
- **Frontend:** HTML, CSS (Bootstrap), and JavaScript (Vanilla JS)
- **Database:** SQL Server

---

## 🔧 Features

### ✅ User Authentication (JWT-based)
- Register with full name, username, and password
- Login with username and password to receive a secure JWT token
- Secure access to task-related features using token-based authentication

### 📋 Task Management (Only after Login)
- Create new tasks with title, description, date of completion, and category
- View your own tasks (only your tasks are shown)
- Update task details
- Delete tasks
- Search tasks by title
- Filter tasks by:
  - Completion status (Complete / Incomplete)
  - Category

---

## 🖥️ Project Structure

### 🔹 Backend: `TaskManagerAPI` (ASP.NET Core 6+)
- `Controllers`: Handles API endpoints for user and task management
- `Models`: Contains C# classes for Users and Tasks
- `Data`: Handles EF Core DbContext and migrations
- `Program.cs`: Configures services (CORS, JWT auth, etc.)

### 🔹 Frontend: Plain HTML/CSS/JS
- `register.html`: User Registration page
- `login.html`: User Login page
- `tasks.html`: Main task dashboard (secured via JWT token)
- All HTTP calls are made using `fetch()` to the Web API

---

## 🚀 How to Run the Project

### ✅ Backend (ASP.NET Core API)

1. Open the solution in **Visual Studio**
2. Update the SQL Server connection string in `appsettings.json`
3. Run EF Core migrations if needed:
4. Run the API (F5 or `dotnet run`)
5. The API will be available at:  
`https://localhost:7246` (or your launch URL)

> ✅ Make sure CORS is enabled for your frontend URLs in `Program.cs`.

### ✅ Frontend (HTML + JS)

1. Open `login.html` in a browser
2. Register a new user → Login → Get redirected to `tasks.html`
3. All task operations work with JWT token stored in `localStorage`

---

## 🔐 Security

- Passwords are hashed before saving in the database
- JWT tokens are used to protect API routes
- CORS is enabled only for specific frontend URLs

---

## 🌐 Hosting Info

- Frontend can be hosted on GitHub Pages, Netlify, or Vercel
- Backend can be hosted on Render or Azure App Service

---

## 🙋‍♂️ About the Developer

This project was created by a passionate developer learning fullstack development using ASP.NET Core and modern JavaScript.  
Feel free to explore, use, and improve the project!

---

## 📬 Contact

If you have any questions, feel free to raise an issue or message me on GitHub.

