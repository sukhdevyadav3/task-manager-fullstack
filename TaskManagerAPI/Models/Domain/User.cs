namespace TaskManagerAPI.Models.Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
    }
}
