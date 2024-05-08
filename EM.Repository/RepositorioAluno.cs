using System.Data.Common;
using System.Linq.Expressions;
using EM.Domain;
using EM.Domain.Enums;
using EM.Domain.ExtensionMethods;
using EM.Domain.Interfaces;
using ProjetoEscola;

namespace EM.Repository
{
	public class RepositorioAluno : IRepositorioAbstrato<Aluno>, IRepositorioAluno<Aluno>
	{
		public void Add(Aluno aluno)
		{
			using DbConnection conn = BancoDeDados.GetConexao();
			using DbCommand cmd = conn.CreateCommand();

			cmd.CommandText = "INSERT INTO ALUNO(NOME, CPF, DATANASCIMENTO, SEXO, CIDADE_ID)" +
								"VALUES(@NOME, @CPF, @DATANASCIMENTO, @SEXO, @CIDADE_ID)";

			cmd.Parameters.CreateParameter("@Nome", aluno.Nome);
			cmd.Parameters.CreateParameter("@DATANASCIMENTO", aluno.DataNascimento);
			cmd.Parameters.CreateParameter("@SEXO", aluno.Sexo);
			cmd.Parameters.CreateParameter("@CPF", aluno.CPF);
			cmd.Parameters.CreateParameter("@CIDADE_ID", aluno.Cidade.ID_Cidade);

			cmd.ExecuteNonQuery();
		}

		public void Remove(Aluno aluno)
		{
			using DbConnection conn = BancoDeDados.GetConexao();
			using DbCommand cmd = conn.CreateCommand();

			cmd.CommandText = "DELETE FROM ALUNO WHERE MATRICULA = @Matricula";
			try
			{
				cmd.Parameters.CreateParameter("@Matricula", aluno.Matricula);

				cmd.ExecuteNonQuery();
				Console.WriteLine("Aluno Removido com Sucesso!");
			}
			catch (Exception erro)
			{
				Console.WriteLine($"Erro ao deletar um Aluno, detalhe do erro {erro}");
			}
		}

		public void Update(Aluno aluno)
		{
			using DbConnection conn = BancoDeDados.GetConexao();
			using DbCommand cmd = conn.CreateCommand();

			cmd.CommandText = @"
				UPDATE ALUNO 
				SET NOME = @Nome, 
				DATANASCIMENTO = @DataNascimento, 
				SEXO = @Sexo, 
				CPF = @CPF, 
				CIDADE_ID = @Cidade_ID 
				WHERE MATRICULA = @Matricula";

			cmd.Parameters.CreateParameter("@Nome", aluno.Nome);
			cmd.Parameters.CreateParameter("@DataNascimento", aluno.DataNascimento);
			cmd.Parameters.CreateParameter("@Sexo", aluno.Sexo);
			cmd.Parameters.CreateParameter("@CPF", aluno.CPF);
			cmd.Parameters.CreateParameter("@Cidade_ID", aluno.Cidade.ID_Cidade);
			cmd.Parameters.CreateParameter("@Matricula", aluno.Matricula);

			cmd.ExecuteNonQuery();
		}

		public IEnumerable<Aluno> GetAll()
		{
			using DbConnection cn = BancoDeDados.GetConexao();
			using DbCommand cmd = cn.CreateCommand();

			cmd.CommandText = @"
				SELECT A.matricula, A.nome, A.sexo, A.dataNascimento, A.CPF, C.nome AS nomeCidade, C.UF AS UFCIDADE
				FROM Aluno A
				INNER JOIN Cidades C ON A.cidade_ID = C.ID_cidade
				ORDER BY MATRICULA";

			List<Aluno> alunos = new List<Aluno>();

			DbDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Aluno aluno = new Aluno();
				aluno.Matricula = Convert.ToInt32(reader["matricula"]);
				int sexoInt = Convert.ToInt32(reader["sexo"]);
				Sexo sexo = (Sexo)sexoInt;
				aluno.Sexo = sexo;
				aluno.Nome = reader["nome"].ToString();
				aluno.CPF = reader["CPF"].ToString();
				aluno.DataNascimento = Convert.ToDateTime(reader["dataNascimento"]);

				aluno.Cidade = new Cidade();
				aluno.Cidade.Nome = reader["nomeCidade"].ToString();
				aluno.Cidade.UF = reader["UFCIDADE"].ToString();

				alunos.Add(aluno);
			}
			reader.Close();
			return alunos;
		}

		public IEnumerable<Aluno> Get(Expression<Func<Aluno, bool>> predicate) => GetAll().Where(predicate.Compile());

		public Aluno GetByMatricula(int matricula) => GetAll().First(c => c.Matricula == matricula);	

		public IEnumerable<Aluno> GetByContendoNoNome(string parteDoNome) => GetAll().Where(a => a.Nome.IndexOf(parteDoNome, StringComparison.OrdinalIgnoreCase) >= 0);
	}
}

