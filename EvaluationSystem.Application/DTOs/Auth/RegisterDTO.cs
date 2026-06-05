namespace EvaluationSystem.Application.DTOs.Auth
{
    public class RegisterDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int DepartmentId { get; set; } 
    }
}
