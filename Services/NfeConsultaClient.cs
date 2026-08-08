using System.ServiceModel;
using System.Xml;

namespace Ordem_Servicos_Web.Services
{
    internal class NfeConsultaClient
    {
        private BasicHttpBinding binding;
        private EndpointAddress endpoint;

        public NfeConsultaClient(BasicHttpBinding binding, EndpointAddress endpoint)
        {
            this.binding = binding;
            this.endpoint = endpoint;
        }

        internal Object nfeConsultaNF(XmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}