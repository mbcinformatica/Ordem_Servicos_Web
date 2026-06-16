<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ordem_Servicos_Web.Helpers;
=======
﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ordem_Servicos_Web.Helpers;
using System.Linq;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

namespace Ordem_Servicos_Web.TagHelpers
{
    [HtmlTargetElement("estado-select", Attributes = ForAttributeName)]
    public class EstadoSelectTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
<<<<<<< HEAD
        public ModelExpression? For { get; set; }
=======
        public ModelExpression For { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.Attributes.SetAttribute("class", "form-select");
            output.Attributes.SetAttribute("style", "width:200px;");
<<<<<<< HEAD
            output.Attributes.SetAttribute("name", For?.Name ?? "");
            output.Attributes.SetAttribute("id", For?.Name ?? "");
=======
            output.Attributes.SetAttribute("name", For.Name);
            output.Attributes.SetAttribute("id", For.Name);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

            var estados = EstadoHelper.GetEstados();

            // Cria a lista de opções
            var options = estados.Select(e =>
<<<<<<< HEAD
                $"<option value=\"{e.Value}\" {(For?.Model?.ToString() == e.Value ? "selected" : "")}>{e.Text}</option>"
=======
                $"<option value=\"{e.Value}\" {(For.Model?.ToString() == e.Value ? "selected" : "")}>{e.Text}</option>"
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            );

            output.Content.SetHtmlContent("<option value=\"\">Selecione</option>" + string.Join("", options));
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
