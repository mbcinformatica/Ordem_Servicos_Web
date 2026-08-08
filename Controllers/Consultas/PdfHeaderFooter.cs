// Controllers/Relatorios/PdfHeaderFooter.cs
using iText.Commons.Actions;
using iText.Kernel.Pdf.Event;

namespace Ordem_Servicos_Web.Controllers.Consultas
{
    public class PdfHeaderFooter(String title) : AbstractPdfDocumentEventHandler
    {
        private readonly string _title = title;

        // Implementação exigida pela base abstrata
        public override void OnEvent(IEvent @event)
        {
            // encaminha para seu manipulador específico, se já existir
            HandleEvent((PdfDocumentEvent)@event);
        }

        public void HandleEvent(PdfDocumentEvent docEvent)
        {
            ArgumentNullException.ThrowIfNull(docEvent);
            // mantenha aqui a implementação existente do header/footer
        }

        protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}