namespace TransacaoFinanceira.Models
{
    public record TransactionRequest(long account_origin, long account_destination, decimal value);
}
