using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ordem_Servicos_Web.Helpers;

namespace Ordem_Servicos_Web.TagHelpers
{
    [HtmlTargetElement("estado-select", Attributes = ForAttributeName)]
    public class EstadoSelectTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression? For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.Attributes.SetAttribute("class", "form-select");
            output.Attributes.SetAttribute("style", "width:200px;");
            output.Attributes.SetAttribute("name", For?.Name ?? "");
            output.Attributes.SetAttribute("id", For?.Name ?? "");

            var estados = EstadoHelper.GetEstados();

            // Cria a lista de opções
            var options = estados.Select(e =>
                $"<option value=\"{e.Value}\" {(For?.Model?.ToString() == e.Value ? "selected" : "")}>{e.Text}</option>"
            );

            output.Content.SetHtmlContent("<option value=\"\">Selecione</option>" + string.Join("", options));
        }
    }
}