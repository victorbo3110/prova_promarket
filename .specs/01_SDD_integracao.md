# Arquitetura

- A aplicação é inicialmente apenas uma camada, porém já implementada com arquitetura hexagonal, ou seja, temos a pasta core e nela a aplicação de pagamento.

# Tecnologias e bibliotecas

- Aplicação CRUD simples, utilizando C# (ASP.NET) e SQLite na camada de persistencia.

# Regras de negócio

- Idempotência sempre deve ser pensada na criação de qualquer classe ou método.
- Aplicação deve ser escalável para estar preparada para trabalhar com mensageria (AWS SQS), porém em primeiro momento será apenas REST com controllers

# Rotas

POST /pagamentos -> Grava em banco e gera {{idPagamento}} 
GET /pagamentos -> Busca os últimos pagamentos (Parametros ASC/DESC)
GET /pagamentos/{{idPagamento}} -> Busca um pagamento por id gerado após registro
PUT /pagamentos/{{idPagamento}} -> Atualiza os dados do pagamento
DELETE /pagamentos/{{idPagamento}} - Deleta pagamento por ID

# Contratos da API

POST /pagamentos deve ter no body em Json

{
     "pedidoId": int
     "eventoId": int 
     "valor": float
}

Response (200)

{
    "status": "ok"
    "idPagamento": int
}

Response (400)

{
    "Status": "Inválido"
    "Mensagem": str (mensagem de erro)
}

