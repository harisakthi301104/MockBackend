using MockHackothonBackend.Models;

namespace MockHackothonBackend.Model
{
    public class Need
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public int RequiredVolunteers { get; set; }
        public int CurrentVolunteersCount { get; set; }

        public NeedStatus Status { get; set; }

        public int CreatedBy { get; set; }
        public User Organization { get; set; } = null!;

        public ICollection<Commitment> Commitments { get; set; } = new List<Commitment>();
    }
    public enum NeedStatus
    {
        Open = 1,
        Closed = 2
    }
}
