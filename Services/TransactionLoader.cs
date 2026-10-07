using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TransacaoFinanceira.Models;

namespace TransacaoFinanceira.Services
{
    public class TransactionLoader
    {
        public List<TransactionRequest> LoadTransactions(string arquivo)
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", arquivo);

                if (!File.Exists(jsonPath))
                {
                    jsonPath = Path.Combine(Environment.CurrentDirectory, "Data", arquivo);
                }

                if (!File.Exists(jsonPath))
                {
                    throw new FileNotFoundException($"Não encontrado arquivo com dados de transações {jsonPath}");
                }

                string jsonString = File.ReadAllText(jsonPath);
                return JsonSerializer.Deserialize<List<TransactionRequest>>(jsonString) ?? new List<TransactionRequest>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível recuperar os dados de transações {ex.Message}", ex);
            }
        }
    }
}
