
# 🏢 Ambev Developer Evaluation API

API RESTful desenvolvida como parte do processo de avaliação técnica da Ambev.  
A aplicação foi construída seguindo os princípios da **Clean Architecture**, utilizando **.NET 7**, **Entity Framework Core (PostgreSQL)**, **MediatR**, **Serilog** e **Docker Compose** para orquestração do ambiente.

---

## 📚 Sumário
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Como rodar o projeto](#como-rodar-o-projeto)
- [Endpoints](#endpoints)
- [Testes](#testes)
- [Logging](#logging)
- [Health Checks](#health-checks)
- [Decisões Técnicas](#decisões-técnicas)
- [Autores](#autores)

---

## 🚀 Tecnologias

- .NET 7
- Entity Framework Core (PostgreSQL)
- MediatR (CQRS + Pipeline Behaviors)
- Serilog (Logs estruturados)
- FluentValidation
- Swagger / OpenAPI
- JWT Authentication
- Health Checks
- Docker e Docker Compose

---

## 🏛️ Arquitetura

O projeto foi desenvolvido seguindo os princípios da **Clean Architecture**, com separação clara de responsabilidades.

```
├── Domain
├── Application
├── Infrastructure (ORM, IoC)
├── WebApi
```

---

## 🔧 Como rodar o projeto

### Pré-requisitos

- Docker
- Docker Compose v2

### Passo a passo

1. Clone o repositório:

```bash
git clone https://github.com/StunerX/Ambev.DeveloperEvaluation.git
cd seu-projeto
```

2. Rode o Docker Compose:

```bash
docker compose up -d --build
```
após isso, verifique se os serviços estao rodando

```bash
docker ps -a
```

caso o serviço ambev_developer_evaluation_webapi estiver parado, apenas execute:

```bash
docker start ambev_developer_evaluation_webapi
```
com tudo certo, o [swagger](http://localhost:8080/swagger/index.html) ficara disponivel

### Serviços que serão criados:

| Serviço                          | Porta local | Container Name                      |
|----------------------------------|-------------|------------------------------------|
| Web API                          | 8080        | ambev_developer_evaluation_webapi  |
| PostgreSQL                       | 5432        | ambev_developer_evaluation_database |
| MongoDB                          | 27017       | ambev_developer_evaluation_nosql   |
| Redis                            | 6379        | ambev_developer_evaluation_cache   |

---

## 🌐 Endpoints

| Verbo  | Endpoint           | Descrição                  |
|--------|--------------------|----------------------------|
| GET    | `/api/Sales`       | Lista todas as vendas      |
| GET    | `/api/Sales/{id}`  | Busca uma venda por Id     |
| POST   | `/api/Sales`       | Cria uma nova venda        |
| PUT    | `/api/Sales/{id}`  | Atualiza uma venda         |
| DELETE | `/api/Sales/{id}`  | Deleta (soft delete) uma venda |

---

## ✅ Testes

O projeto inclui:

- **Unit Tests:** Cobrem regras de negócio e validações de entrada.
- **Integration Tests:** Validam a persistência no banco com EF Core e PostgreSQL.
- **Functional Tests:** Validam os endpoints da API de ponta a ponta via HTTP.

### Rodando os testes:

```bash
dotnet test ./tests/Ambev.DeveloperEvaluation.Unit
dotnet test ./tests/Ambev.DeveloperEvaluation.Integration
dotnet test ./tests/Ambev.DeveloperEvaluation.Functional
```

---

## 📄 Logging

A aplicação usa **Serilog**, com configuração para:

- Logs no Console (modo Debug)
- Logs em Arquivo (modo Release)
- Enriquecimento com `Serilog.Exceptions` para detalhes de exceções

Exemplo de log:

```
[2025-03-15 14:32:20.000 +00:00] [INF] [CreateSaleHandler] Successfully created Sale with Id: 123e4567-e89b-12d3-a456-426614174000
```

---

## 🏥 Health Checks

A aplicação expõe um endpoint de health check:

```
/health
```

Verifica:

- Conexão com o banco de dados PostgreSQL

---

## 🔍 Decisões Técnicas

- **CQRS com MediatR** para separação de comandos e queries.
- **Serilog** como logger estruturado.
- **Soft Delete** com campo `DeletedAt` nas entidades.
- **Validations** com FluentValidation nos comandos.
- **JWT Authentication** para autenticação e autorização.
- **Middleware Global** para tratamento de exceções.
- **Docker Compose** para orquestração de serviços.

---

## ✍️ Autor

| Nome      | GitHub                                                               |
|-----------|----------------------------------------------------------------------|
| Cleyton   | [@stunerx](https://github.com/StunerX) |

---

