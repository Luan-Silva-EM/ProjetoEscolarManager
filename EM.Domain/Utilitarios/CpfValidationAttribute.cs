using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Utilitarios
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
			return (value == null || Validacoes.CPFValidacao(value.ToString())) ? ValidationResult.Success : new ValidationResult("CPF inválido");
		}
    }
}
