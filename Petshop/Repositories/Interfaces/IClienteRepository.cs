using Petshop.Models;

namespace Petshop.Repositories.Interfaces;

public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync();
    Task<List<Cliente>> GetActiveAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByCpfAsync(string cpf);
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Cliente cliente);
}