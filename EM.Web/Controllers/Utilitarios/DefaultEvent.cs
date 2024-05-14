using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.Web.Controllers.Utilitarios;

public class DefaultEvent : PdfPageEventHelper
{
	public override void OnStartPage(PdfWriter writer, Document document)
	{
		base.OnStartPage(writer, document);

		PdfPTable layoutTable = new([3, 7, 3])
		{
			WidthPercentage = 100
		};

		string logoPath = "C:\\WorkLuan\\EM.Web\\EM.Web\\wwwroot\\images\\escolar_manager_logo (2).png";
		Image logo = Image.GetInstance(logoPath);
		logo.ScaleToFit(100, 100);
		PdfPCell logoCell = new(logo)
		{
			Border = Rectangle.NO_BORDER,
			HorizontalAlignment = Element.ALIGN_CENTER,
			VerticalAlignment = Element.ALIGN_MIDDLE
		};
		layoutTable.AddCell(logoCell);

		BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
		Font fonteTitulo = new(bf, 24, Font.BOLD);

		PdfPCell titleCell = new(new Phrase("Relatório de Alunos", fonteTitulo))
		{
			Border = Rectangle.NO_BORDER,
			HorizontalAlignment = Element.ALIGN_CENTER,
			VerticalAlignment = Element.ALIGN_MIDDLE
		};
		layoutTable.AddCell(titleCell);

		PdfPCell invisibleCell = new()
		{
			Border = Rectangle.NO_BORDER

		};
		layoutTable.AddCell(invisibleCell);

		layoutTable.SpacingAfter = 20;
		document.Add(layoutTable);
	}

	public override void OnEndPage(PdfWriter writer, Document document)
	{
		base.OnEndPage(writer, document);

		BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
		Font fontRodape = new(bf);

		PdfPTable footer = new PdfPTable(2)
		{
			TotalWidth = document.PageSize.Width
		};
		footer.DefaultCell.Border = Rectangle.NO_BORDER;


		PdfPCell dateCell = new(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontRodape))
		{
			HorizontalAlignment = Element.ALIGN_LEFT,
			PaddingLeft = 24,
			VerticalAlignment = Element.ALIGN_BOTTOM,
			Border = Rectangle.NO_BORDER
		};
		footer.AddCell(dateCell);

		PdfPCell pageNumberCell = new PdfPCell(new Phrase(writer.PageNumber.ToString(), fontRodape))
		{
			HorizontalAlignment = Element.ALIGN_RIGHT,
			PaddingRight = 24,
			VerticalAlignment = Element.ALIGN_BOTTOM,
			Border = Rectangle.NO_BORDER
		};
		footer.AddCell(pageNumberCell);

		float footerPosition = document.BottomMargin;
		footer.WriteSelectedRows(0, -1, 0, footerPosition, writer.DirectContent);

	}
}
