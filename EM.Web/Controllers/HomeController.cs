using EM.Domain;
using EM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers;

public class HomeController : Controller
{
	private readonly RepositorioAluno _repositorioAluno = new();
	public IActionResult Index()
	{
		IEnumerable<Aluno> listaAlunos = _repositorioAluno.GetAll();
		return View(listaAlunos);
	}

}
