namespace GestaoLogistico.Services.FormatService
{
    public class FormatService : IFormatService
    {
        private readonly ILogger<FormatService> _logger;
        
        public FormatService(ILogger<FormatService> logger)
        {
            _logger = logger;
        }

        public string SetupFormatDocument(string valor)
        {
            if (String.IsNullOrWhiteSpace(valor))
                return valor;
            var apenasNumeros = RemoverCaracteresNaoNumericos(valor);
            return apenasNumeros.Length switch
            {
                11 => FormatCpf(apenasNumeros),
                14 => FormatCnpj(apenasNumeros),
                _ => valor
            };

        }

        public string SetupFormatPhone(string valor)
        {
            if (String.IsNullOrWhiteSpace(valor))
                return valor;
            var apenasNumeros = RemoverCaracteresNaoNumericos(valor);
            return apenasNumeros.Length switch
            {
                10 => FormatTelefoneFixo(apenasNumeros),
                11 => FormatTelefoneCelular(apenasNumeros),
                _ => valor
            };
        }

        public string FormatCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return cpf;

            var apenasNumeros = RemoverCaracteresNaoNumericos(cpf);

            if (apenasNumeros.Length != 11)
            {
                _logger.LogWarning("CPF inválido: deve conter 11 dígitos");
                return cpf;
            }

            return $"{apenasNumeros.Substring(0, 3)}.{apenasNumeros.Substring(3, 3)}.{apenasNumeros.Substring(6, 3)}-{apenasNumeros.Substring(9, 2)}";
        }

        public string FormatCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return cnpj;

            var apenasNumeros = RemoverCaracteresNaoNumericos(cnpj);

            if (apenasNumeros.Length != 14)
            {
                _logger.LogWarning("CNPJ inválido: deve conter 14 dígitos");
                return cnpj;
            }

            return $"{apenasNumeros.Substring(0, 2)}.{apenasNumeros.Substring(2, 3)}.{apenasNumeros.Substring(5, 3)}/{apenasNumeros.Substring(8, 4)}-{apenasNumeros.Substring(12, 2)}";
        }

        public string FormatTelefoneFixo(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return telefone;

            var apenasNumeros = RemoverCaracteresNaoNumericos(telefone);

            if (apenasNumeros.Length != 10)
            {
                _logger.LogWarning("Telefone fixo inválido: deve conter 10 dígitos");
                return telefone;
            }

            return $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2, 4)}-{apenasNumeros.Substring(6, 4)}";
        }

        public string FormatTelefoneCelular(string celular)
        {
            if (string.IsNullOrWhiteSpace(celular))
                return celular;

            var apenasNumeros = RemoverCaracteresNaoNumericos(celular);

            if (apenasNumeros.Length != 11)
            {
                _logger.LogWarning("Telefone celular inválido: deve conter 11 dígitos");
                return celular;
            }

            return $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2, 5)}-{apenasNumeros.Substring(7, 4)}";
        }

        private string RemoverCaracteresNaoNumericos(string valor)
        {
            return new string(valor.Where(char.IsDigit).ToArray());
        }
    }
}
