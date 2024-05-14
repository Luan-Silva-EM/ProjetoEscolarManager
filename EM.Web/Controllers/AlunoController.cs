using EM.Domain;
using EM.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers;

public class AlunoController : Controller
{
	private readonly IRepositorioAbstrato<Aluno> _repositorioAlunoAbstratro;
	private readonly IRepositorioAbstrato<Cidade> _repositorioCidade;
	private readonly IRepositorioAluno<Aluno> _repositorioAluno;

	public AlunoController(IRepositorioAbstrato<Aluno> repositorioAluno, IRepositorioAbstrato<Cidade> repositorioCidade, IRepositorioAluno<Aluno> repositorioRemove)
	{
		_repositorioAlunoAbstratro = repositorioAluno;
		_repositorioCidade = repositorioCidade;
		_repositorioAluno = repositorioRemove;
	}

	public IActionResult CadastreAluno(int? id)
	{
		ViewBag.Cidades = _repositorioCidade.GetAll().ToList();
		if (id != null)
		{
			Aluno? aluno = _repositorioAlunoAbstratro.Get(c => c.Matricula == id).FirstOrDefault();

			ViewBag.IsEdicao = true;
			return View(aluno);
		}
		ViewBag.IsEdicao = false;
		return View(new Aluno());

	}

	[HttpPost]
	public IActionResult CadastreAluno(Aluno aluno)
	{
		if (ModelState.IsValid)
		{
			if (aluno.Matricula > 0)
			{
				_repositorioAlunoAbstratro.Update(aluno);
			}
			else
			{
				_repositorioAlunoAbstratro.Add(aluno);
			}

			return RedirectToAction("Index", "Home");
		}
		ViewBag.IsEdicao = aluno.Matricula > 0;
		ViewBag.Cidades = _repositorioCidade.GetAll().ToList();
		return View(aluno);
	}

	[HttpPost]
	public IActionResult RemoveAluno(Aluno aluno)
	{
		_repositorioAluno.Remove(aluno);
		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	public ActionResult Pesquise(string TermoPesquisa, string TipoPesquisa)
	{
		if (TipoPesquisa != null)
		{
			if (TipoPesquisa.Equals("matricula", StringComparison.CurrentCultureIgnoreCase) && int.TryParse(TermoPesquisa, out int matricula))
			{
				Aluno aluno = _repositorioAluno.GetByMatricula(matricula);

				IEnumerable<Aluno> alunos = aluno != null ? new List<Aluno> { aluno } : [];
				return View("Views/Home/Index.cshtml", alunos);
			}
			else if (TipoPesquisa.Equals("nome", StringComparison.CurrentCultureIgnoreCase))
			{
				IEnumerable<Aluno> alunos = _repositorioAluno.GetByContendoNoNome(TermoPesquisa);
				return View("Views/Home/Index.cshtml", alunos);
			}
		}
		return View("Views/Home/Index.cshtml", new List<Aluno>());
	}
}
