using System.ComponentModel.DataAnnotations;
using EM.Domain.Enums;
using EM.Domain.Interfaces;
using EM.Domain.Utilitarios;

namespace EM.Domain
{
	public class Aluno : IEntidade
	{
		public int Matricula { get; set; }

		[StringLength(100, ErrorMessage = "Nome Deve ter no máximo 100 caracteres!")]
		[MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
		public string Nome { get; set; } = string.Empty;

		public Sexo Sexo { get; set; }

		[DataNascimentoValidation]
		public DateTime DataNascimento { get; set; }
		public Cidade Cidade { get; set; } = new Cidade();

		[CpfValidation]
		public string? CPF { get; set; }

		public override bool Equals(object? obj)
		{
			return obj is Aluno aluno && Matricula == aluno.Matricula && Nome == aluno.Nome && Sexo == aluno.Sexo && DataNascimento == aluno.DataNascimento && Cidade == aluno.Cidade && CPF == aluno.CPF;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string? ToString()
		{
			return base.ToString();
		}
	}
}
