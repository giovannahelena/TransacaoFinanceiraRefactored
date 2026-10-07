using Xunit;
using TransacaoFinanceira.Repositories;
using TransacaoFinanceira.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransacaoFinanceira.Tests
{
    public class TransactionServiceTests
    {
        private IAccountRepository CreateAccountRepo() => new InMemoryAccountRepository();
        private ITransactionRepository CreateTransactionRepo() => new InMemoryTransactionRepository();

        [Fact]
        public void Transfer_ShouldSucceed_WhenBalanceIsSufficient()
        {
            // Arrange
            var accountRepo = CreateAccountRepo();
            var transRepo = CreateTransactionRepo();
            var service = new TransactionService(accountRepo, transRepo);

            long originAccountNum = 938485762; // Initial Balance: 180
            long destAccountNum = 2147483649;  // Initial Balance: 0
            decimal amount = 50m;

            // Act
            var result = service.Transfer(originAccountNum, destAccountNum, amount);

            // Assert
            Assert.True(// result.Success
                result.Success);
            Assert.Equal(130m, accountRepo.GetAccount(originAccountNum).Saldo);
            Assert.Equal(50m, accountRepo.GetAccount(destAccountNum).Saldo);
        }

        [Fact]
        public void Transfer_ShouldFail_WhenBalanceIsInsufficient()
        {
            // Arrange
            var accountRepo = CreateAccountRepo();
            var transRepo = CreateTransactionRepo();
            var service = new TransactionService(accountRepo, transRepo);

            long originAccountNum = 938485762; // Initial Balance: 180
            long destAccountNum = 2147483649;
            decimal amount = 200m;

            // Act
            var result = service.Transfer(originAccountNum, destAccountNum, amount);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("falta de saldo", result.Mensagem);
        }

        [Fact]
        public void Transfer_ShouldFail_WhenAccountNotFound()
        {
            // Arrange
            var accountRepo = CreateAccountRepo();
            var transRepo = CreateTransactionRepo();
            var service = new TransactionService(accountRepo, transRepo);

            long invalidAccount = 999999999;
            long validAccount = 938485762;

            // Act
            var result = service.Transfer(invalidAccount, validAccount, 10m);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("não encontradas", result.Mensagem);
        }

        [Fact]
        public async Task Transfer_ConcurrentTransactions_ShouldMaintainConsistency()
        {
            // Arrange
            var accountRepo = CreateAccountRepo();
            var transRepo = CreateTransactionRepo();
            var service = new TransactionService(accountRepo, transRepo);

            long accountA = 938485762; // 180
            long accountB = 2147483649; // 0
            decimal initialTotal = accountRepo.GetAccount(accountA).Saldo + accountRepo.GetAccount(accountB).Saldo;

            int iterations = 10;
            decimal amountPerTx = 10m;

            // Act
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(Task.Run(() => service.Transfer(accountA, accountB, amountPerTx)));
            }
            await Task.WhenAll(tasks);

            // Assert
            decimal finalTotal = accountRepo.GetAccount(accountA).Saldo + accountRepo.GetAccount(accountB).Saldo;
            Assert.Equal(initialTotal, finalTotal);
            Assert.Equal(80m, accountRepo.GetAccount(accountA).Saldo);
            Assert.Equal(100m, accountRepo.GetAccount(accountB).Saldo);
        }
    }
}
