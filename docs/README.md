# Documentação da API de Pagamentos

Este diretório contém a collection do Postman para testar os endpoints da API REST de pagamentos.

## Como usar

1. Inicie a API localmente:

```bash
cd ~/Documents/prova_promarket
dotnet run --project ./src/Promarket.Payments.Api/Promarket.Payments.Api.csproj --urls http://localhost:5000
```

2. Abra o Postman.
3. Clique em Importar > Arquivo.
4. Selecione o arquivo:

```text
docs/Promarket-Pagamentos.postman_collection.json
```

5. Ajuste a variável `baseUrl`, se necessário.

## Endpoints

- `POST /pagamentos`
- `GET /pagamentos?ordem=asc|desc`
- `GET /pagamentos/{id}`
- `PUT /pagamentos/{id}`
- `DELETE /pagamentos/{id}`

## Payload do POST

```json
{
  "id": 1001,
  "pedidoId": 10,
  "eventoId": 20,
  "valor": 99.90
}
```

A propriedade `id` é a chave de idempotência: requisições repetidas com o mesmo valor em intervalo curto devem ser rejeitadas com `409 Conflict`.

## Resposta de sucesso

```json
{
  "status": "ok",
  "idPagamento": 1
}
```

## Observações

- O campo `id` é o identificador do pagamento gerado pelo banco.
- O campo `pedidoId` é o ID do pedido informado na criação.
- A API usa SQLite localmente e cria o arquivo `pagamentos.db` na raiz do projeto.
