namespace GestaoLogistico.DTOs.UsersDTO
{
    public class UserSimpleDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string UrlPhoto { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IList<string> Roles { get; set; }
    }
}
