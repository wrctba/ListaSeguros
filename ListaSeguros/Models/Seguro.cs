using ListaSeguros.Enum;
using ListaSeguros.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListaSeguros.Models
{
    public class Seguro
    {

        [Key]
        [Display(Name = "Número")]
        public int Id { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Campo obrigatório")]
        public TipoPessoa TipoPessoa { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [CpfCnpj(TipoPessoaProperty: "TipoPessoa", ErrorMessage = "CPF/CNPJ inválido")]
        [Display(Name = "CPF/CNPJ")]
        public string CpfCnpj { get; set; }

        [Required (ErrorMessage = "Campo obrigatório")]
        [Display(Name ="Tipo de Seguro")]
        public TipoSeguro TipoSeguro { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [ObjetoSegurado(TipoSeguroProperty: "TipoSeguro", ErrorMessage = "Formato inválido")]
        [MaxLength(120)]
        [Display(Name = "Objeto Segurado")]
        public string ObjetoSegurado { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Seguro seguro &&
                   Id == seguro.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    sealed public class CpfCnpjAttribute : ValidationAttribute
    {
        private readonly string _tipoPessoaProperty;

        public CpfCnpjAttribute(string TipoPessoaProperty)
        {
            if (String.IsNullOrEmpty(TipoPessoaProperty))
            {
                throw new ArgumentException("The property name must be provided");
            }
            _tipoPessoaProperty = TipoPessoaProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {

                try
                {
                    var propertyName = validationContext.ObjectType.GetProperty(_tipoPessoaProperty);
                    if (propertyName == null)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    var propertyValue = propertyName.GetValue(validationContext.ObjectInstance, null);
                    if (propertyValue == null)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }

                    if ((int)propertyValue == (int)TipoPessoa.PJ)
                    {
                        if (!CpfCnpjUtils.IsCnpj(value.ToString()))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                    else if ((int)propertyValue == (int)TipoPessoa.PF)
                    {
                        if (!CpfCnpjUtils.IsCpf(value.ToString()))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                catch
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }

    sealed public class ObjetoSeguradoAttribute : ValidationAttribute
    {
        private readonly string _tipoSeguroProperty;

        public ObjetoSeguradoAttribute(string TipoSeguroProperty)
        {
            if (String.IsNullOrEmpty(TipoSeguroProperty))
            {
                throw new ArgumentException("The property name must be provided");
            }
            _tipoSeguroProperty = TipoSeguroProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var propertyName = validationContext.ObjectType.GetProperty(_tipoSeguroProperty);
                if (propertyName == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                var propertyValue = propertyName.GetValue(validationContext.ObjectInstance, null);
                if (propertyValue == null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                
                try
                {
                    if ((int)propertyValue == (int)TipoSeguro.Automovel)
                    {
                        if (!Regex.IsMatch(value.ToString(), @"^[a-zA-Z]{3}-[0-9]{4}$"))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                    else if ((int)propertyValue == (int)TipoSeguro.Vida)
                    {
                        if (!CpfCnpjUtils.IsCpf(value.ToString()))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                catch
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}
