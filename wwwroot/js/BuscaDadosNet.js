// Função que Busca o Cep da Internet
function buscarCEP(cepInputId, enderecoId, bairroId, municipioId, ufId) {
    return new Promise((resolve, reject) => {
        const cepCampo = document.getElementById(cepInputId);
        const cep = cepCampo.value.replace(/\D/g, "");

        if (cep.length !== 8) {
            resolve(false);
            return;
        }

        fetch(`https://viacep.com.br/ws/${cep}/json/`)
            .then(response => response.json())
            .then(data => {
                if (data.erro) {
                    resolve(false);
                    return;
                }

                if (enderecoId) document.getElementById(enderecoId).value = data.logradouro || "";
                if (bairroId) document.getElementById(bairroId).value = data.bairro || "";
                if (municipioId) document.getElementById(municipioId).value = data.localidade || "";
                if (ufId) document.getElementById(ufId).value = data.uf || "";

                resolve(true);
            })
            .catch(() => {
                resolve(false);
            });
    });
}

// Função que Busca os Dados de CNPJ da Internet
function buscarCNPJ(
    cpfCnpjId, tipoPessoaId, nomeId,
    enderecoId, numeroId, bairroId, municipioId,
    ufId, cepId, foneCelularId, foneFixoId,
    emailId, contatoId, entidade) {
    return new Promise((resolve, reject) => {
        const cpfCnpjInput = document.getElementById(cpfCnpjId);

        if (!cpfCnpjInput) {
            resolve(false);
            return;
        }
        // 🔹 Remove qualquer formatação (mantém só números)
        const cnpj = cpfCnpjInput.value.replace(/\D/g, "");
        cpfCnpjInput.value = cnpj; // 🔹 garante que o campo fique limpo
        
        fetch(`/Entidades/BuscarDadosPorCnpj?cnpj=${encodeURIComponent(cnpj)}&entidade=${encodeURIComponent(entidade)}`)
            .then(response => response.json())
            .then(data => {
                if (!data.sucesso) {
                    resolve(false);
                    return;
                }
        
                // 🔹 Preenche os campos retornados
                if (nomeId) document.getElementById(nomeId).value = data.nomeRazaoSocial || "";
                if (enderecoId) document.getElementById(enderecoId).value = data.endereco || "";
                if (numeroId) {
                    const numeroInput = document.getElementById(numeroId);
                    numeroInput.value = data.numero || "";
                    validarNumero(numeroInput);
                }
                if (bairroId) document.getElementById(bairroId).value = data.bairro || "";
                if (municipioId) document.getElementById(municipioId).value = data.municipio || "";
                if (ufId) document.getElementById(ufId).value = data.uf || "";
        
                // 🔹 Campos numéricos também sem formatação
                if (cepId) document.getElementById(cepId).value = (data.cep || "").replace(/\D/g, "");
                if (foneCelularId) document.getElementById(foneCelularId).value = (data.foneCelular || "").replace(/\D/g, "");
                if (foneFixoId) document.getElementById(foneFixoId).value = (data.foneFixo || "").replace(/\D/g, "");
        
                if (emailId) document.getElementById(emailId).value = data.email || "";
                if (contatoId) document.getElementById(contatoId).value = data.contato || "";
        
                resolve(true);
        
            })
            .catch(() => {
                resolve(false);
            });
    });
}
