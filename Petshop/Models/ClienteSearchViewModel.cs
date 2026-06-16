using System.ComponentModel.DataAnnotations;

namespace Petshop.Models;

public class ClienteSearchViewModel
{
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "CPF")]
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    public string CPF { get; set; } = string.Empty;

    public Cliente? ClienteResultado { get; set; }
}
