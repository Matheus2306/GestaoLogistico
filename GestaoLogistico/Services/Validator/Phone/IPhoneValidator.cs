namespace GestaoLogistico.Services.Validator.Phone
{
    public interface IPhoneValidator
    {
        public bool IsValid(string phone);
        public string RemoveFormatting(string phone);
    }
}
