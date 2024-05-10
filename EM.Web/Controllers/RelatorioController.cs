using EM.Domain;
using EM.Domain.Enums;
using EM.Domain.Interfaces;
using EM.Web.Controllers.Utilitarios;
using iTextSharp.text;
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

			byte[] pdfBytes = _tabelaRelatorio.GerarRelatorio(alunos, null, null, false, null);

			return File(pdfBytes, "application/pdf");
		}

		[HttpPost]
		public IActionResult GerarRelatorio(Document document, int? ID_Cidade, Sexo? sexo, string ordem, string? uf, bool linhasZebradas, string horizontal)
		{
			List<Aluno> alunos = _repositorioAluno.GetAll().ToList();

			alunos = _tabelaRelatorio.AplicarFiltros(document, alunos, ID_Cidade, sexo, ordem, uf);

			byte[] pdfBytes = _tabelaRelatorio.GerarRelatorio(alunos, sexo, uf, linhasZebradas, horizontal);

			return File(pdfBytes, "application/pdf");
		}
	}
}
