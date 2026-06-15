using Microsoft.EntityFrameworkCore;
using Petshop.Data;
using Petshop.Models;
using Petshop.Repositories.Interfaces;

namespace Petshop.Repositories;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext _context;

    public PetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pet>> GetAllAsync()
    {
        return await _context.Pets
            .Include(p => p.Cliente)
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<List<Pet>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Pets
            .AsNoTracking()
            .Where(p => p.ClienteId == clienteId)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Pet?> GetByIdAsync(int id)
    {
        return await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Pet pet)
    {
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pet pet)
    {
        _context.Pets.Update(pet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Pet pet)
    {
        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();
    }

    public async Task DisablePetsByClienteIdAsync(int clienteId)
    {
        var pets = await _context.Pets.Where(p => p.ClienteId == clienteId).ToListAsync();
        foreach (var pet in pets)
        {
            pet.Ativo = false;
        }

        await _context.SaveChangesAsync();
    }
}