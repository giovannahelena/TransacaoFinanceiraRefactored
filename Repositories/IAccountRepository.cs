using TransacaoFinanceira.Models;
using System.Collections.Generic;

namespace TransacaoFinanceira.Repositories
{
    public interface IAccountRepository
    {
        Conta GetAccount(long numeroConta);
        void UpdateAccount(Conta Conta);
        IEnumerable<Conta> GetAllAccounts();
    }
}
