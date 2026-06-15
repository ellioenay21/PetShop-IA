using Petshop.Models;

namespace Petshop.Repositories.Interfaces;

public interface IPetRepository
{
    Task<List<Pet>> GetAllAsync();
    Task<List<Pet>> GetByClienteIdAsync(int clienteId);
    Task<Pet?> GetByIdAsync(int id);
    Task AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(Pet pet);
    Task DisablePetsByClienteIdAsync(int clienteId);
}