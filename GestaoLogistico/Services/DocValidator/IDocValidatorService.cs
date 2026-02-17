namespace GestaoLogistico.Services.DocValidator
{
    public interface IDocValidatorService
    {
        bool ValidarCPF(string cpf);
        bool ValidarCNPJ(string cnpj);
    }
}
