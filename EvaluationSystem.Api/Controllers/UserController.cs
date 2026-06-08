using EvaluationSystem.Application.DTOs.User;
using EvaluationSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserSearchSort searchSort)
        {
            var users = await _userService.ListUsersAsync(searchSort);
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        [HttpPut("{id}/department")]
        public async Task<IActionResult> AssignDepartment(int id, [FromBody] AssignDepartmentDto dto)
        {
            await _userService.AssignDepartmentAsync(id, dto);
            return NoContent();
        }
        [HttpPut("{id}/role")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleDto dto)
        {
            await _userService.AssignRoleAsync(id, dto);
            return NoContent();
        }
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            await _userService.ActivateUser(id);
            return NoContent();
        }
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            await _userService.DeactivateUser(id);
            return NoContent();
        }
    }
}
