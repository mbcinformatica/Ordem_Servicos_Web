using System.ServiceModel;
using System.Xml;

namespace Ordem_Servicos_Web.Services
{
    internal class NFeDistribuicaoDFeClient(BasicHttpBinding binding, EndpointAddress endpoint)
    {
        private BasicHttpBinding binding = binding;
        private EndpointAddress endpoint = endpoint;

        public required Object ClientCredentials { get; internal set; }

        internal Object nfeDistDFeInteresse(XmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}