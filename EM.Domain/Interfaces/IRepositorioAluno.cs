namespace EM.Domain.Interfaces;

public interface IRepositorioAluno<T> where T : IEntidade
{
	void Remove(T objeto);
	Aluno GetByMatricula(int matricula);
	IEnumerable<Aluno> GetByContendoNoNome(string parteDoNome);
}
