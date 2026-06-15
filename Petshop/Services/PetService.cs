using Petshop.Models;
using Petshop.Repositories.Interfaces;
using Petshop.Services.Interfaces;

namespace Petshop.Services;

public class PetService : IPetService
{
    private readonly IPetRepository _repository;

    public PetService(IPetRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Pet>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Pet?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<List<Pet>> GetByClienteIdAsync(int clienteId)
    {
        return _repository.GetByClienteIdAsync(clienteId);
    }

    public Task CreateAsync(Pet pet)
    {
        return _repository.AddAsync(pet);
    }

    public Task UpdateAsync(Pet pet)
    {
        return _repository.UpdateAsync(pet);
    }

    public async Task DeleteAsync(int id)
    {
        var pet = await _repository.GetByIdAsync(id);
        if (pet is not null)
        {
            await _repository.DeleteAsync(pet);
        }
    }
}