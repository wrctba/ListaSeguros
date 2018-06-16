using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListaSeguros.Enum
{
    public enum TipoPesquisa
    {
        [Display(Name = "Número")]
        Id = 1,
        [Display(Name = "Placa")]
        Placa = 2
    }
}
