using System.ComponentModel.DataAnnotations;

namespace Petshop.Models;

public class Pet
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente.")]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Especie { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Raca { get; set; }

    [Range(0, 30)]
    public int Idade { get; set; }

    public bool Ativo { get; set; } = true;

    [StringLength(500)]
    public string? Observacoes { get; set; }
}