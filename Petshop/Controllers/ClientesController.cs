using Microsoft.AspNetCore.Mvc;
using Petshop.Models;
using Petshop.Services.Interfaces;

namespace Petshop.Controllers;

public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IPetService _petService;

    public ClientesController(IClienteService clienteService, IPetService petService)
    {
        _clienteService = clienteService;
        _petService = petService;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _clienteService.GetAllAsync();
        return View(clientes);
    }

    public IActionResult Search()
    {
        return View(new ClienteSearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(ClienteSearchViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var cliente = await _clienteService.GetByCpfAsync(model.CPF);
        if (cliente is null)
        {
            ModelState.AddModelError(nameof(model.CPF), "Cliente não encontrado para o CPF informado.");
            return View(model);
        }

        return View("Details", cliente);
    }

    public IActionResult Create()
    {
        return View(new Cliente { Ativo = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        try
        {
            await _clienteService.CreateAsync(cliente);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(Cliente.CPF), ex.Message);
            return View(cliente);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        try
        {
            await _clienteService.UpdateAsync(cliente);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(Cliente.CPF), ex.Message);
            return View(cliente);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        cliente.Pets = await _petService.GetByClienteIdAsync(cliente.Id);
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _clienteService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
