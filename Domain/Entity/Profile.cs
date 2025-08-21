namespace Domain.Entity
{
    public class Profile
    {
        public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
