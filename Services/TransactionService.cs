using System;
using TransacaoFinanceira.Models;
using TransacaoFinanceira.Repositories;

namespace TransacaoFinanceira.Services
{
    public class TransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public TransactionResult Transfer(long numeroContaOrigem, long numeroContaDestino, decimal valor)
        {
            Guid correlationId = Guid.NewGuid();
            var firstLock = numeroContaOrigem < numeroContaDestino ? numeroContaOrigem : numeroContaDestino;
            var secondLock = numeroContaOrigem < numeroContaDestino ? numeroContaDestino : numeroContaOrigem;

            var contaOrigem = _accountRepository.GetAccount(numeroContaOrigem);
            var contaDestino = _accountRepository.GetAccount(numeroContaDestino);

            if (contaOrigem == null || contaDestino == null)
            {
                var msg = $"Transação {correlationId} falhou: uma ou ambas as contas não encontradas.";
                SaveHistory(correlationId, numeroContaOrigem, numeroContaDestino, valor, StatusTransacao.Cancelado, msg);
                return new TransactionResult { Success = false, Mensagem = msg };
            }

            lock (GetLockObject(firstLock))
            {
                lock (GetLockObject(secondLock))
                {
                    if (contaOrigem.Saldo < valor)
                    {
                        var msg = $"Transação {correlationId} cancelada por falta de saldo.";
                        SaveHistory(correlationId, numeroContaOrigem, numeroContaDestino, valor, StatusTransacao.Cancelado, msg);
                        return new TransactionResult
                        {
                            Success = false,
                            Mensagem = msg
                        };
                    }

                    contaOrigem.Saldo -= valor;
                    contaDestino.Saldo += valor;

                    var msgSucesso = $"Transação {correlationId} processada com sucesso!";
                    SaveHistory(correlationId, numeroContaOrigem, numeroContaDestino, valor, StatusTransacao.Sucesso, msgSucesso);

                    return new TransactionResult
                    {
                        Success = true,
                        Mensagem = msgSucesso,
                        OriginBalance = contaOrigem.Saldo,
                        DestinationBalance = contaDestino.Saldo
                    };
                }
            }
        }

        private void SaveHistory(Guid id, long origin, long dest, decimal valor, StatusTransacao status, string mensagem)
        {
            _transactionRepository.Save(new RegistroTransacao(
                id,
                origin,
                dest,
                valor,
                DateTime.Now,
                status,
                mensagem
            ));
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<long, object> _locks =
            new System.Collections.Concurrent.ConcurrentDictionary<long, object>();

        private object GetLockObject(long numeroConta)
        {
            return _locks.GetOrAdd(numeroConta, _ => new object());
        }
    }
}
