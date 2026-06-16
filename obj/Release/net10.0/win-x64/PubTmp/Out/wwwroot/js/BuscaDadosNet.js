// Função que Busca o Cep da Internet
function buscarCEP(cepInputId, enderecoId, bairroId, municipioId, ufId) {
    const cep = document.getElementById(cepInputId).value.replace(/\D/g, "");

    if (cep.length === 8) {
        fetch(`https://viacep.com.br/ws/${cep}/json/`)
            .then(response => response.json())
            .then(data => {
                if (!data.erro) {
                    if (enderecoId) document.getElementById(enderecoId).value = data.logradouro || "";
                    if (bairroId) document.getElementById(bairroId).value = data.bairro || "";
                    if (municipioId) document.getElementById(municipioId).value = data.localidade || "";
                    if (ufId) document.getElementById(ufId).value = data.uf || "";
                } else {
                    mostrarToast("CEP não encontrado.", "aviso");
                }
            })
            .catch(() => {
                mostrarToast("Erro ao consultar CEP.", "erro");
            });
    } else if (cep.length > 0) {
        mostrarToast("CEP inválido. Deve conter 8 dígitos.", "erro");
    }
}
// Função que Busca o Dados de CNPJ da Internet
function buscarCNPJ(cpfCnpjId, tipoPessoaId, nomeId,
    enderecoId, numeroId, bairroId, municipioId,
    ufId, cepId, fone1Id, fone2Id, emailId, contatoId) {

    const cnpj = document.getElementById(cpfCnpjId).value;

    fetch(`/Clientes/BuscarDadosPorCnpj?cnpj=${encodeURIComponent(cnpj)}`)
        .then(response => response.json())
        .then(data => {
            if (!data.sucesso) {
                mostrarToast(data.mensagem || "Erro ao consultar CNPJ.", "erro");
                return;
            }

            document.getElementById(nomeId).value = data.nomeRazaoSocial || "";
            document.getElementById(enderecoId).value = data.endereco || "";
            document.getElementById(numeroId).value = data.numero || "";
            document.getElementById(bairroId).value = data.bairro || "";
            document.getElementById(municipioId).value = data.municipio || "";
            document.getElementById(ufId).value = data.uf || "";
            document.getElementById(cepId).value = data.cep || "";
            document.getElementById(fone1Id).value = data.fone1 || "";
            document.getElementById(fone2Id).value = data.fone2 || "";
            document.getElementById(emailId).value = data.email || "";
            document.getElementById(contatoId).value = data.contato || "";
        })
        .catch(() => {
            mostrarToast("Erro ao consultar CNPJ no servidor.", "erro");
        });
}