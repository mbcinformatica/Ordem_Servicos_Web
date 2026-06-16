using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Ordem_Servicos_Web.Helpers
{
    public static class EstadoHelper
    {
        public static List<SelectListItem> GetEstados()
        {
<<<<<<< HEAD
            return
            [
=======
            return new List<SelectListItem>
            {
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                new SelectListItem { Value = "AC", Text = "AC - Acre" },
                new SelectListItem { Value = "AL", Text = "AL - Alagoas" },
                new SelectListItem { Value = "AP", Text = "AP - Amapá" },
                new SelectListItem { Value = "AM", Text = "AM - Amazonas" },
                new SelectListItem { Value = "BA", Text = "BA - Bahia" },
                new SelectListItem { Value = "CE", Text = "CE - Ceará" },
                new SelectListItem { Value = "DF", Text = "DF - Distrito Federal" },
                new SelectListItem { Value = "ES", Text = "ES - Espírito Santo" },
                new SelectListItem { Value = "GO", Text = "GO - Goiás" },
                new SelectListItem { Value = "MA", Text = "MA - Maranhão" },
                new SelectListItem { Value = "MT", Text = "MT - Mato Grosso" },
                new SelectListItem { Value = "MS", Text = "MS - Mato Grosso do Sul" },
                new SelectListItem { Value = "MG", Text = "MG - Minas Gerais" },
                new SelectListItem { Value = "PA", Text = "PA - Pará" },
                new SelectListItem { Value = "PB", Text = "PB - Paraíba" },
                new SelectListItem { Value = "PR", Text = "PR - Paraná" },
                new SelectListItem { Value = "PE", Text = "PE - Pernambuco" },
                new SelectListItem { Value = "PI", Text = "PI - Piauí" },
                new SelectListItem { Value = "RJ", Text = "RJ - Rio de Janeiro" },
                new SelectListItem { Value = "RN", Text = "RN - Rio Grande do Norte" },
                new SelectListItem { Value = "RS", Text = "RS - Rio Grande do Sul" },
                new SelectListItem { Value = "RO", Text = "RO - Rondônia" },
                new SelectListItem { Value = "RR", Text = "RR - Roraima" },
                new SelectListItem { Value = "SC", Text = "SC - Santa Catarina" },
                new SelectListItem { Value = "SP", Text = "SP - São Paulo" },
                new SelectListItem { Value = "SE", Text = "SE - Sergipe" },
                new SelectListItem { Value = "TO", Text = "TO - Tocantins" }
<<<<<<< HEAD
            ];
=======
            };
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        }
    }
}
