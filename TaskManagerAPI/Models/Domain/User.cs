namespace TaskManagerAPI.Models.Domain  // Namespace for the domain models related to the Task Manager API.
{
    public class User  // Define the User class, which represents a user in the task management system.
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // Unique identifier for each user. The default value is generated as a new GUID.

        public string Username { get; set; }  // The username chosen by the user for login 

        public string FullName { get; set; }  // The full name of the user

        public string PasswordHash { get; set; }  // The hashed password of the user. It is stored securely to prevent storing raw passwords.
    }
}
