using System.ComponentModel.DataAnnotations;

namespace ListaSeguros.Enum
{
    public enum TipoSeguro
    {
        [Display(Name = "Automóvel")]
        Automovel = 1,
        Residencial = 2,
        Vida = 3
    }
}
