# PetShop IA

Projeto ASP.NET Core MVC para gerenciar clientes e pets com busca por CPF, cadastro e visualização de relacionamentos.

## Visão geral

O `PetShop` é uma aplicação web construída com ASP.NET Core 10 usando o padrão MVC. Ele mantém dados de clientes e pets, valida CPF, organiza relacionamentos entre clientes e seus pets, e fornece uma interface profissional com menu de navegação, cadastro, lista e busca.

## Estrutura geral

- `Petshop/Program.cs` — ponto de entrada do app e configuração de serviços
- `Petshop/Data/AppDbContext.cs` — definição do DbContext e modelos EF Core
- `Petshop/Models` — classes de domínio (`Cliente`, `Pet`, `ClienteSearchViewModel`, `CpfAttribute`)
- `Petshop/Repositories` — abstração de acesso a dados
- `Petshop/Services` — regras de negócio e transações
- `Petshop/Controllers` — manipuladores das rotas MVC
- `Petshop/Views` — interfaces Razor e layouts
- `Petshop/wwwroot` — arquivos estáticos, estilos e imagens

## Como executar

1. Abra o terminal na pasta do repositório.
2. Entre no projeto:

```powershell
cd "C:\Users\ellion\Documents\Petshop\Petshop"
```

3. Compile:

```powershell
dotnet build
```

4. Execute:

```powershell
dotnet run
```

5. Abra no navegador:

```text
http://localhost:5148
```

## Principais componentes

### Program.cs

Este arquivo usa o modelo de top-level statements do C# e configura:

- `AddControllersWithViews()` para habilitar MVC com views Razor.
- `AddDbContext<AppDbContext>()` para registrar o contexto do EF Core com SQLite.
- `AddScoped(...)` para injetar repositórios e serviços via DI.
- `EnsureCreated()` para criar o banco de dados automaticamente em tempo de execução.
- `UseHttpsRedirection()`, `UseRouting()` e `UseAuthorization()` para configurar o pipeline HTTP.
- `MapControllerRoute(...)` para rotas padrão MVC.

### AppDbContext.cs

Define duas `DbSet`s:

- `DbSet<Cliente> Clientes`
- `DbSet<Pet> Pets`

Configura a modelagem do banco com `OnModelCreating`:

- `Cliente` e `Pet` são mapeados para tabelas dedicadas.
- `Cliente.CPF` é único (`HasIndex(c => c.CPF).IsUnique()`).
- Relacionamento um-para-muitos entre `Cliente` e `Pet` com `HasForeignKey(p => p.ClienteId)`.
- Restrições de tamanhos máximos e obrigatoriedade.

### Models

#### Cliente.cs

Representa o cliente.

- `Id` — chave primária.
- `Nome` — obrigatório, máximo 100 caracteres.
- `CPF` — obrigatório, validado por `CpfAttribute` e máximo 11 caracteres.
- `Email`, `Telefone` — campos opcionais com tamanho máximo.
- `Ativo` — indica cliente ativo/desativado.
- `Pets` — lista de pets vinculados.

#### Pet.cs

Representa o pet.

- `ClienteId` — chave estrangeira obrigatória.
- `Cliente` — propriedade de navegação.
- `Nome` e `Especie` — obrigatórios.
- `Raca`, `Observacoes` — opcionais.
- `Idade` — valor entre 0 e 30.
- `Ativo` — status do pet.

#### ClienteSearchViewModel.cs

Usado para pesquisa de clientes.

- `Nome` — campo opcional.
- `CPF` — obrigatório.
- `ClienteResultado` — pode armazenar o resultado encontrado.

### Repositories

O padrão Repository separa o acesso a dados da lógica de negócio.

#### IClienteRepository.cs

Expõe métodos como:

- `GetAllAsync()`
- `GetActiveAsync()`
- `GetByIdAsync(int id)`
- `GetByCpfAsync(string cpf)`
- `AddAsync(Cliente cliente)`
- `UpdateAsync(Cliente cliente)`
- `DeleteAsync(Cliente cliente)`

#### ClienteRepository.cs

Implementa métodos que usam EF Core:

- `Include(c => c.Pets)` carrega a relação cliente-pets.
- `AsNoTracking()` melhora desempenho em consultas somente leitura.
- `GetByCpfAsync` normaliza o CPF removendo caracteres que não são dígitos.
- `UpdateAsync` reutiliza a entidade já rastreada para evitar conflitos de estado.
- `DeleteAsync` remove pets relacionados antes de excluir o cliente.

#### IPetRepository.cs / PetRepository.cs

Expõe e implementa métodos para pets:

- `GetAllAsync()` carrega pets com cliente associado.
- `GetByClienteIdAsync(int clienteId)` retorna pets de um cliente.
- `GetByIdAsync(int id)` carrega um pet por ID.
- `AddAsync`, `UpdateAsync`, `DeleteAsync` para operações CRUD.
- `DisablePetsByClienteIdAsync(int clienteId)` desativa pets de um cliente quando o cliente é desativado.

### Services

Camada de negócio que utiliza repositórios.

#### IClienteService.cs / ClienteService.cs

- `GetByCpfAsync(string cpf)` faz a busca por CPF.
- `CreateAsync(Cliente cliente)` normaliza o CPF e garante unicidade.
- `UpdateAsync(Cliente cliente)` faz checagem de CPF único ao editar.
- `DeleteAsync(int id)` e `ToggleActiveAsync(int id)` usam transações EF Core (`BeginTransactionAsync`) para garantir integridade.
- `ToggleActiveAsync` também desativa pets associados quando um cliente é desativado.

#### IPetService.cs / PetService.cs

Serviço simples para delegar operações do repositório.

- `GetAllAsync()`
- `GetByIdAsync(int id)`
- `GetByClienteIdAsync(int clienteId)`
- `CreateAsync(Pet pet)`
- `UpdateAsync(Pet pet)`
- `DeleteAsync(int id)`

### Controllers

Os controllers processam requisições HTTP e retornam views.

#### ClientesController.cs

- `Index()` lista clientes.
- `Search()` exibe o formulário de busca.
- `Search(ClienteSearchViewModel)` valida o CPF e retorna `Details` se encontrar o cliente.
- `Create()` e `Create(Cliente)` fazem cadastro de cliente.
- `Edit()` e `Edit(int, Cliente)` atualizam dados.
- `Details(int)` mostra cliente e pets vinculados.
- `Delete(int)` exclui cliente.

#### PetsController.cs

- `Index()` lista pets.
- `Create()` e `Create(Pet)` permitem cadastrar pet e selecionam clientes ativos.
- `Edit()` e `Edit(int, Pet)` atualizam pets.
- `Delete()` e `DeleteConfirmed(int)` removem pets.

### Views

#### `Views/Shared/_Layout.cshtml`

Define o layout comum com Bootstrap:

- Menu principal com `Home`, `Cadastro`, `Lista` e `Busca`.
- `@RenderBody()` para inserir o conteúdo da view.
- Arquivos estáticos: Bootstrap, jQuery e CSS.

#### `Views/Home/Index.cshtml`

Página inicial com banner, CTA e imagem ilustrativa.

#### `Views/Clientes/Search.cshtml`

Tela de busca com CPF obrigatório e nome opcional.

#### `Views/Clientes/Details.cshtml`

Exibe cliente e pets vinculados.

## Tecnologias e sintaxe utilizadas

- ASP.NET Core MVC
- C# 12 / .NET 10
- Entity Framework Core com SQLite
- Razor Pages / Razor syntax
- Dependência injeção via `builder.Services.AddScoped`
- Assíncrono com `async` / `await`
- Validação de modelo com atributos:
  - `[Required]`
  - `[StringLength]`
  - `[Range]`
  - `[Display]`
  - `[Cpf]` (customizado)
- EF Core query syntax:
  - `Include(...)`
  - `Where(...)`
  - `OrderBy(...)`
  - `FirstOrDefaultAsync(...)`
  - `AsNoTracking()`
- Razor tag helpers:
  - `asp-controller`
  - `asp-action`
  - `asp-route-id`
  - `asp-for`

## O que está presente

- Cadastro de clientes com CPF único.
- Cadastro de pets associados a clientes.
- Listagem de clientes e pets.
- Busca por CPF exibindo cliente e pets.
- Menu de navegação com dropdowns.
- Validação de CPF no backend.
- Banco SQLite criado automaticamente.

## Possíveis otimizações

1. **Paginação para Listas Grandes**
   - Para muitos clientes ou pets, usar paginação em `GetAllAsync()` em vez de carregar tudo na memória.

2. **Buscar por CPF parcial / nome**
   - Atualmente a busca exige CPF exato. Poderia aceitar CPF com formatação e permitir pesquisa parcial por nome.

3. **Melhorar `UpdateAsync` do repositório**
   - A lógica de `Local.FirstOrDefault` funciona, mas pode ser simplificada usando `Attach` ou `Entry(entity).State = EntityState.Modified`.

4. **Cascade delete ou soft delete**
   - Em vez de remover pets manualmente ao excluir cliente, considerar cascade delete ou `IsDeleted`.

5. **Separar validação do CPF**
   - A validação atual está em um atributo `CpfAttribute`; para maior controle, pode-se validar também no serviço ou usar uma biblioteca externa.

6. **Camada de DTO/ViewModel**
   - O projeto mistura entidades de domínio com views em alguns locais. Usar DTOs ou ViewModels reduz exposição de dados desnecessários.

7. **Cache / Performance**
   - Consultas mais frequentes, como `GetActiveAsync`, podem se beneficiar de cache em memória quando os dados não mudam frequentemente.

8. **Testes automatizados**
   - Adicionar testes de unidade para serviços e repositórios fortalece a manutenção.

9. **Logging e tratamento de erros**
   - Capturar exceções com logging estruturado e páginas de erro personalizadas.

10. **Interface amigável**
   - Adicionar feedback visual em operações de cadastro e busca usando toasts ou alertas.

## Observações finais

- O projeto usa um padrão clássico de camadas `Controllers -> Services -> Repositories -> Data`.
- A aplicação já está pronta para evolução: novos campos, relatórios e autenticação.
- A arquitetura atual é simples e adequada para MVP, mas pode crescer com mais abstrações e testes.
