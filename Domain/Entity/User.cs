namespace Domain.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RefreshToken {  get; set; }


        public Profile Profile { get; set; }
        public List<UserRole> UserRole { get; set; } = new();


    }
}
