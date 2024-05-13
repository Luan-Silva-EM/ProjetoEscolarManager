using FirebirdSql.Data.FirebirdClient;

namespace ProjetoEscola
{
	public static class BancoDeDados
    {
		private const string ConnectionString =
			@"Server=localhost; Port=3054;Database=C:\WorkLuan\EM.Web\EM.Repository\BancoDeDados\\CADASTRARALUNO.fdb;User=SYSDBA;Password=masterkey;";
		public static FbConnection CrieConexao()
		{
			FbConnection conn = new(ConnectionString);
			conn.Open();
			return conn;
		}
	}
}
