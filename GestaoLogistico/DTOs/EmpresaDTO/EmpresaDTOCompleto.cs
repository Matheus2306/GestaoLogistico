namespace GestaoLogistico.DTOs.EmpresaDTO
{
    /// <summary>
    /// DTO completo de empresa com relacionamentos
    /// </summary>
    public class EmpresaDTOCompleto
    {
        public Guid EmpresaId { get; set; }
        public string RazaoSocial { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string? InscricaoEstadual { get; set; }
        public string? InscricaoMunicipal { get; set; }
        
        // Endereço
        public string? CEP { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? UF { get; set; }

        public bool Ativo { get; set; }

        public List<EmpresaEmailDTO> Emails { get; set; } = new();
        public List<EmpresaTelefoneDTO> Telefones { get; set; } = new();

        public string? UsuarioResponsavelId { get; set; }
        public string? UsuarioResponsavelNome { get; set; }

        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
