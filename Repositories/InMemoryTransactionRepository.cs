using System.Collections.Concurrent;
using System.Collections.Generic;
using TransacaoFinanceira.Models;

namespace TransacaoFinanceira.Repositories
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private readonly ConcurrentBag<RegistroTransacao> _history = new ConcurrentBag<RegistroTransacao>();

        public void Save(RegistroTransacao record)
        {
            _history.Add(record);
        }

        public IEnumerable<RegistroTransacao> GetAll()
        {
            return _history;
        }
    }
}
