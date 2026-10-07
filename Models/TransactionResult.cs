namespace TransacaoFinanceira.Models
{
    public class TransactionResult
    {
        public bool Success { get; set; }
        public string Mensagem { get; set; }
        public decimal OriginBalance { get; set; }
        public decimal DestinationBalance { get; set; }
    }
}
