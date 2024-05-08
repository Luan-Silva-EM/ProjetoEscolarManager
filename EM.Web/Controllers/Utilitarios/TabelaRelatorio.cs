using EM.Domain;
using EM.Domain.Enums;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.Web.Controllers.Utilitarios;

public class TabelaRelatorio
{
	public byte[] GerarRelatorio(List<Aluno> alunos, int? ID_Cidade, Sexo? Sexo, string Ordem)
	{

		// Aplica os filtros se forem fornecidos
		if (ID_Cidade.HasValue)
		{
			alunos = alunos.Where(a => a.Cidade.ID_Cidade == ID_Cidade).ToList();
		}
		if (Sexo.HasValue)
		{
			alunos = alunos.Where(a => a.Sexo == Sexo).ToList();
		}

		// Ordena os alunos de acordo com a opção escolhida
		switch (Ordem)
		{
			case "Nome":
				alunos = alunos.OrderBy(a => a.Nome).ToList();
				break;
			case "Cidade":
				alunos = alunos.OrderBy(a => a.Cidade.Nome).ToList();
				break;
			case "UF":
				alunos = alunos.OrderBy(a => a.Cidade.UF).ToList();
				break;
			default:
				break;
		}
		try
		{
			using MemoryStream ms = new();
			Document document = new(PageSize.A4);
			PdfWriter writer = PdfWriter.GetInstance(document, ms);
			document.Open();

			PdfContentByte canvas = writer.DirectContentUnder;
			canvas.SaveState();
			canvas.Rectangle(0, 0, document.PageSize.Width, document.PageSize.Height);
			canvas.RestoreState();

			PdfPTable layoutTable = new([3, 7])
			{
				WidthPercentage = 100
			};

			string logoPath = "C:\\WorkLuan\\EM.Web\\EM.Web\\wwwroot\\images\\escolar_manager_logo (2).png";
			Image logo = Image.GetInstance(logoPath);
			logo.ScaleToFit(100, 100);
			PdfPCell logoCell = new(logo) { Border = Rectangle.NO_BORDER };
			layoutTable.AddCell(logoCell);

			BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			Font fonteTitulo = new(bf, 24, Font.BOLD);
			Paragraph title = new("Relatório de Alunos", fonteTitulo);
			PdfPCell titleCell = new();
			titleCell.AddElement(title);
			titleCell.Border = Rectangle.NO_BORDER;
			layoutTable.AddCell(titleCell);

			document.Add(layoutTable);

			document.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 112f, BaseColor.BLACK, Element.ALIGN_CENTER, -1)));

			document.Add(new Paragraph("\n"));

			PdfPTable tabelaDeEstudante = CriarTabelaDeEstudante(alunos);
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
	static PdfPTable CriarTabelaDeEstudante(List<Aluno> alunos)
	{
		BaseColor corFundoTitulo = new(76, 154, 109);
		BaseColor corFonteTitulo = BaseColor.WHITE;

		BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
		Font fonteConteudo = new(bf, 9, Font.NORMAL);
		Font fonteTitulo = new(bf, 12, Font.NORMAL, corFonteTitulo);

		PdfPTable tabela = new([12, 27, 14, 11, 10, 6, 18]) { WidthPercentage = 100 };

		tabela.DefaultCell.BackgroundColor = corFundoTitulo;
		tabela.DefaultCell.FixedHeight = 30;
		tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
		tabela.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

		tabela.AddCell(new Phrase("Matricula", fonteTitulo));
		tabela.AddCell(new Phrase("Nome", fonteTitulo));
		tabela.AddCell(new Phrase("Sexo", fonteTitulo));
		tabela.AddCell(new Phrase("Idade", fonteTitulo));
		tabela.AddCell(new Phrase("Cidade", fonteTitulo));
		tabela.AddCell(new Phrase("UF", fonteTitulo));
		tabela.AddCell(new Phrase("CPF", fonteTitulo));

		foreach (Aluno aluno in alunos)
		{
			Phrase Matricula = new(aluno.Matricula.ToString(), fonteConteudo);
			AdicionarCelulaTabela(tabela, Matricula);

			Phrase Nome = new(aluno.Nome, fonteConteudo);
			AdicionarCelulaTabela(tabela, Nome, horizontalAlignment: Element.ALIGN_LEFT);

			Phrase Sexo = new(aluno.Sexo.ToString(), fonteConteudo);
			AdicionarCelulaTabela(tabela, Sexo);

			Phrase Idade = new(CalcularIdade(aluno.DataNascimento), fonteConteudo);
			AdicionarCelulaTabela(tabela, Idade);

			Phrase Cidade = new(aluno.Cidade.Nome, fonteConteudo);
			AdicionarCelulaTabela(tabela, Cidade);

			Phrase UF = new(aluno.Cidade.UF, fonteConteudo);
			AdicionarCelulaTabela(tabela, UF);

			Phrase CPF = new(aluno.CPF, fonteConteudo);
			AdicionarCelulaTabela(tabela, CPF);
		}
		return tabela;
	}

	static void AdicionarCelulaTabela(PdfPTable table, Phrase phrase, float fixedHeight = 20, int horizontalAlignment = Element.ALIGN_CENTER, int verticalAlignment = Element.ALIGN_MIDDLE)
	{
		PdfPCell cell = new(phrase)
		{
			FixedHeight = fixedHeight,
			HorizontalAlignment = horizontalAlignment,
			VerticalAlignment = verticalAlignment
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
