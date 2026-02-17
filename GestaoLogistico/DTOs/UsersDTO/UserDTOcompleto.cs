namespace GestaoLogistico.DTOs.UsersDTO
{
    public class UserDTOcompleto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string atualizadoEm { get; set; }
        public string? criadoPor { get; set; }
        public string? atualizadoPor { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public IList<string> Roles { get; set; }
    }
}
