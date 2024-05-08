using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Utilitarios
{
	public class DataNascimentoValidationAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
		{
			return (value == null || !DateTime.TryParse(value.ToString(), out DateTime dataNascimento))
				? new ValidationResult("Data de Nascimento inválida.")
				: (dataNascimento >= DateTime.Now)
					? new ValidationResult("Data de Nascimento deve estar no passado")
					: ValidationResult.Success;
		}
	}
}
