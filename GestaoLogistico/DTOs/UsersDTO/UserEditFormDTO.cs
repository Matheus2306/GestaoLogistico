namespace GestaoLogistico.DTOs.UsersDTO
{
    public class UserEditFormDTO
    {
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }
}
