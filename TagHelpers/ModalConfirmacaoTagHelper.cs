using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ordem_Servicos_Web.TagHelpers
{
    [HtmlTargetElement("modal-confirmacao")]
    public class ModalConfirmacaoTagHelper : TagHelper
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        [ViewContext]
        public ViewContext ViewContext { get; set; } = default!;

        public ModalConfirmacaoTagHelper(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelperFactory urlHelperFactory)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public required string IdModal { get; set; }
        public required string Icone { get; set; }
        public required string Titulo { get; set; }
        public required string Mensagem { get; set; }
        public required string ClasseMensagem { get; set; }
        public required string ActionController { get; set; }
        public required string ActionName { get; set; }
        public string? IdRegistro { get; set; }
        public required string IconeBotaoPrincipal { get; set; }
        public required string TextoBotaoPrincipal { get; set; }
        public required string ClasseBotaoPrincipal { get; set; }
        public required string TextoBotaoCancelar { get; set; }
        public required string ClasseBotaoSecundario { get; set; }
        public required string ActionControllerSecundario { get; set; }
        public required string ActionNameSecundario { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "modal fade");
            output.Attributes.SetAttribute("id", IdModal);
            output.Attributes.SetAttribute("tabindex", "-1");
            output.Attributes.SetAttribute("aria-hidden", "true");

            // Token antiforgery
            var tokens = _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext);
            var antiForgeryInput = $"<input name='{tokens.FormFieldName}' type='hidden' value='{tokens.RequestToken}' />";

            // URL do botão Voltar
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var voltarUrl = urlHelper.Action(ActionNameSecundario, ActionControllerSecundario);

            var html = $@"
<div class='modal-dialog modal-dialog-centered'>
    <div class='modal-content text-center {ClasseMensagem}'>
        <div class='modal-header d-flex justify-content-center align-items-center fw-bold fs-5'>
            <i class='{Icone} me-2'></i>
            <span>{Titulo}</span>
        </div>
        <div class='modal-body text-center fw-bold fs-5'>
            <span class='mensagem-texto'>{Mensagem}</span>
            <form asp-controller='{ActionController}' asp-action='{ActionName}' method='post' class='text-center mt-4'>
                {antiForgeryInput}
                {(string.IsNullOrEmpty(IdRegistro) ? "" : $"<input type='hidden' name='id' value='{IdRegistro}' />")}
                <div class='d-flex justify-content-center gap-3'>
                    <button type='submit' class='btn {ClasseBotaoPrincipal} btn-bg'>
                        <i class='{IconeBotaoPrincipal}'></i> {TextoBotaoPrincipal}
                    </button>
                    <a href='{voltarUrl}' class='btn {ClasseBotaoSecundario} btn-bg'>
                        <i class='fa fa-arrow-left'></i> {TextoBotaoCancelar}
                    </a>
                </div>
            </form>
        </div>
    </div>
</div>";

            output.Content.SetHtmlContent(html);
        }
    }
}