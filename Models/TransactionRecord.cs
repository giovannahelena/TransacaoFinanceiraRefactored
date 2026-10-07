using System;

namespace TransacaoFinanceira.Models
{
    public enum StatusTransacao
    {
        Sucesso,
        Cancelado
    }

    public record RegistroTransacao(
        Guid CorrelationId,
        long ContaOrigem,
        long ContaDestino,
        decimal Valor,
        DateTime Timestamp,
        StatusTransacao Status,
        string Mensagem
    );
}
