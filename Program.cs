using System;
using TransacaoFinanceira.Repositories;
using TransacaoFinanceira.Services;

namespace TransacaoFinanceira
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IAccountRepository accountRepo = new InMemoryAccountRepository();
                ITransactionRepository transactionRepo = new InMemoryTransactionRepository();
                TransactionLoader loader = new TransactionLoader();
                TransactionService transactionService = new TransactionService(accountRepo, transactionRepo);
                ProcessadorTransacoes processor = new ProcessadorTransacoes(transactionService, transactionRepo, loader);

                processor.ProcessarTransacoes("transacoes.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }
}
