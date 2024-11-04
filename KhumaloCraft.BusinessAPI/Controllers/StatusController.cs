using Microsoft.AspNetCore.Mvc;
using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace KhumaloCraft.BusinessAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class StatusController : ControllerBase
  {
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
      _statusService = statusService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllStatus()
    {
      var status = await _statusService.GetAllStatus();
      return Ok(status);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStatusById(int id)
    {
      var status = await _statusService.GetStatusById(id);
      if (status == null) return NotFound();
      return Ok(status);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddStatus([FromBody] StatusDTO statusDTO)
    {
      await _statusService.AddStatus(statusDTO);
      return Ok("Status added successfully");
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusDTO statusDTO)
    {
      if (id != statusDTO.StatusId) return BadRequest("Status ID mismatch");

      await _statusService.UpdateStatus(statusDTO);
      return Ok("Status updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStatus(int id)
    {
      await _statusService.DeleteStatus(id);
      return Ok("Status deleted successfully");
    }
  }
}
