using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockHackothonBackend.Commitment;
using MockHackothonBackend.Services;
using System.Security.Claims;

namespace MockHackothonBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Volunteer")]
    public class CommitmentsController : ControllerBase
    {
        private readonly CommitmentService _commitmentService;

        public CommitmentsController(CommitmentService commitmentService)
        {
            _commitmentService = commitmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CommitToNeed([FromBody] CommitDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var (success, message, remainingSlots) = await _commitmentService.CommitToNeed(dto, userId);
            
            if (!success)
                return BadRequest(new { error = message });

            return Ok(new 
            { 
                message = message,
                needId = dto.NeedId,
                remainingSlots = remainingSlots
            });
        }
    }
}
