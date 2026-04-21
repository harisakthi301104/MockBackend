namespace MockHackothonBackend.DTOs.Need
{
    public class CreateNeedDto
    {
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int RequiredVolunteers { get; set; }
    }
}
