using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpPost]
        public async Task<ActionResult<InterviewResponseDto>> CreateInterview([FromBody] CreateInterviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // For demo purposes, using a default user ID
                // In production, extract from JWT token or session
                var userId = "default-user";

                var interview = await _interviewService.CreateInterviewAsync(dto, userId);
                return CreatedAtAction(nameof(GetInterview), new { id = interview.Id }, interview);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the interview", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InterviewResponseDto>> GetInterview(int id)
        {
            var interview = await _interviewService.GetInterviewByIdAsync(id);
            if (interview == null)
                return NotFound(new { message = "Interview not found" });

            return Ok(interview);
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewResponseDto>>> GetInterviews()
        {
            try
            {
                var interviews = await _interviewService.GetAllInterviewsAsync();
                return Ok(interviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving interviews", detail = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateInterviewStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            if (!Enum.TryParse<InterviewStatus>(dto.Status, out var status))
                return BadRequest(new { message = "Invalid status value" });

            var success = await _interviewService.UpdateInterviewStatusAsync(id, status);
            if (!success)
                return NotFound(new { message = "Interview not found" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInterview(int id)
        {
            var success = await _interviewService.DeleteInterviewAsync(id);
            if (!success)
                return NotFound(new { message = "Interview not found" });

            return NoContent();
        }
    }
}