﻿using EM.Domain;
using EM.Domain.Enums;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.Web.Controllers.Utilitarios;

public class TabelaRelatorio
{
	public byte[] GereRelatorio(List<Aluno> alunos, Sexo? sexo, string? uf, bool linhasZebradas, string? horizontal)
	{
		try
		{
			using MemoryStream ms = new();
			Document document = (horizontal == "horizontal") ? document = new(PageSize.A4.Rotate(), 25, 25, 20, 25) : document = new(PageSize.A4,25,25,20,25);
			PdfWriter writer = PdfWriter.GetInstance(document, ms);
			writer.PageEvent = new DefaultEvent();
			document.Open();

			Font filterFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
			document.Add(new Paragraph("Filtros utilizados:"));

			alunos = uf != null ? alunos.Where(a => a.Cidade.UF == uf).ToList() : alunos;
			alunos = sexo.HasValue ? alunos.Where(a => a.Sexo == sexo).ToList() : alunos;

			if (uf != null)
				document.Add(new Paragraph($"Estado: {uf}") { Alignment = Element.ALIGN_LEFT });

			if (sexo.HasValue)
			{
				string sexoTexto = sexo == 0 ? "Masculino" : "Feminino";
				document.Add(new Paragraph($"Sexo: {sexoTexto}", filterFont) { Alignment = Element.ALIGN_LEFT });
			}

			PdfPTable tabelaDeEstudante = CrieTabelaDeEstudante(alunos, linhasZebradas);
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

	static PdfPTable CrieTabelaDeEstudante(List<Aluno> alunos, bool linhasZebradas)
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

		tabela.DefaultCell.BackgroundColor = linhasZebradas ? BaseColor.LIGHT_GRAY : tabela.DefaultCell.BackgroundColor;
		bool isZebrado = linhasZebradas;

		int count = 0;

		foreach (Aluno aluno in alunos)
		{

			BaseColor? backgroundColor = isZebrado ? BaseColor.LIGHT_GRAY : null;

			Phrase Matricula = new(aluno.Matricula.ToString(), fonteConteudo);
			AdicioneCelulaTabela(tabela, Matricula, backgroundColor);

			Phrase Nome = new(aluno.Nome, fonteConteudo);
			AdicioneCelulaTabela(tabela, Nome, backgroundColor, horizontalAlignment: Element.ALIGN_LEFT);

			Phrase Sexo = new(aluno.Sexo == Domain.Enums.Sexo.Masculino ? "M" : "F".ToString(), fonteConteudo);
			AdicioneCelulaTabela(tabela, Sexo, backgroundColor);

			Phrase Idade = new(CalculeIdade(aluno.DataNascimento), fonteConteudo);
			AdicioneCelulaTabela(tabela, Idade, backgroundColor);

			Phrase Cidade = new(aluno.Cidade.Nome, fonteConteudo);
			AdicioneCelulaTabela(tabela, Cidade, backgroundColor);

			Phrase UF = new(aluno.Cidade.UF, fonteConteudo);
			AdicioneCelulaTabela(tabela, UF, backgroundColor);

			Phrase CPF = new(aluno.CPF, fonteConteudo);
			AdicioneCelulaTabela(tabela, CPF, backgroundColor);

			if (isZebrado || count != 0)
			{
				isZebrado = !isZebrado;
				count++;
			}
		}
		return tabela;
	}

	public List<Aluno> ApliqueFiltros(List<Aluno> alunos, int? ID_Cidade, Sexo? sexo, string ordem, string? uf)
	{
		// Criando uma cópia da lista original para evitar alterações indesejadas
		List<Aluno> alunosFiltrados = new(alunos);

		bool filtroAplicado = ID_Cidade.HasValue || sexo.HasValue || !string.IsNullOrEmpty(uf);

		// Aplica os filtros se forem fornecidos
		alunosFiltrados = alunosFiltrados
			.Where(a => !ID_Cidade.HasValue || a.Cidade.ID_Cidade == ID_Cidade)
			.Where(a => !sexo.HasValue || a.Sexo == sexo)
			.ToList();

		alunosFiltrados = uf != null ? alunosFiltrados.Where(a => a.Cidade.UF == uf).ToList() : alunosFiltrados;

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

	static void AdicioneCelulaTabela(PdfPTable table, Phrase phrase, BaseColor backGroundColor, float fixedHeight = 20, int horizontalAlignment = Element.ALIGN_CENTER, int verticalAlignment = Element.ALIGN_MIDDLE)
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

	private static string CalculeIdade(DateTime dataNascimento)
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