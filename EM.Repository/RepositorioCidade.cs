using EM.Domain;
using EM.Domain.ExtensionMethods;
using EM.Domain.Interfaces;
using ProjetoEscola;
using System.Data.Common;
using System.Linq.Expressions;

namespace EM.Repository
{
    public class RepositorioCidade : IRepositorioAbstrato<Cidade>
    {
        public void Add(Cidade cidade)
        {
            using DbConnection conn = BancoDeDados.GetConexao();
            using DbCommand cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO CIDADES(NOME, UF)" +
                                "VALUES(@NOME, @UF)";

            cmd.Parameters.CreateParameter("@Nome", cidade.Nome);
            cmd.Parameters.CreateParameter("@UF", cidade.UF);

            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Cidade> Get(Expression<Func<Cidade, bool>> predicate)
        {
            return GetAll().Where(predicate.Compile());
        }

        public IEnumerable<Cidade> GetAll()
        {
            using DbConnection cn = BancoDeDados.GetConexao();
            using DbCommand cmd = cn.CreateCommand();

            cmd.CommandText = @"
				SELECT C.id_cidade, c.nome, C.UF
				FROM CIDADES C";

            List<Cidade> cidades = new List<Cidade>();

            DbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cidade cidade = new Cidade();
                cidade.ID_Cidade = int.Parse(reader["id_cidade"].ToString());
                cidade.Nome = reader["nome"].ToString();
                cidade.UF = reader["UF"].ToString();

                cidades.Add(cidade);
            }
            reader.Close();
            return cidades;
        }

		public void Remove(Cidade objeto)
		{
			throw new NotImplementedException();
		}

		public void Update(Cidade cidade)
        {
            using DbConnection conn = BancoDeDados.GetConexao();
            using DbCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE CIDADES SET NOME = @Nome, UF = @UF " +
                                "WHERE ID_CIDADE = @ID";

            cmd.Parameters.CreateParameter("@Nome", cidade.Nome);
            cmd.Parameters.CreateParameter("@UF", cidade.UF);
            cmd.Parameters.CreateParameter("@ID", cidade.ID_Cidade);

            cmd.ExecuteNonQuery();
        }
    }
}

