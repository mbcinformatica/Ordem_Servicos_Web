// Função para Validação de Login
async function validarLogin(
    campo,
    url,
    mensagemErro,
    parametrosExtras = {},
    nomeParametro = "valor") {

    let valor = campo.value.trim();

    if (valor.length === 0) return false;

    // 🔹 Lista de campos que devem ser normalizados (apenas dígitos)
    const campoNumericos = ["cpf", "cnpj", "cep", "fone", "telefone"];

    // Normaliza o campo principal
    const campoLower = (campo.name || campo.id || nomeParametro).toLowerCase();
    if (campoNumericos.some(c => campoLower.includes(c))) {
        valor = valor.replace(/\D/g, "");
        campo.value = valor;
    }

    // Monta query string inicial
    const queryParams = new URLSearchParams({ [nomeParametro]: valor });

    // Adiciona entidade se existir
    if (parametrosExtras.entidade) {
        queryParams.append("entidade", parametrosExtras.entidade);
    }

    // 🔹 Normaliza e adiciona múltiplos campo extras
    if (parametrosExtras.campo && typeof parametrosExtras.campo === "object") {
        Object.entries(parametrosExtras.campo).forEach(([key, val]) => {
            let valNormalizado = val;
            if (typeof val === "string" && campoNumericos.some(c => key.toLowerCase().includes(c))) {
                valNormalizado = val.replace(/\D/g, "");
            }
            queryParams.append(key, valNormalizado);
        });
    }

    try {
        const response = await fetch(`${url}?${queryParams.toString()}`);
        const data = await response.json();

        // 🔹 Se o endpoint retorna { existe: true/false } ou { sucesso: true/false }
        if (!(data.existe)) {
            mostrarToast(mensagemErro, "erro");
            return false;
        }

        // 🔹 Busca imagem do usuário
        try {
            const imgResponse = await fetch(
                `/Entidades/GetImagens?campoID=${encodeURIComponent(valor)}&campoBD=Login&campoImagem=Imagem&campoDescricao=NomeUsuario&entidade=Usuario&apelido=us`
            );
            const imgData = await imgResponse.json();

            if (imgData.exists && imgData.imagemBase64) {
                userImage.src = `data:${imgData.contentType};base64,${imgData.imagemBase64}`;
                userImageContainer.style.display = "block";
                nomeUsuario.textContent = imgData.nome;
            } else {
                userImageContainer.style.display = "none";
                nomeUsuario.textContent = "";
            }
        } catch {
            userImageContainer.style.display = "none";
            nomeUsuario.textContent = "";
        }

    } catch (error) {
        mostrarToast("Erro ao Consultar Login no Servidor.", "erro");
        return false;
    }

    limparErro(campo);
    return true;
}

// Função para Validação de Login e Senha Juntos
async function validarLoginSenha(campoSenha, login) {
    const senha = campoSenha.value.trim();

    if (!login || !senha) {
        return false;
    }

    try {
        // 🔹 Chama endpoint que valida login/senha
        const response = await fetch(`/Entidades/ValidarLoginSenha?login=${encodeURIComponent(login)}&senha=${encodeURIComponent(senha)}`);
        const data = await response.json();
        if (!data.sucesso) {
            mostrarToast("Senha Incorreta. Tente Novamente.", "erro");
            return false;
        }

    } catch (error) {
        mostrarToast("Erro ao Validar Login/Senha no Servidor.", "erro");
        return false;
    }
    limparErro(campoSenha);
    return true;
}

// Função Valida Senha Forte
function validarSenhaForte(campo, campoConfirmacao = null, opcional = false) {
    const valor = campo.value.trim();


    // 🔹 Se for opcional e estiver vazio
    if (opcional && valor.length === 0) {

        // Desabilita confirmação se senha estiver vazia
        if (campoConfirmacao) {
            campoConfirmacao.value = "";
            campoConfirmacao.disabled = true;
        }
        return true;
    }

    // Habilita confirmação se senha foi preenchida
    if (campoConfirmacao) {
        campoConfirmacao.disabled = false;
    }

    // Regex para senha forte
    const regexSenhaForte = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/;

    if (!regexSenhaForte.test(valor)) {
        mostrarToast("A Senha Deve ter no Mínimo 8 Caracteres, Incluindo Maiúscula, Minúscula, Número e Símbolo.", "erro");
        return false;
    }
    limparErro(campo);
    return true;
}

// Função Valida Confirmação de Senha
function validarConfirmacaoSenha(campoConfirmacao, campoSenha, opcional = false) {
    const senha = campoSenha.value.trim();
    const confirmacao = campoConfirmacao.value.trim();

    // 🔹 Se senha for opcional e estiver vazia, confirmação fica desabilitada
    if (opcional && senha.length === 0) {
        campoConfirmacao.disabled = true;
        campoConfirmacao.value = "";
        return true;
    }

    if (senha !== confirmacao) {
        mostrarToast("A Confirmação da Senha não Confere.", "erro");
        return false;
    }

    limparErro(campoConfirmacao);
    return true;
}

// Função para buscar entidades dinamicamente
async function GetEntidades(campo, url, entidade, parametrosExtras = {}) {
    const queryParams = new URLSearchParams({
        entidade,
        ...parametrosExtras
    });

    try {
        const response = await fetch(`${url}?${queryParams.toString()}`);
        const result = await response.json();

        if (!result.sucesso) {
            mostrarToast("Erro ao Consultar no Servidor.", "erro");
            return false;
        }

        // 🔹 popula o select dinamicamente
        campo.innerHTML = "";
        campo.append(new Option(`-- Selecione ${entidade} --`, ""));
        result.dados.forEach(item => {
            campo.append(new Option(item.valorDescricao, item.id));
        });

        // 🔹 garante que ao selecionar, o valor fique armazenado
        campo.addEventListener("change", function () {
            if (campo.value) {
                // campo.value já é o id selecionado
                limparErro(campo);
            }
        });

    } catch (error) {
        mostrarToast("Erro ao Consultar no Servidor.", "erro");
        return false;
    }

    return true;
}

// Função paraValidação de CPF
function ValidaCpf(cpf) {
    cpf = cpf.replace(/\D/g, "");
    if (cpf.length !== 11) return false;

    let soma = 0, resto;
    for (let i = 1; i <= 9; i++) soma += parseInt(cpf.substring(i - 1, i)) * (11 - i);
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.substring(9, 10))) return false;

    soma = 0;
    for (let i = 1; i <= 10; i++) soma += parseInt(cpf.substring(i - 1, i)) * (12 - i);
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    return resto === parseInt(cpf.substring(10, 11));
}

// Função para Validação de CNPJ
function ValidaCnpj(cnpj) {
    cnpj = cnpj.replace(/\D/g, "");
    if (cnpj.length !== 14) return false;

    let tamanho = cnpj.length - 2;
    let numeros = cnpj.substring(0, tamanho);
    let digitos = cnpj.substring(tamanho);
    let soma = 0, pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
        soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
        if (pos < 2) pos = 9;
    }
    let resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado !== parseInt(digitos.charAt(0))) return false;

    tamanho++;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (let i = tamanho; i >= 1; i--) {
        soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
        if (pos < 2) pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    return resultado === parseInt(digitos.charAt(1));
}

// Função para validar CPF/CNPJ
async function validarCpfCnpj(input, tipoPessoa, entidade) {
    let valor = semMascaraCampo(input);

    if (tipoPessoa === "JURÍDICA") {
        if (valor.length !== 14 || !ValidaCnpj(valor)) {
            mostrarToast("CNPJ Inválido.", "erro");
            return false;
        }
    } else if (tipoPessoa === "FÍSICA") {
        if (valor.length !== 11 || !ValidaCpf(valor)) {
            mostrarToast("CPF Inválido.", "erro");
            return false;
        }
    }
    const duplicado = await ValidacaoUtils.consultaDuplicidade(
        input,
        "/Entidades/VerificarDuplicidade",
        "CPF/CNPJ já Cadastrado.",
        {
            entidade: entidade,
            campo: {
                CpfCnpj: document.getElementById("CpfCnpj").value
            }
        }
    );
    if (duplicado) return false;

    if (tipoPessoa && tipoPessoa.toUpperCase() === "JURÍDICA") {
        const resultado = await buscarCNPJ(
            "CpfCnpj", "TipoPessoa", "NomeRazaoSocial", "Endereco", "Numero",
            "Bairro", "Municipio", "Uf", "Cep", "FoneCelular", "FoneFixo", "Email", "Contato",
            entidade
        );
        if (!resultado) {
            mostrarToast("Erro ao Consultar CNPJ no Servidor.", "erro");
            return false;
        }
    }

    limparErro(input);
    return true;
}

// Função para validar CEP
async function validarCep(input) {
    let valor = semMascaraCampo(input);

    if (valor.length !== 0 && valor.length !== 8) {
        mostrarToast("CEP Inválido. Deve Conter 8 Dígitos.", "erro");
        return false;
    }

    const resultado = await buscarCEP("Cep", "Endereco", "Bairro", "Municipio", "Uf");
    if (!resultado) {
        mostrarToast("CEP não Existente.", "aviso");
        return false;
    }

    limparErro(input);
    return true;
}

// Função para validar Telefone
function validarTelefone(input) {
    let valor = semMascaraCampo(input);

    if (valor.length !== 0 && valor.length !== 10 && valor.length !== 11) {
        mostrarToast("Telefone Inválido. Deve Conter 10 ou 11 Dígitos.", "erro");
        return false;
    }

    limparErro(input);
    return true;
}

// Função Valida Número
function validarNumero(input) {
    let valor = semMascaraCampo(input);
    if (valor.length === 0) {
        input.value = "S/N";
    }
    limparErro(input);
    return true;
}

// Função Valida Valor Monetário
function validarValor(input) {

    limparErro(input);
    return true;
}

// Função Valida Quantidade
function validarQuantidade(input) {

    limparErro(input);
    return true;
}

// Função Valida Código Interno do Produto
async function validarProdutoInterno(input) {
    // 🔹 Remove espaços extras
    let valor = input.value.trim();

    // 🔹 Só adiciona prefixo se ainda não tiver
    if (valor && !valor.startsWith("MBC-")) {
        input.value = "MBC-" + valor;
    }

    limparErro(input);
    return true;
}

// Função Valida e-MAIL
function validarEmail(input) {
    let valor = input.value.trim();
    let regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (valor.length !== 0 && !regex.test(valor)) {
        mostrarToast("E-Mail Inválido. Formato Correto usuario@dominio.com.", "erro");
        return false;
    }

    input.value = input.value.trim().toLowerCase();
    limparErro(input);
    return true;
}