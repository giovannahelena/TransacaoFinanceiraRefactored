namespace TransacaoFinanceira.Models
{
    public class Conta
    {
        public long NumeroConta { get; }
        public decimal Saldo { get; set; }

        public Conta(long numeroConta, decimal saldoInicial)
        {
            NumeroConta = numeroConta;
            Saldo = saldoInicial;
        }
    }
}
