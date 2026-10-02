# Prova Promarket - API de Pagamentos

Repositório GitHub: https://github.com/victorbo3110/prova_promarket.git

Aplicação REST para registrar pagamentos, com arquitetura voltada para evolução e preparação para mensageria em futuras integrações.

## Visão geral

A aplicação foi implementada em C# com ASP.NET Core e SQLite, seguindo uma estrutura simples mas organizada em camadas:

- `src/Promarket.Payments.Api` — API, controller, serviço e acesso a dados
- `tests/Promarket.Payments.Api.Tests` — testes de integração da API
- `docs/` — documentação e collection do Postman

## Requisitos

Antes de rodar o projeto, verifique se você possui:

- .NET SDK 8.0+
- Git
- Um terminal Bash, PowerShell ou CMD

### Instalar o .NET 8 SDK

No Windows, você pode instalar via winget:

```powershell
winget install --id Microsoft.DotNet.SDK.8 --source winget -e
```

Depois confirme:

```powershell
dotnet --version
```

## Clonar e preparar o projeto

```bash
git clone <url-do-repositorio>
cd prova_promarket
```

Se o projeto já estiver baixado localmente:

```bash
cd ~/Documents/prova_promarket
```

## Restaurar dependências

```bash
dotnet restore
```

## Compilar

```bash
dotnet build --nologo
```

## Executar a API

### Opção 1 — usar porta padrão 5000

```bash
dotnet run --project ./src/Promarket.Payments.Api/Promarket.Payments.Api.csproj --urls http://localhost:5000
```

### Opção 2 — usar outra porta

```bash
dotnet run --project ./src/Promarket.Payments.Api/Promarket.Payments.Api.csproj --urls http://localhost:5001
```

### Observação importante

Se a mensagem abaixo aparecer:

```text
Failed to bind to address http://127.0.0.1:5000: address already in use.
```

significa que outra aplicação já está usando a porta 5000. Nesse caso:

```powershell
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

ou use outra porta, como 5001.

## Endpoints da API

### Criar pagamento

```http
POST /pagamentos
```

Body:

```json
{
  "pedidoId": 10,
  "eventoId": 20,
  "valor": 99.90
}
```

Resposta de sucesso:

```json
{
  "status": "ok",
  "idPagamento": 1
}
```

### Listar pagamentos

```http
GET /pagamentos?ordem=asc
GET /pagamentos?ordem=desc
```

### Buscar pagamento por id

```http
GET /pagamentos/{idPagamento}
```

### Atualizar pagamento

```http
PUT /pagamentos/{idPagamento}
```

Body:

```json
{
  "pedidoId": 15,
  "eventoId": 25,
  "valor": 150.00
}
```

### Deletar pagamento

```http
DELETE /pagamentos/{idPagamento}
```

## Swagger

Quando a API estiver rodando em ambiente de desenvolvimento, o Swagger fica disponível em:

```text
http://localhost:5000/swagger
```

## Banco de dados

A aplicação usa SQLite e cria automaticamente o banco local `pagamentos.db` ao iniciar.

## Como a idempotência foi tratada

A idempotência foi pensada na criação de qualquer registro e aplicada no fluxo de criação de pagamento usando um `id` de operação exclusivo da requisição.

### Estratégia adotada

O cliente envia um `id` único para identificar a tentativa de pagamento. Se a mesma requisição for reenviada em um curto intervalo, a API rejeita a segunda tentativa com `409 Conflict` e não cria um novo pagamento duplicado.

Isso evita duplicidade por retry de rede, reenvio do cliente ou processamento repetido da mesma operação.

### Implementação

No serviço de aplicação, o método de criação faz:

```csharp
var repetido = await _context.Pagamentos
    .AnyAsync(p => p.RequestId == request.Id);

if (repetido)
    throw new InvalidOperationException("Requisição duplicada: esta operação já foi processada recentemente.");
```

Ou seja:

- se a mesma operação chegar novamente com o mesmo `id`,
- a API responde como duplicada,
- e não gera um segundo pagamento.

### Por que isso importa

Em cenários reais de reenvio ou retry, isso reduz:

- duplicidade de pagamentos
- inconsistência de dados
- processamento acidental do mesmo pedido mais de uma vez

## Testes

Para executar os testes da API:

```bash
dotnet test --nologo
```

## Registro dos prompts usados com a IA

Os prompts principais usados durante o desenvolvimento da tarefa foram:

1. "Crie uma API REST .NET com endpoint POST /pagamentos, SQLite e estrutura hexagonal simples."
2. "Adicione idempotência para evitar duplicação quando o mesmo eventoId chega mais de uma vez."
3. "Implemente testes automatizados cobrindo o caso de duplicidade e listagem dos pagamentos."
4. "Crie um README curto com instruções de execução e regras do projeto."
5. "Gere uma collection do Postman para testar os endpoints da API."

Esses prompts foram usados para guiar a implementação, validar a arquitetura e confirmar o comportamento esperado em testes automatizados.

## Collection do Postman

A documentação da collection está em:

```text
docs/Promarket-Pagamentos.postman_collection.json
```

Além disso, o README da documentação está em:

```text
docs/README.md
```

## Observações finais

Este projeto foi estruturado para ser escalável e preparado para evoluir para mensageria, como por exemplo integração com AWS SQS no futuro, sem quebrar a API atual baseada em REST.
