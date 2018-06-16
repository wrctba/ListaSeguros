using ListaSeguros.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ListaSeguros.Models
{
    public class Pesquisa
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public TipoPesquisa TipoPesquisa { get; set; }

        [Required (ErrorMessage ="Campo obrigatório")]
        public string Search { get; set; }

        public string Resultado { get; set; }

    }
}
