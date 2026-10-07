# Relatório de Refatoração: Projeto TransacaoFinanceira

## 1. Introdução
Este documento trata-se de um sistema de transações financeiras. O projeto original apresentava falhas críticas de concorrência, erros de runtime e uma estrutura monolítica que dificultava a manutenção e a testabilidade. O objetivo foi transformar o código em uma aplicação robusta, escalável e seguindo os mais rigorosos padrões de engenharia de software.

## 2. Modificações Realizadas

### 🛠️ Correções Funcionais e Técnicas
- **Correções Básicas Necessárias para Build**:
    - Atualização do .Net Target Framework de 5.0, muito antigo, para 10.0.
	- Utilização de tipo long, ao invés de int, para guardar os números das contas, evitando exceder o Int.MaxValue (2,147,483,647), como ocorria na linha 16 do arquivo Program.cs no projeto original

- **Sincronização e Thread-Safety**: 
    - Implementação de **Locking Hierárquico**: Para evitar *Race Conditions* e *Deadlocks* em transferências paralelas, as contas são travadas sempre em ordem crescente de ID.
    - Uso de `ConcurrentDictionary` e `ConcurrentBag` para garantir a integridade dos dados em ambiente multithread.
- **Tratamento de Tipos e Overflow**: 
    - Correção de erro de convers uma de tipos no `getSaldo` original.
    - **Uso de Tipos de Dados Expandidos (`long`)**: Para suportar identificadores de conta que excedem o limite de `Int32` (ex: conta `2147483649`), foi implementada a utilização de `long` nos modelos de requisição, repositórios e histórico, combinada com a sintaxe `unchecked` onde necessário para garantir a consistência dos IDs.
- **Identificação Única (GUID)**:
    - Implementação de `System.Guid` para a correlação de transações. O sistema gera dinamicamente um identificador globalmente único (`Guid.NewGuid()`) para cada transação, garantindo a rastreabilidade total e eliminando a dependência de IDs sequenciais mockados.
- **Externalização e Automação de Dados**:
    - Migração de dados "hardcoded" para um arquivo `transactions.json`, permitindo a alteração de cenários de teste sem a necessidade de recompilação.
    - Configuração de **Build Automation** via `.csproj` (`CopyToOutputDirectory`), garantindo que os arquivos de dados sejam automaticamente copiados para o diretório de execução, eliminando erros de "Arquivo Não Encontrado".

### 🏗️ Melhorias Estruturais e Arquitetura
O projeto foi migrado de um script único para uma **Arquitetura em Camadas (Layered Architecture)** com separação rigorosa de responsabilidades:

1.  **Camada de Modelos (`/Models`)**:
    - Utilização de **`records`** para `TransactionRequest` e `RegistroTransacao`, garantindo imutabilidade para DTOs e registros de auditoria.
    - `Conta`: Representa o estado da conta.
    - `StatusTransacao`: Enum para status de transação (Success/Cancelled).

2.  **Camada de Repositórios (`/Repositories`)**:
    - Implementação do padrão **Repository**, desacoplando a lógica de negócio do armazenamento.
    - `IAccountRepository` e `ITransactionRepository`: Interfaces que permitem a troca da implementação de armazenamento (ex: de memória para SQL) sem impactar o restante do sistema.

3.  **Camada de Serviços (`/Services`)**:
    - `TransactionService`: Centraliza a regra de negócio (validação de saldo, locks e transferência).
    - `TransactionLoader`: Especialista na leitura e desserialização do arquivo JSON, removendo essa responsabilidade da camada de apresentação.
    - `ProcessadorTransacoes`: Orquestrador de alto nível que coordena o fluxo completo: Carga $\rightarrow$ Processamento $\rightarrow$ Sumário.

4.  **Camada de Apresentação (`Program.cs`)**:
    - Atua exclusivamente como um **Composition Root**. É responsável apenas por instanciar as dependências e disparar o processador.

## 3. Funcionamento do Projeto

### Fluxo de uma Transação:
1.  **Carga**: O `TransactionLoader` lê e valida o arquivo `transactions.json`.
2.  **Orquestração**: O `ProcessadorTransacoes` dispara as transferências via `Parallel.ForEach`.
3.  **Processamento**:
    - O `TransactionService` gera um **GUID** único para a operação.
    - Aplica travas (`lock`) ordenadas por ID de conta para evitar deadlocks.
    - Verifica a disponibilidade de saldo.
    - Se válido: Debita da origem $\rightarrow$ Credita no destino $\rightarrow$ Registra "Sucesso" no histórico.
    - Se inválido: Registra "Cancelado" no histórico com o motivo.
4.  **Finalização**: O sistema gera um sumário final de auditoria recuperando todos os registros do `ITransactionRepository`.

## 4. Princípios de Software Aplicados

- **SOLID**:
    - **SRP (Single Responsibility)**: Cada classe tem uma única responsabilidade clara (ex: `TransactionLoader` apenas carrega dados).
    - **DIP (Dependency Inversion)**: O serviço e o processador dependem de interfaces, facilitando a testabilidade e a extensibilidade.
- **Clean Code**: Nomenclatura em `PascalCase`, remoção de lógica complexa do `Main` e uso de tipos imutáveis.
- **Auditabilidade**: Implementação de log de transações com identificadores únicos, essencial para conformidade em sistemas financeiros.

## 5. Conclusão
A refatoração eliminou todos os bugs de concorrência e runtime, transformando um código instável em um sistema modular, auditável e profissional. A arquitetura implementada permite que o sistema evolua facilmente para suportar bancos de dados reais e novas regras de negócio sem a necessidade de refatorações profundas.
