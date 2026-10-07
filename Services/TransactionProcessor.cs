using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransacaoFinanceira.Models;
using TransacaoFinanceira.Repositories;
using TransacaoFinanceira.Services;

namespace TransacaoFinanceira.Services
{
    public class ProcessadorTransacoes
    {
        private readonly TransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly TransactionLoader _loader;

        public ProcessadorTransacoes(TransactionService transactionService, ITransactionRepository transactionRepository, TransactionLoader loader)
        {
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
            _loader = loader;
        }

        public void ProcessarTransacoes(string fileName)
        {
            var transactions = _loader.LoadTransactions(fileName);

            Console.WriteLine("Processando transações\n");

            Parallel.ForEach(transactions, item =>
            {
                var result = _transactionService.Transfer(item.account_origin, item.account_destination, item.value);

                if (result.Success)
                {
                    Console.WriteLine($"{result.Mensagem} Saldos Atualizados - Origem:{result.OriginBalance} | Destino: {result.DestinationBalance}");
                }
                else
                {
                    Console.WriteLine(result.Mensagem);
                }
            });

            ExibirHistorico();
        }

        private void ExibirHistorico()
        {
            Console.WriteLine("\n--- HISTÓRICO DE TRANSAÇÕES ---");
            foreach (var record in _transactionRepository.GetAll())
            {
                Console.WriteLine($"ID: {record.CorrelationId} | Status: {record.Status} | Valor: {record.Valor} | Mensagem: {record.Mensagem}");
            }
        }
    }
}
