using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockHackothonBackend.DTOs.Need;
using MockHackothonBackend.Services;
using System.Security.Claims;

namespace MockHackothonBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeedsController : ControllerBase
    {
        private readonly NeedsServices _needService;

        public NeedsController(NeedsServices needService)
        {
            _needService = needService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOpenNeeds()
        {
            var needs = await _needService.GetOpenNeeds();
            return Ok(needs);
        }

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> CreateNeed([FromBody] CreateNeedDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _needService.CreateNeed(dto, userId);

            if (result == null)
                return BadRequest(new { error = "Invalid data" });

            return CreatedAtAction(nameof(GetOpenNeeds), new { id = result.Id }, result);
        }

        [HttpGet("organization")]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> GetOrganizationNeeds()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var needs = await _needService.GetNeedsByOrganization(userId);
            return Ok(needs);
        }

        [HttpGet("{id}/volunteers")]
        [Authorize(Roles = "Organization")]
        public async Task<IActionResult> GetVolunteers(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var volunteers = await _needService.GetVolunteersForNeed(id, userId);
            return Ok(volunteers);
        }
    }
}
