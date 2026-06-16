    using EvaluationSystem.Application.DTOs.Department;
    using EvaluationSystem.Application.interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace EvaluationSystem.Api.Controllers
    {
   
        [Route("api/[controller]")]
        [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
        {
            private readonly IDepartmentService _departmentService;

            public DepartmentsController(
                IDepartmentService departmentService)
            {
                _departmentService = departmentService;
            }

            [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] DepartmentSort_Page dto)
            {
                var departments =
                    await _departmentService.GetAllAsync(dto);

                return Ok(departments);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var department =
                    await _departmentService.GetByIdAsync(id);

                return Ok(department);
            }

            [HttpPost]
            public async Task<IActionResult> Create(
                [FromBody] CreateDepartmentDto dto)
            {
                var department =
                    await _departmentService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = department.Id },
                    department);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(
                int id,
                [FromBody] CreateDepartmentDto dto)
            {
                await _departmentService.UpdateAsync(id, dto);

                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                await _departmentService.DeleteAsync(id);

                return NoContent();
            }
        }
    }