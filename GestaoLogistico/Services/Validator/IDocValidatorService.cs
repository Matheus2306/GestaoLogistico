namespace GestaoLogistico.Services.DocValidator
{
    public interface IDocValidatorService
    {
        string ValidarCPF(string cpf);
        string ValidarCNPJ(string cnpj);
    }
}
