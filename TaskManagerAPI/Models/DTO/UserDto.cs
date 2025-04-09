namespace TaskManagerAPI.Models.DTO
{
    public class UserDto
    {
        public string Username { get; set; }
        public string FullName { get; set; }
    }

    public class AddUserRequestDto
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
