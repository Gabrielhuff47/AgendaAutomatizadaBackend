# Agenda Automatizada API

API REST para gerenciamento de tarefas, desenvolvida com .NET 8, FastEndpoints e MySQL.

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [XAMPP](https://www.apachefriends.org/pt_br/index.html) (para rodar o MySQL)

---

## 1. Instalar o MySQL via XAMPP

### 1.1 Baixar e instalar o XAMPP

1. Acesse [https://www.apachefriends.org/pt_br/index.html](https://www.apachefriends.org/pt_br/index.html)
2. Baixe a versão para Windows (XAMPP 8.x)
3. Execute o instalador e mantenha as opções padrão
4. Durante a instalação, selecione pelo menos o componente **MySQL** (os demais são opcionais)

### 1.2 Iniciar o MySQL

1. Abra o **XAMPP Control Panel**
2. Clique no botão **Start** ao lado de **MySQL**
3. Aguarde até que a porta **3306** apareça como ativa (verde)

> O MySQL será iniciado na porta padrão 3306 com usuário `root` e senha vazia (padrão do XAMPP).

### 1.3 Criar o banco de dados

Com o MySQL rodando, abra o terminal e execute:

```bash
# Acessar o MySQL via linha de comando
\"C:\xampp\mysql\bin\mysql.exe\" -u root
```

Dentro do MySQL, crie o banco:

```sql
CREATE DATABASE IF NOT EXISTS db_agendaautomatizada;
EXIT;
```

---

## 2. Configurar a Conexão

A string de conexão fica no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=db_agendaautomatizada;User=root;Password="
  }
}
```

> ⚠️ Se você definiu uma senha para o MySQL, substitua `Password=` por `Password=sua_senha`.

---

## 3. Aplicar as Migrations

As migrations criam e atualizam as tabelas no banco de dados automaticamente.

### 3.1 Instalar a ferramenta dotnet-ef (caso não tenha)

```bash
dotnet tool install --global dotnet-ef
```

### 3.2 Navegar até o projeto da API

```bash
cd AgendaAutomatizadaApi
```

### 3.3 Criar uma nova migration (primeira vez)

```bash
dotnet ef migrations add InitialCreate
```

### 3.4 Aplicar a migration ao banco

```bash
dotnet ef database update
```

Esse comando criará todas as tabelas necessárias no MySQL.

### 3.5 Remover a última migration (se precisar refazer)

```bash
dotnet ef migrations remove
```

---

## 4. Executar a Aplicação

### 4.1 Restaurar pacotes

```bash
dotnet restore
```

### 4.2 Executar a API

```bash
cd AgendaAutomatizadaApi
dotnet run
```

A API será iniciada em:
- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:5001`

### 4.3 Acessar o Swagger

Com a aplicação rodando, abra o navegador em:

```
http://localhost:5000/swagger
```

O Swagger permite testar todos os endpoints interativamente.

---

## 5. Endpoints da API

### Usuários

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/usuarios` | Criar um novo usuário |

### Autenticação

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/login` | Autenticar usuário (email + senha) |

### Tarefas

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/tarefas` | Criar uma nova tarefa |
| GET | `/api/tarefas` | Listar todas as tarefas |
| GET | `/api/tarefas/{id}` | Obter uma tarefa pelo ID |
| PUT | `/api/tarefas/{id}` | Atualizar uma tarefa |
| DELETE | `/api/tarefas/{idTarefa}` | Deletar uma tarefa |

---

## 6. Exemplos de Requisições

### Criar Usuário

```json
POST /api/usuarios
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "123456",
  "cpf": "12345678901",
  "telefone": "11999999999"
}
```

### Autenticar

```json
POST /api/auth/login
{
  "email": "joao@email.com",
  "senha": "123456"
}
```

### Criar Tarefa

```json
POST /api/tarefas
{
  "titulo": "Reunião de planejamento",
  "descricao": "Discutir metas do próximo trimestre",
  "data": "2026-07-10T14:00:00"
}
```

---

## 7. Testes

### Executar todos os testes (unitários + integração)

```bash
dotnet test
```

Os testes de integração utilizam um banco **MySQL real** com um banco temporário de nome único (db_agendaautomatizada_test_*) que é criado e removido automaticamente a cada execução. 

---

## 8. Estrutura do Projeto

```
AgendaAutomatizada (solução)
├── AgendaAutomatizada.Domain          # Entidades e interfaces
│   ├── Entities/
│   │   ├── TarefaEntity.cs
│   │   └── UsuarioEntity.cs
│   └── Interfaces/
│       ├── ITarefaRepository.cs
│       ├── IUsuarioRepository.cs
│       └── IPasswordHasher.cs
├── AgendaAutomatizada.Infrastructure  # Persistência (EF Core + MySQL)
│   ├── Data/
│   │   └── AgendaDbContext.cs
│   ├── Migrations/
│   └── Repositories/
│       ├── TarefaRepository.cs
│       └── UsuarioRepository.cs
├── AgendaAutomatizada.Service         # Lógica de negócio
│   ├── Interfaces/
│   │   ├── ITarefaService.cs
│   │   └── IUsuarioService.cs
│   ├── Services/
│   │   ├── TarefaService.cs
│   │   └── UsuarioService.cs
│   └── Shared/
│       ├── PasswordHasher.cs
│       └── Result.cs
├── AgendaAutomatizadaApi              # API (FastEndpoints)
│   ├── Controllers/
│   ├── DTOs/
│   ├── Mappers/
│   ├── Validators/
│   └── Program.cs
└── AgendaAutomatizada.Tests           # Testes
    ├── Integration/
    │   ├── CustomWebApplicationFactory.cs
    │   ├── TarefaIntegrationTests.cs
    │   └── UsuarioIntegrationTests.cs
    ├── TarefaServiceTests.cs
    └── UsuarioServiceTests.cs
```

---

## 9. Tecnologias Utilizadas

- **.NET 8** — Framework principal
- **FastEndpoints** — Framework de API (substituto do MVC)
- **Entity Framework Core 9** — ORM para acesso a dados
- **MySQL** (via Pomelo.EntityFrameworkCore.MySql) — Banco de dados
- **xUnit + Moq** — Testes unitários
- **Swagger / Swashbuckle** — Documentação interativa
- **BCrypt.Net** — Hash de senhas

## Licença

Projeto acadêmico — Gabriel Quintana Huf.
