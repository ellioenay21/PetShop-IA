using Petshop.Models;

namespace Petshop.Services.Interfaces;

public interface IPetService
{
    Task<List<Pet>> GetAllAsync();
    Task<Pet?> GetByIdAsync(int id);
    Task<List<Pet>> GetByClienteIdAsync(int clienteId);
    Task CreateAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(int id);
}