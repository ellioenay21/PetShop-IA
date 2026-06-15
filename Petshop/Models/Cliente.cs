using System.ComponentModel.DataAnnotations;

namespace Petshop.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Cpf]
    public string CPF { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    public bool Ativo { get; set; } = true;

    public List<Pet> Pets { get; set; } = new();
}