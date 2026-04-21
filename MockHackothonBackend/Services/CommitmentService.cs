using Microsoft.EntityFrameworkCore;
using MockHackothonBackend.Commitment;
using MockHackothonBackend.Data;
using MockHackothonBackend.Enums;

namespace MockHackothonBackend.Services
{
    public class CommitmentService
    {
        private readonly AppDbContext _context;

        public CommitmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, int? RemainingSlots)> CommitToNeed(CommitDto dto, int userId)
        {
            // Start transaction - CRITICAL FOR PREVENTING OVERBOOKING
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Fetch need with lock
                var need = await _context.Needs
                    .FirstOrDefaultAsync(n => n.Id == dto.NeedId);

                if (need == null)
                    return (false, "Need not found", null);

                if (need.Status == NeedStatus.Closed)
                    return (false, "Need is already full", null);

                // Check if already committed
                var existingCommitment = await _context.Commitments
                    .AnyAsync(c => c.UserId == userId && c.NeedId == dto.NeedId);

                if (existingCommitment)
                    return (false, "Already committed to this need", null);

                // CRITICAL CHECK: Prevent overbooking
                if (need.CurrentVolunteersCount >= need.RequiredVolunteers)
                    return (false, "Need is already full", 0);

                // Increment count
                need.CurrentVolunteersCount++;

                // Auto-close if full
                if (need.CurrentVolunteersCount >= need.RequiredVolunteers)
                {
                    need.Status = NeedStatus.Closed;
                }

                // Create commitment
                var commitment = new Commitment
                {
                    UserId = userId,
                    NeedId = dto.NeedId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Commitments.Add(commitment);
                
                // Save changes
                await _context.SaveChangesAsync();
                
                // Commit transaction
                await transaction.CommitAsync();

                int remainingSlots = need.RequiredVolunteers - need.CurrentVolunteersCount;

                return (true, "Successfully committed to need", remainingSlots);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error committing: {ex.Message}", null);
            }
        }
    }
}
