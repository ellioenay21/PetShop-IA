using Petshop.Models;

namespace Petshop.Services.Interfaces;

public interface IClienteService
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<List<Cliente>> GetActiveAsync();
    Task CreateAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task ToggleActiveAsync(int id);
}