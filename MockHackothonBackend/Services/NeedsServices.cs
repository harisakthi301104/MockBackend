using MockHackothonBackend.Data;
using MockHackothonBackend.Model;
using RapidRelief.DTOs.Need;

namespace MockHackothonBackend.Services
{
    public class NeedsServices
    {

        private readonly AppDbContext _context;

        public NeedService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NeedResponseDto?> CreateNeed(CreateNeedDto dto, int organizationId)
        {
            // Validate
            if (dto.RequiredVolunteers <= 0)
                return null;

            var need = new Need
            {
                Title = dto.Title,
                Location = dto.Location,
                RequiredVolunteers = dto.RequiredVolunteers,
                CurrentVolunteersCount = 0,
                Status = NeedStatus.Open,
                CreatedBy = organizationId
            };

            _context.Needs.Add(need);
            await _context.SaveChangesAsync();

            return MapToDto(need);
        }

        public async Task<List<NeedResponseDto>> GetOpenNeeds()
        {
            var needs = await _context.Needs
                .Where(n => n.Status == NeedStatus.Open)
                .ToListAsync();

            return needs.Select(MapToDto).ToList();
        }

        public async Task<List<NeedResponseDto>> GetNeedsByOrganization(int organizationId)
        {
            var needs = await _context.Needs
                .Where(n => n.CreatedBy == organizationId)
                .ToListAsync();

            return needs.Select(MapToDto).ToList();
        }

        public async Task<List<object>> GetVolunteersForNeed(int needId, int organizationId)
        {
            var need = await _context.Needs
                .Include(n => n.Commitments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(n => n.Id == needId && n.CreatedBy == organizationId);

            if (need == null)
                return new List<object>();

            return need.Commitments.Select(c => new
            {
                id = c.User.Id,
                name = c.User.Name,
                email = c.User.Email,
                committedAt = c.CreatedAt
            }).Cast<object>().ToList();
        }

        private NeedResponseDto MapToDto(Need need)
        {
            return new NeedResponseDto
            {
                Id = need.Id,
                Title = need.Title,
                Location = need.Location,
                RequiredVolunteers = need.RequiredVolunteers,
                CurrentVolunteersCount = need.CurrentVolunteersCount,
                RemainingSlots = need.RequiredVolunteers - need.CurrentVolunteersCount,
                Status = need.Status.ToString()
            };
        }


    }
}
