namespace GestaoLogistico.Services.DocValidator
{
    public class DocValidatorService : IDocValidatorService
    {
 

        public string ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return null;

            if (!cpf.All(char.IsDigit))
                return null;

            if (cpf.Distinct().Count() == 1)
                return null;

            int[] multiplicador1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplicador2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf;
        }

        public string ValidarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return string.Empty;

            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

            if (cnpj.Length != 14)
                return null;

            if (!cnpj.All(char.IsDigit))
                return null;


            if (cnpj.Distinct().Count() == 1)
                return null;

            int[] multiplicador1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplicador2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;

            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cnpj;
        }
    }
}
