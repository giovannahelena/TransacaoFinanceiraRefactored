using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TransacaoFinanceira.Models;

namespace TransacaoFinanceira.Repositories
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        private readonly ConcurrentDictionary<long, Conta> _accounts = new ConcurrentDictionary<long, Conta>();

        public InMemoryAccountRepository()
        {
            var initialAccounts = new List<Conta>
            {
                new Conta(938485762, 180),
                new Conta(347586970, 1200),
                new Conta(2147483649, 0),
                new Conta(675869708, 4900),
                new Conta(238596054, 478),
                new Conta(573659065, 787),
                new Conta(210385733, 10),
                new Conta(674038564, 400),
                new Conta(563856300, 1200)
            };

            foreach (var Conta in initialAccounts)
            {
                _accounts.TryAdd(Conta.NumeroConta, Conta);
            }
        }

        public Conta GetAccount(long numeroConta)
        {
            _accounts.TryGetValue(numeroConta, out var Conta);
            return Conta;
        }

        public void UpdateAccount(Conta Conta)
        {
            _accounts[Conta.NumeroConta] = Conta;
        }

        public IEnumerable<Conta> GetAllAccounts()
        {
            return _accounts.Values;
        }
    }
}
