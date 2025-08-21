namespace Domain.Entity
{
    public class BlackList
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public DateTime RevokedAt { get; set; }

    }
}
