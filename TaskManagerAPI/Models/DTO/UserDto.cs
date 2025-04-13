namespace TaskManagerAPI.Models.DTO  // Namespace for Data Transfer Objects (DTOs) used in the Task Manager API.
{
    // This DTO represents the data structure for returning user information.
    public class UserDto
    {
        public string Username { get; set; }  // Represents the username of the user, a unique identifier for them in the system.

        public string FullName { get; set; }  // Represents the full name of the user, used for display purposes in the application.
    }

    // This DTO is used when creating a new user, containing the necessary information for registration.
    public class AddUserRequestDto
    {
        public string Username { get; set; }  // The username of the user that will be used for authentication. It should be unique.

        public string FullName { get; set; }  // The full name of the user, which is typically used for displaying their name in the app.

        public string Password { get; set; }  // The password provided by the user. This will be hashed before storing for security.
    }

    // This DTO represents the data needed for a user to log in to the system.
    public class LoginRequestDto
    {
        public string Username { get; set; }  // The username of the user attempting to log in. It must match the username stored in the database.

        public string Password { get; set; }  // The password entered by the user. This will be validated by comparing it to the hashed password stored in the database.
    }
}

