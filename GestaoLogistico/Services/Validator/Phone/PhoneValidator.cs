using System.Text.RegularExpressions;

namespace GestaoLogistico.Services.Validator.Phone
{
    public class PhoneValidator : IPhoneValidator
    {
        public string RemoveFormatting(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            return Regex.Replace(phone, @"[^\d]", string.Empty);
        }

        public bool HasOnlyNumbers(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return Regex.IsMatch(phone, @"^\d+$");
        }

        public bool IsValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var cleanPhone = RemoveFormatting(phone);

            // Telefone brasileiro deve ter 10 (fixo) ou 11 (celular) dígitos
            if (cleanPhone.Length != 10 && cleanPhone.Length != 11)
                return false;

            // Verifica se tem apenas números
            if (!HasOnlyNumbers(cleanPhone))
                return false;

            // Verifica DDD (primeiros 2 dígitos devem estar entre 11 e 99)
            if (!int.TryParse(cleanPhone.Substring(0, 2), out int ddd) || ddd < 11 || ddd > 99)
                return false;

            return true;
        }

        public string Format(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            var cleanPhone = RemoveFormatting(phone);

            if (!IsValid(cleanPhone))
                return cleanPhone;

            if (cleanPhone.Length == 11)
            {
                // Formato: (XX) 9XXXX-XXXX
                return $"({cleanPhone.Substring(0, 2)}) {cleanPhone.Substring(2, 5)}-{cleanPhone.Substring(7, 4)}";
            }
            else if (cleanPhone.Length == 10)
            {
                // Formato: (XX) XXXX-XXXX
                return $"({cleanPhone.Substring(0, 2)}) {cleanPhone.Substring(2, 4)}-{cleanPhone.Substring(6, 4)}";
            }

            return cleanPhone;
        }
    }
}
