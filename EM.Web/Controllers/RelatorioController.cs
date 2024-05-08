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

			byte[] pdfBytes = tabelaRelatorio.GerarRelatorio(alunos, null, null, null);

			return File(pdfBytes, "application/pdf");
		}

		[HttpPost]
		public IActionResult GerarRelatorio(int? ID_Cidade, Sexo? Sexo, string Ordem)
		{
			List<Aluno> alunos = _repositorioAluno.GetAll().ToList();

			byte[] pdfBytes = tabelaRelatorio.GerarRelatorio(alunos, ID_Cidade, Sexo, Ordem);

			return File(pdfBytes, "application/pdf");
		}
	}
}
