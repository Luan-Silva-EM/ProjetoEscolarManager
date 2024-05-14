using System.ComponentModel.DataAnnotations;
using EM.Domain.Enums;
using EM.Domain.Interfaces;
using EM.Domain.Utilitarios;

namespace EM.Domain;

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
		// Verifica se o objeto passado é nulo ou se não é uma instância de Aluno
		if (obj == null || !(obj is Aluno))
		{
			return false;
		}

		// Compara os atributos relevantes para determinar se são iguais
		Aluno outroAluno = (Aluno)obj;
		return Matricula == outroAluno.Matricula &&
			   Nome == outroAluno.Nome &&
			   Sexo == outroAluno.Sexo &&
			   DataNascimento == outroAluno.DataNascimento &&
			   Cidade.Equals(outroAluno.Cidade) && // Supondo que Cidade implementa corretamente o método Equals
			   CPF == outroAluno.CPF;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Matricula, Nome, Sexo, DataNascimento, Cidade, CPF);
	}


	public override string? ToString()
	{
		return $"Matrícula: {Matricula}, Nome: {Nome}, Sexo: {Sexo}, Data de Nascimento: {DataNascimento}, Cidade: {Cidade}, CPF: {CPF}";
	}
}
