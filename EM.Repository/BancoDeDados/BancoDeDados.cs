using FirebirdSql.Data.FirebirdClient;

namespace ProjetoEscola
{
	public static class BancoDeDados
    {
		private const string ConnectionString =
			@"Server=localhost; Port=3054;Database=C:\WorkLuan\EM.Web\EM.Repository\BancoDeDados\\CADASTRARALUNO.fdb;User=SYSDBA;Password=masterkey;";
		private static FbConnection? conn = null;
		public static FbConnection GetConexao()
		{
			if (conn == null || conn.State != System.Data.ConnectionState.Open)
			{
				FbConnection.ClearAllPools();
				conn = new FbConnection(ConnectionString);
				conn.Open();
			}
			return conn;
		}
	}
}
