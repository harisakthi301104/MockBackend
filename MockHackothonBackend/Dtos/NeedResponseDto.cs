namespace MockHackothonBackend.DTOs.Need
{
    public class NeedResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public int RequiredVolunteers { get; set; }
        public int CurrentVolunteersCount { get; set; }

        public int RemainingSlots { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
