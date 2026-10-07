using System.Collections.Generic;
using TransacaoFinanceira.Models;

namespace TransacaoFinanceira.Repositories
{
    public interface ITransactionRepository
    {
        void Save(RegistroTransacao record);
        IEnumerable<RegistroTransacao> GetAll();
    }
}
