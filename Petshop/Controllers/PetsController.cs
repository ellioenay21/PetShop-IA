using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Petshop.Models;
using Petshop.Services.Interfaces;

namespace Petshop.Controllers;

public class PetsController : Controller
{
    private readonly IPetService _petService;
    private readonly IClienteService _clienteService;

    public PetsController(IPetService petService, IClienteService clienteService)
    {
        _petService = petService;
        _clienteService = clienteService;
    }

    public async Task<IActionResult> Index()
    {
        var pets = await _petService.GetAllAsync();
        return View(pets);
    }

    public async Task<IActionResult> Create(int? clienteId = null)
    {
        await LoadClientesAsync(clienteId);
        return View(new Pet { ClienteId = clienteId ?? 0, Ativo = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pet pet)
    {
        if (!ModelState.IsValid)
        {
            await LoadClientesAsync(pet.ClienteId);
            return View(pet);
        }

        await _petService.CreateAsync(pet);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pet = await _petService.GetByIdAsync(id);
        if (pet is null)
        {
            return NotFound();
        }

        await LoadClientesAsync(pet.ClienteId);
        return View(pet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pet pet)
    {
        if (id != pet.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadClientesAsync(pet.ClienteId);
            return View(pet);
        }

        await _petService.UpdateAsync(pet);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _petService.GetByIdAsync(id);
        if (pet is null)
        {
            return NotFound();
        }

        return View(pet);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _petService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadClientesAsync(int? selectedClienteId = null)
    {
        var clientes = await _clienteService.GetActiveAsync();
        ViewBag.Clientes = new SelectList(clientes, nameof(Cliente.Id), nameof(Cliente.Nome), selectedClienteId);
    }
}