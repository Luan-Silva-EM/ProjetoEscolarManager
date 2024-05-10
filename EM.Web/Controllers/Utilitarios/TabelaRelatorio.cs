using EM.Domain;
using EM.Domain.Enums;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.Web.Controllers.Utilitarios;

public class TabelaRelatorio
{
	public byte[] GerarRelatorio(List<Aluno> alunos, Sexo? sexo, string? uf, bool linhasZebradas, string? horizontal)
	{
		try
		{
			using MemoryStream ms = new();
			Document document = (horizontal == "horizontal") ? document = new(PageSize.A4.Rotate(), 25, 25, 20, 25) : document = new(PageSize.A4,25,25,20,25);
			PdfWriter writer = PdfWriter.GetInstance(document, ms);
			writer.PageEvent = new DefaultEvent();
			document.Open();


			

			if (uf != null || sexo.HasValue)
			{
				Font filterFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
				document.Add(new Paragraph($"Filtros utilizados:"));
				Paragraph filtros = new Paragraph($"Filtros utilizados:");
				if (uf != null)
				{
					alunos = alunos.Where(a => a.Cidade.UF == uf).ToList();
					Paragraph filterUf = new Paragraph($"Estado: {uf}");
					filterUf.Alignment = Element.ALIGN_LEFT;
					document.Add(filterUf);
				}
				if (sexo.HasValue)
				{
					alunos = alunos.Where(a => a.Sexo == sexo).ToList();
					Paragraph filterSexo = new Paragraph($"Sexo: {(sexo == 0 ? "Masculino" : "Feminino")}", filterFont);
					filterSexo.Alignment = Element.ALIGN_LEFT;
					document.Add(filterSexo);
				}
			}

			PdfPTable tabelaDeEstudante = CriarTabelaDeEstudante(alunos, linhasZebradas);
			tabelaDeEstudante.SpacingBefore = 15;
			tabelaDeEstudante.HeaderRows = 1;
			document.Add(tabelaDeEstudante);



			document.Close();

			return ms.ToArray();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Erro ao gerar PDF: " + ex.Message);
			Console.WriteLine("StackTrace: " + ex.StackTrace);
			throw;
		}
	}

	static PdfPTable CriarTabelaDeEstudante(List<Aluno> alunos, bool linhasZebradas)
	{
		BaseColor corFundoTitulo = new(76, 154, 109);
		BaseColor corFonteTitulo = BaseColor.WHITE;

		BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
		Font fonteConteudo = new(bf, 9, Font.NORMAL);
		Font fonteTitulo = new(bf, 12, Font.NORMAL, corFonteTitulo);

		PdfPTable tabela = new([11, 24, 8, 11, 15, 6, 15]) { WidthPercentage = 100 };

		tabela.DefaultCell.BackgroundColor = corFundoTitulo;
		tabela.DefaultCell.FixedHeight = 30;
		tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
		tabela.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

		tabela.AddCell(new Phrase("Matricula", fonteTitulo));
		tabela.AddCell(new Phrase("Nome", fonteTitulo));
		tabela.AddCell(new Phrase("sexo", fonteTitulo));
		tabela.AddCell(new Phrase("Idade", fonteTitulo));
		tabela.AddCell(new Phrase("Cidade", fonteTitulo));
		tabela.AddCell(new Phrase("UF", fonteTitulo));
		tabela.AddCell(new Phrase("CPF", fonteTitulo));

		if (linhasZebradas)
		{
			tabela.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
		}
		bool isZebrado = linhasZebradas;

		int count = 0;

		foreach (Aluno aluno in alunos)
		{

			BaseColor? backgroundColor = isZebrado ? BaseColor.LIGHT_GRAY : null;

			Phrase Matricula = new(aluno.Matricula.ToString(), fonteConteudo);
			AdicionarCelulaTabela(tabela, Matricula, backgroundColor);

			Phrase Nome = new(aluno.Nome, fonteConteudo);
			AdicionarCelulaTabela(tabela, Nome, backgroundColor, horizontalAlignment: Element.ALIGN_LEFT);

			Phrase Sexo = new(aluno.Sexo == Domain.Enums.Sexo.Masculino ? "M" : "F".ToString(), fonteConteudo);
			AdicionarCelulaTabela(tabela, Sexo, backgroundColor);

			Phrase Idade = new(CalcularIdade(aluno.DataNascimento), fonteConteudo);
			AdicionarCelulaTabela(tabela, Idade, backgroundColor);

			Phrase Cidade = new(aluno.Cidade.Nome, fonteConteudo);
			AdicionarCelulaTabela(tabela, Cidade, backgroundColor);

			Phrase UF = new(aluno.Cidade.UF, fonteConteudo);
			AdicionarCelulaTabela(tabela, UF, backgroundColor);

			Phrase CPF = new(aluno.CPF, fonteConteudo);
			AdicionarCelulaTabela(tabela, CPF, backgroundColor);

			if (isZebrado || count != 0)
			{
				isZebrado = !isZebrado;
				count++;
			}
		}
		return tabela;
	}

	public List<Aluno> AplicarFiltros(Document document, List<Aluno> alunos, int? ID_Cidade, Sexo? sexo, string ordem, string? uf)
	{
		// Criando uma cópia da lista original para evitar alterações indesejadas
		List<Aluno> alunosFiltrados = new(alunos);

		bool filtroAplicado = ID_Cidade.HasValue || sexo.HasValue || !string.IsNullOrEmpty(uf);

		// Aplica os filtros se forem fornecidos
		alunosFiltrados = alunosFiltrados
			.Where(a => !ID_Cidade.HasValue || a.Cidade.ID_Cidade == ID_Cidade)
			.Where(a => !sexo.HasValue || a.Sexo == sexo)
			.ToList();

		if (uf != null)
		{
			alunosFiltrados = alunosFiltrados.Where(a => a.Cidade.UF == uf).ToList();
		}

		// Ordena os alunos de acordo com a opção escolhida
		switch (ordem)
		{
			case "Nome":
				alunosFiltrados = alunosFiltrados.OrderBy(a => a.Nome).ToList();
				break;
			case "Cidade":
				alunosFiltrados = alunosFiltrados.OrderBy(a => a.Cidade.Nome).ToList();
				break;
			case "UF":
				alunosFiltrados = alunosFiltrados.OrderBy(a => a.Cidade.UF).ToList();
				break;
			default:
				break;
		}

		return alunosFiltrados;
	}


	static void AdicionarCelulaTabela(PdfPTable table, Phrase phrase, BaseColor backGroundColor, float fixedHeight = 20, int horizontalAlignment = Element.ALIGN_CENTER, int verticalAlignment = Element.ALIGN_MIDDLE)
	{
		PdfPCell cell = new(phrase)
		{
			FixedHeight = fixedHeight,
			HorizontalAlignment = horizontalAlignment,
			VerticalAlignment = verticalAlignment,
			BackgroundColor = backGroundColor,
		};

		table.AddCell(cell);
	}

	private static string CalcularIdade(DateTime dataNascimento)
	{
		DateTime agora = DateTime.Now;
		int anos = agora.Year - dataNascimento.Year;
		int meses = agora.Month - dataNascimento.Month;
		int dias = agora.Day - dataNascimento.Day;

		meses = dias < 0 ? meses - 1 : meses;
		anos = meses < 0 ? anos - 1 : anos;
		meses = meses < 0 ? meses + 12 : meses;
		dias = dias < 0 ? dias + DateTime.DaysInMonth(agora.Year, agora.Month) : dias;

		return $"{anos}a {meses}m {dias}d";
	}
}