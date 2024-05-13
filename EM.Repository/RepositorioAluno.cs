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
			using DbConnection conn = BancoDeDados.CrieConexao();
			using DbCommand cmd = conn.CreateCommand();

			cmd.CommandText = "INSERT INTO ALUNO(NOME, CPF, DATANASCIMENTO, SEXO, CIDADE_ID)" +
								"VALUES(@NOME, @CPF, @DATANASCIMENTO, @SEXO, @CIDADE_ID)";

			cmd.Parameters.CreateParameter("@Nome", aluno.Nome);
			cmd.Parameters.CreateParameter("@DATANASCIMENTO", aluno.DataNascimento);
			cmd.Parameters.CreateParameter("@SEXO", aluno.Sexo);
			cmd.Parameters.CreateParameter("@CPF", aluno.CPF != null ? aluno.CPF : DBNull.Value);
			cmd.Parameters.CreateParameter("@CIDADE_ID", aluno.Cidade.ID_Cidade);

			cmd.ExecuteNonQuery();
		}

		public void Remove(Aluno aluno)
		{
			using DbConnection conn = BancoDeDados.CrieConexao();
			using DbCommand cmd = conn.CreateCommand();

			cmd.CommandText = "DELETE FROM ALUNO WHERE MATRICULA = @Matricula";

			cmd.Parameters.CreateParameter("@Matricula", aluno.Matricula);
			cmd.ExecuteNonQuery();
			Console.WriteLine("Aluno Removido com Sucesso!");
		}

		public void Update(Aluno aluno)
		{
			using DbConnection conn = BancoDeDados.CrieConexao();
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
			cmd.Parameters.CreateParameter("@CPF", aluno.CPF != null ? aluno.CPF : DBNull.Value);
			cmd.Parameters.CreateParameter("@Cidade_ID", aluno.Cidade.ID_Cidade);
			cmd.Parameters.CreateParameter("@Matricula", aluno.Matricula);

			cmd.ExecuteNonQuery();
		}

		public IEnumerable<Aluno> GetAll()
		{
			List<Aluno> alunos = [];

			using DbConnection cn = BancoDeDados.CrieConexao();
			using DbCommand cmd = cn.CreateCommand();

			cmd.CommandText = @"
				SELECT A.matricula, A.nome, A.sexo, A.dataNascimento, A.CPF, C.nome AS nomeCidade, C.UF AS UFCIDADE, C.ID_CIDADE AS IDCIDADE
				FROM Aluno A
				INNER JOIN Cidades C ON A.cidade_ID = C.ID_cidade
				ORDER BY MATRICULA";


			DbDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{

				alunos.Add(new Aluno
				{
					Matricula = Convert.ToInt32(reader["matricula"]),
					Sexo = (Sexo)Convert.ToInt32(reader["sexo"]),
					Nome = reader["nome"].ToString()!,
					CPF = reader["CPF"].ToString(),
					DataNascimento = Convert.ToDateTime(reader["dataNascimento"]),
					Cidade = new Cidade
					{
						Nome = reader["nomeCidade"].ToString(),
						UF = reader["UFCIDADE"].ToString(),
						ID_Cidade = Convert.ToInt32(reader["IDCIDADE"])

					}
				});
			}
			reader.Close();
			return alunos;
		}

		public IEnumerable<Aluno> Get(Expression<Func<Aluno, bool>> predicate) => GetAll().Where(predicate.Compile());

		public Aluno GetByMatricula(int matricula) => GetAll().First(c => c.Matricula == matricula);

		public IEnumerable<Aluno> GetByContendoNoNome(string parteDoNome) => GetAll().Where(a => a.Nome.Contains(parteDoNome, StringComparison.OrdinalIgnoreCase));
	}
}
