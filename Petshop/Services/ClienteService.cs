using Microsoft.EntityFrameworkCore;
using Petshop.Data;
using Petshop.Models;
using Petshop.Repositories.Interfaces;
using Petshop.Services.Interfaces;

namespace Petshop.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPetRepository _petRepository;

    public ClienteService(AppDbContext context, IClienteRepository clienteRepository, IPetRepository petRepository)
    {
        _context = context;
        _clienteRepository = clienteRepository;
        _petRepository = petRepository;
    }

    public Task<List<Cliente>> GetAllAsync()
    {
        return _clienteRepository.GetAllAsync();
    }

    public Task<List<Cliente>> GetActiveAsync()
    {
        return _clienteRepository.GetActiveAsync();
    }

    public Task<Cliente?> GetByIdAsync(int id)
    {
        return _clienteRepository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Cliente cliente)
    {
        cliente.CPF = new string(cliente.CPF.Where(char.IsDigit).ToArray());
        var existing = await _clienteRepository.GetByCpfAsync(cliente.CPF);
        if (existing is not null)
        {
            throw new InvalidOperationException("Já existe um cliente com este CPF.");
        }

        await _clienteRepository.AddAsync(cliente);
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        cliente.CPF = new string(cliente.CPF.Where(char.IsDigit).ToArray());
        var existing = await _clienteRepository.GetByCpfAsync(cliente.CPF);
        if (existing is not null && existing.Id != cliente.Id)
        {
            throw new InvalidOperationException("Já existe um cliente com este CPF.");
        }

        await _clienteRepository.UpdateAsync(cliente);
    }

    public async Task DeleteAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null)
            {
                return;
            }

            await _clienteRepository.DeleteAsync(cliente);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ToggleActiveAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null)
            {
                return;
            }

            cliente.Ativo = !cliente.Ativo;
            await _clienteRepository.UpdateAsync(cliente);

            if (!cliente.Ativo)
            {
                await _petRepository.DisablePetsByClienteIdAsync(cliente.Id);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}