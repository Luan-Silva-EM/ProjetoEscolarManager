using EM.Domain;
using EM.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EM.Web.Controllers
{
	public class CidadeController : Controller
    {
		private readonly IRepositorioAbstrato<Cidade> _repositorioCidade;
		public CidadeController(IRepositorioAbstrato<Cidade> repositorioCidade)
		{
			_repositorioCidade = repositorioCidade;
		}

		public IActionResult Index()
        {
			IEnumerable<Cidade> listaCidade= _repositorioCidade.GetAll();
            return View(listaCidade);
        }


		public IActionResult CadastreCidade(int? id)
		{
			if (id != null)
			{
				Cidade? cidade = _repositorioCidade.Get(c => c.ID_Cidade == id).FirstOrDefault();

				ViewBag.IsEdicao = true;
				return View(cidade);
			}
			ViewBag.IsEdicao = false;
			return View(new Cidade());
		}


		[HttpPost]
		public IActionResult CadastreCidade(Cidade cidade)
		{
			if (ModelState.IsValid)
			{
				if (cidade.ID_Cidade > 0)
				{
					_repositorioCidade.Update(cidade);
				}
				else
				{ 
					_repositorioCidade.Add(cidade);
				}
				return RedirectToAction("Index");
			}
			return View(cidade);
		}
	}
}
	