

namespace MockHackothonBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }

        public ICollection<Commitment> Commitments { get; set; } = new List<Commitment>();
        public ICollection<Need> CreatedNeeds { get; set; } = new List<Need>();
    }
}
