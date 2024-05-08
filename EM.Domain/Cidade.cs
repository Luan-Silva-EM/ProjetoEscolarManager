
using EM.Domain.Interfaces;

namespace EM.Domain
{
	public class Cidade : IEntidade
	{
		public int ID_Cidade {  get; set; }
		public string? Nome { get; set; }
		public string? UF { get; set; }
	}
}
