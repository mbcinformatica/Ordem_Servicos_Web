async function validarLogin(
    campo,
    url,
    mensagemErro,
    parametrosExtras = {},
    nomeParametro = "valor"
) {
    const valor = campo.value.trim();
    if (!valor) {
        mostrarToast("Login não informado.", "erro");
        return false;
    }

    const queryParams = new URLSearchParams({ [nomeParametro]: valor, ...parametrosExtras });

    try {
        const response = await fetch(`${url}?${queryParams.toString()}`);
        const data = await response.json();

        // 🔹 Se o endpoint retorna { existe: true/false }
        if (!data.existe && !data.sucesso) {
            mostrarToast(mensagemErro, "erro");
            return false;
        }

        // 🔹 Busca imagem do usuário
        try {
            const imgResponse = await fetch(`/Usuarios/GetUserImage?valor=${encodeURIComponent(valor)}`);
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

// Validação de Login e Senha Juntos
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
        mostrarToast("Erro ao validar login/senha no servidor.", "erro");
        return false;
    }
    limparErro(campoSenha);
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
            mostrarToast("Erro ao consultar no servidor.", "erro");
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
        mostrarToast("Erro ao consultar no servidor.", "erro");
        return false;
    }

    return true;
}

// Validação de CPF
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

// Validação de CNPJ
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

// Função Valida CPF/CNPJ
async function validarCpfCnpj(input, tipoPessoa, entidade) {
    let valor = input.value.replace(/\D/g, "");
    if (tipoPessoa === "JURÍDICA") {
        let valido = ValidaCnpj(valor);
        if (!valido) {
            mostrarToast("CNPJ Inválido.", "erro");
            return false;
        }
    } else if (tipoPessoa === "FÍSICA") {
        let valido = ValidaCpf(valor);
        if (!valido) {
            mostrarToast("CPF Inválido.", "erro");
            return false;
        }
    }

    // 🔹 Checa duplicidade usando entidade dinâmica
    const duplicado = await ValidacaoUtils.consultaDuplicidade(
        input,
        "/Entidades/VerificarDuplicidade",
        "CPF/CNPJ já Cadastrado.",
        { entidade: entidade, campo: "CpfCnpj" }
    );

    if (duplicado) {
        return false; // já existe → inválido
    }

    // 🔹 Se não existe duplicidade e for JURÍDICA → busca dados
    if (tipoPessoa && tipoPessoa.toUpperCase() === "JURÍDICA") {
        const resultado = await buscarCNPJ(
            "CpfCnpj",
            "TipoPessoa",
            "NomeRazaoSocial",
            "Endereco",
            "Numero",
            "Bairro",
            "Municipio",
            "Uf",
            "Cep",
            "FoneCelular",
            "FoneFixo",
            "Email",
            "Contato",
            entidade // 🔹 agora flexível
        );
        if (!resultado) {
            mostrarToast("Erro ao Consultar CNPJ no Servidor.", "erro");
            return false;
        }
    }

    limparErro(input);
    return true;
}

// Função Valida CEP
async function validarCep(input) {
    let valor = input.value.replace(/\D/g, "");
    if (valor.length !== 8) {
        mostrarToast("CEP Inválido. Deve Conter 8 Dígitos.", "erro");
        return false;
    }

    const resultado = await buscarCEP("Cep", "Endereco", "Bairro", "Municipio", "Uf");
    if (!resultado) {
        mostrarToast("CEP não Encontrado.", "aviso");
        return false;
    }

    limparErro(input);
    return true;
}

// Função Valida Telefone
function validarTelefone(input) {
    let valor = input.value.replace(/\D/g, "");
    if (valor.length !== 10 && valor.length !== 11 && valor.length !== 0) {
        mostrarToast("Telefone Inválido. Deve Conter 10 ou 11 Dígitos.", "erro");
        return false;    
    }
    limparErro(input);
    return true;
}

// Função Valida Número
function validarNumero(input) {
    let valor = input.value.replace(/\D/g, "");
    if (valor.length === 0) {
        input.value = "S/N";
    }
    limparErro(input);
    return true;
}

// Função Valida e-MAIL
function validarEmail(input) {
    let valor = input.value.trim();
    let regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!regex.test(valor)) {
        mostrarToast("E-mail Inválido. Informe no Formato usuario@dominio.com.", "erro");
        return false;
    }
    input.value = converteParaMinusculo(input.value);
    limparErro(input);
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
        mostrarToast("A senha deve ter no mínimo 8 caracteres, incluindo maiúscula, minúscula, número e símbolo.", "erro");
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
