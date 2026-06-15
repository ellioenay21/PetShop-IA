# PetShop IA

Projeto ASP.NET Core MVC para gerenciar clientes e pets.
Contém camadas de Repository e Service, validação de CPF e persistência com SQLite.

Repositório GitHub:

https://github.com/ellioenay21/PetShop-IA

## Pré-requisitos

- .NET 10 SDK instalado
- Git instalado (opcional para clonar o repositório)

## Como executar localmente

1. Abra o terminal na pasta do repositório.
2. Execute:

```powershell
cd "C:\Users\ellion\Documents\Petshop"
```

3. Construa a solução:

```powershell
dotnet build
```

4. Rode o projeto:

```powershell
dotnet run --project Petshop/Petshop.csproj
```

5. Abra o navegador em:

```text
http://localhost:5148
```

## Estrutura do projeto

- `Petshop/Models` — modelos de dados e validações
- `Petshop/Data` — `AppDbContext` com configuração do SQLite
- `Petshop/Repositories` — padrão Repository para acesso a dados
- `Petshop/Services` — lógica de negócio e transações
- `Petshop/Controllers` — controllers MVC para clientes e pets
- `Petshop/Views` — views Razor para as telas do sistema
- `Petshop/wwwroot` — arquivos estáticos

## Observações

- O banco de dados local usa SQLite e é criado automaticamente.
- Para alterações futuras, crie um novo commit e faça push para o repositório remoto.
