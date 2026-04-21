namespace MockHackothonBackend.Models
{
    public class Commitment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int NeedId { get; set; }
        public Need Need { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
