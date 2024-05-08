using EM.Domain.Interfaces;
using EM.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using EM.Repository;
using EM.Web.Controllers.Utilitarios;

namespace EM.Web.Controllers
{
	public class RelatorioController : Controller
	{
		private readonly IRepositorioAbstrato<Cidade> _repositorioCidade;
		private readonly IRepositorioAbstrato<Aluno> _repositorioAluno;
		private readonly TabelaRelatorio tabelaRelatorio;

		public RelatorioController(IRepositorioAbstrato<Cidade> repositorioCidade, IRepositorioAbstrato<Aluno> repositorioAluno, TabelaRelatorio tabelaRelatorio)
		{
			_repositorioCidade = repositorioCidade;
			_repositorioAluno = repositorioAluno;
			this.tabelaRelatorio = tabelaRelatorio;
		}
		public IActionResult RelatorioAluno()
		{
			ViewBag.cidades = _repositorioCidade.GetAll().ToList();
			return View("/Views/Aluno/RelatorioAluno.cshtml");
		}

		public IActionResult GerarRelatorio()
		{
			List<Aluno> alunos = _repositorioAluno.GetAll().ToList();

			byte[] pdfBytes = tabelaRelatorio.GerarRelatorio(alunos);

			return File(pdfBytes, "application/pdf");

		}
	}
}
