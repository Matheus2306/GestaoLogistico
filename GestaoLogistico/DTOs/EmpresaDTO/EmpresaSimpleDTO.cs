namespace GestaoLogistico.DTOs.EmpresaDTO
{
    public class EmpresaSimpleDTO
    {
        public Guid Id { get; set; }
        public string RazaoSocial { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string FullAdress { get; set; } = string.Empty;
        public List<EmpresaEmailDTO> Emails { get; set; } = new();
        public List<EmpresaTelefoneDTO> Telefones { get; set; } = new();
    }
}
