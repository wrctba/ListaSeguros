using System.ComponentModel.DataAnnotations;

namespace ListaSeguros.Enum
{
    public enum TipoPessoa
    {
        [Display(Name = "Pessoa Física")]
        PF = 1,
        [Display(Name = "Pessoa Jurídica")]
        PJ = 2
    }
}
