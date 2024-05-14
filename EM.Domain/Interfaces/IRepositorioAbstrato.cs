using System.Linq.Expressions;

namespace EM.Domain.Interfaces;

public interface IRepositorioAbstrato<T> where T : IEntidade
{
	void Add(T objeto);
	void Update(T objeto);
	IEnumerable<T> GetAll();
	IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
}
