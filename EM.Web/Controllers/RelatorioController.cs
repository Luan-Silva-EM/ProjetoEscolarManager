using EM.Domain;
using EM.Domain.Enums;
using EM.Domain.Interfaces;
using EM.Web.Controllers.Utilitarios;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers
{
	public class RelatorioController : Controller
	{
		private readonly IRepositorioAbstrato<Cidade> _repositorioCidade;
		private readonly IRepositorioAbstrato<Aluno> _repositorioAluno;
		private readonly TabelaRelatorio _tabelaRelatorio;

		public RelatorioController(IRepositorioAbstrato<Cidade> repositorioCidade, IRepositorioAbstrato<Aluno> repositorioAluno, TabelaRelatorio tabelaRelatorio)
		{
			_repositorioCidade = repositorioCidade;
			_repositorioAluno = repositorioAluno;
			_tabelaRelatorio = tabelaRelatorio;
		}
		public IActionResult RelatorioAluno()
		{
			ViewBag.cidades = _repositorioCidade.GetAll().ToList();
			return View("/Views/Aluno/RelatorioAluno.cshtml");
		}
		public IActionResult LimparFiltros()
		{
			return RedirectToAction(nameof(RelatorioAluno));
		}

		public IActionResult GerarRelatorio()
		{
			List<Aluno> alunos = _repositorioAluno.GetAll().ToList();

			byte[] pdfBytes = _tabelaRelatorio.GerarRelatorio(alunos, null, null, null, null);

			return File(pdfBytes, "application/pdf");
		}

		[HttpPost]
		public IActionResult GerarRelatorio(int? ID_Cidade, Sexo? Sexo, string Ordem, string? Uf)
		{
			List<Aluno> alunos = _repositorioAluno.GetAll().ToList();

			byte[] pdfBytes = _tabelaRelatorio.GerarRelatorio(alunos, ID_Cidade, Sexo, Ordem, Uf);

			return File(pdfBytes, "application/pdf");
		}
	}
}
