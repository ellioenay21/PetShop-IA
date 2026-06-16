using Microsoft.EntityFrameworkCore;
using Petshop.Data;
using Petshop.Models;
using Petshop.Repositories.Interfaces;

namespace Petshop.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .Include(c => c.Pets)
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<Cliente>> GetActiveAsync()
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.Pets)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> GetByCpfAsync(string cpf)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        return await _context.Clientes
            .Include(c => c.Pets)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CPF == digits);
    }

    public async Task AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        var trackedCliente = _context.Clientes.Local.FirstOrDefault(c => c.Id == cliente.Id);

        if (trackedCliente is not null)
        {
            if (!ReferenceEquals(trackedCliente, cliente))
            {
                _context.Entry(trackedCliente).CurrentValues.SetValues(cliente);
            }

            await _context.SaveChangesAsync();
            return;
        }

        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Cliente cliente)
    {
        if (cliente is null)
        {
            return;
        }

        if (cliente.Pets is not null && cliente.Pets.Any())
        {
            _context.Pets.RemoveRange(cliente.Pets);
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }
}