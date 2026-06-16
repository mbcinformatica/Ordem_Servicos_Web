const ValidacaoUtils = (function () {

    // Função genérica para validar campo em tempo real + blur
    function validarCampo(
        campo,
        validacaoFn = null,
        consultaServidorFn = null,
        mensagemErro = "",
        opcional = false,
        obrigatorio = false) {   // 🔹 novo parâmetro

        if (!campo) return async () => true;


        const executarValidacao = async () => {
            // opcional e vazio → válido
            if (opcional && !campo.value.trim()) {
                campo.classList.remove("Invalid");
                campo.classList.remove("input-validation-error");
                campo.classList.add("Valid");
                return true;
            }

            // obrigatório e vazio → inválido
            if (obrigatorio && !campo.value.trim()) {
                campo.classList.add("Invalid");
                campo.classList.remove("Valid");
                mostrarToast("Campo Obrigatório não Pode Ficar Vazio.", "erro");
                return false;
            }


            // validação local
            if (validacaoFn) {
                let resultado = await validacaoFn(campo);
                if (resultado instanceof Promise) resultado = await resultado;
                if (!resultado) {
                    campo.classList.add("Invalid");
                    campo.classList.remove("Valid");
                    if (mensagemErro) mostrarToast(mensagemErro, "erro");
                    return false;
                }
            }

            // validação duplicidade
            if (consultaServidorFn) {
                let duplicado = await consultaServidorFn(campo);
                if (duplicado instanceof Promise) duplicado = await duplicado;
                if (duplicado) {
                    campo.classList.add("Invalid");
                    campo.classList.remove("Valid");
                    return false;
                }
            }

            campo.classList.remove("Invalid");
            campo.classList.remove("input-validation-error");
            campo.classList.add("Valid");
            return true;
        };

        campo.addEventListener("blur", async (e) => {
            const valido = await executarValidacao();
            if (!valido && obrigatorio) {
                // 🔹 devolve o foco se obrigatório e inválido
                e.preventDefault();
                campo.focus();
            }
        });

        campo._executarValidacao = executarValidacao;

        return executarValidacao;
    }

    // 🔹 Função genérica de duplicidade reutilizável
    async function consultaDuplicidade(
        campo,
        url,
        mensagemErro,
        parametrosExtras = {},
        nomeParametro = "valor"
    ) {
        let valor = campo.value.trim();
        const campoLower = (parametrosExtras.campo || nomeParametro).toLowerCase();

        // 🔹 Normaliza campos numéricos
        const camposNumericos = ["cpf", "cnpj", "cep", "fone", "telefone"];
        if (camposNumericos.some(c => campoLower.includes(c))) {
            valor = valor.replace(/\D/g, "");
            campo.value = valor;
        }

        if (valor.length === 0) return false;

        const queryParams = new URLSearchParams({ [nomeParametro]: valor, ...parametrosExtras });

        try {
            const response = await fetch(`${url}?${queryParams.toString()}`);
            const data = await response.json();

            if (data.existe) {
                mostrarToast(mensagemErro, "erro");
                return true;
            }

            return false;
        } catch (error) {
            mostrarToast("Erro ao Consultar CPF/CNPJ no Servidor.", "erro");
            return true; // 🔹 trata como duplicidade/erro
        }
    }

    // 🔹 Validação final no submit
    function validarFormulario(form, camposObrigatorios) {
        form.addEventListener("submit", function (e) {
            const invalidFields = form.querySelectorAll(".Invalid");
            if (invalidFields.length > 0) {
                e.preventDefault();
                mostrarToast("Existem Campos Inválidos. Corrija Antes de Salvar.", "erro");
                invalidFields[0].focus();
                return false;
            }

            for (let id of camposObrigatorios) {
                const campo = document.getElementById(id);
                if (!campo || !campo.value.trim()) {
                    e.preventDefault();
                    mostrarToast("Preencha Todos os Campos Obrigatórios.", "erro");
                    campo.focus();
                    return false;
                }
            }
            return true;
        });
    }

    // 🔹 Configuração automática de validação
    function configurarValidacaoCampos(form, camposObrigatorios = [], entidade) {
        const campos = form.querySelectorAll("input, select, textarea");

        campos.forEach(campo => {
            const id = campo.id?.toLowerCase() || "";
            const ehObrigatorio = camposObrigatorios.includes(campo.id);

            // 🔹 se não for Create e o campo estiver readonly → não valida
            if (campo.hasAttribute("readonly")) {
                return;
            }

            if (id.includes("cpfcnpj")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    async c => await validarCpfCnpj(
                        c,
                        document.getElementById("TipoPessoa")?.value,
                        entidade
                    ),
                    null,                    
                    "",
                    false,
                    ehObrigatorio
                );
            }
            else if (id.includes("cep")) {
                ValidacaoUtils.validarCampo(campo, validarCep, null, "", false, ehObrigatorio);
            }
            else if (id.includes("numero")) {
                ValidacaoUtils.validarCampo(campo, validarNumero, null, "", false, ehObrigatorio);
            }
            else if (id.includes("email")) {
                ValidacaoUtils.validarCampo(campo, validarEmail, null, "", true, ehObrigatorio);
            }
            else if (id.includes("fone") || id.includes("telefone")) {
                ValidacaoUtils.validarCampo(campo, validarTelefone, null, "", true, ehObrigatorio);
            }
            else if (id.includes("nomeusuario")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Usuário já Cadastrado.",
                        { entidade: entidade, campo: "NomeUsuario" }
                    ),
                    "Nome do Usuário é Obrigatório.",
                    false,
                    ehObrigatorio
                );
            }
            else if (id.includes("login")) {
                const isLoginScreen = form.id?.toLowerCase().includes("formlogin"); // 🔹 identifica se é tela de login

                if (isLoginScreen) {
                    // Tela de login → valida se o login existe
                    ValidacaoUtils.validarCampo(
                        campo,
                        async c => await validarLogin(
                            c,
                            "/Entidades/VerificarDuplicidade",
                            "Login não Cadastrado.",
                            { entidade: entidade, campo: "Login" }
                        ),
                        null,
                        "",
                        false,
                        ehObrigatorio
                    );
                } else {
                    // Tela de cadastro → valida duplicidade
                    ValidacaoUtils.validarCampo(
                        campo,
                        c => c.value.trim().length > 0,
                        c => ValidacaoUtils.consultaDuplicidade(
                            c,
                            "/Entidades/VerificarDuplicidade",
                            "Login já Cadastrado.",
                            { entidade: entidade, campo: "Login" }
                        ),
                        "Login do Usuário é Obrigatório.",
                        false,
                        ehObrigatorio
                    );
                }
            }
            else if (id.includes("senha")) {
                const isLoginScreen = form.id?.toLowerCase().includes("formlogin"); // 🔹 identifica se é tela de login

                if (isLoginScreen) {
                    ValidacaoUtils.validarCampo(
                        campo,
                        async c => await validarLoginSenha(
                            c,
                            document.getElementById("Login")?.value
                        ),
                        null,
                        "",
                        false,
                        ehObrigatorio
                    );
                } else {
                    ValidacaoUtils.validarCampo(
                        campo,
                        c => validarSenhaForte(
                            c,
                            document.getElementById("ConfirmaSenha"),
                            false // senha opcional na alteração
                        ),
                        null,
                        "",
                        false,
                        ehObrigatorio
                    );
                }
            }
            else if (id.includes("confirmasenha")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => validarConfirmacaoSenha(
                        c,
                        document.getElementById("Senha"),
                        false
                    ),
                    null,
                    "",
                    false,
                    ehObrigatorio
                );
            }
            else if (id.includes("fornecedorselect")) {
                ["focus", "click"].forEach(evt => {
                    fornecedorSelect.addEventListener(evt, async function () {
                        if (fornecedorSelect.options.length <= 1) {
                            await GetEntidades(
                                fornecedorSelect,
                                "/Entidades/GetEntidades",
                                "FORNECEDORES",
                                { campoDescricao: "NomeRazaoSocial", apelido: "f" }
                            );
                        }
                    });
                });

                // 🔹 ao selecionar fornecedor → foca no próximo campo (marca)
                fornecedorSelect.addEventListener("change", function () {
                    if (fornecedorSelect.value && marcaSelect) {
                        marcaSelect.focus();
                    }
                });
            }
            // 🔹 Marca
            else if (id.includes("marcaselect")) {
                ["focus", "click"].forEach(evt => {
                    marcaSelect.addEventListener(evt, async function () {
                        if (marcaSelect.options.length <= 1) {
                            await GetEntidades(
                                marcaSelect,
                                "/Entidades/GetEntidades",
                                "MARCAS",
                                { campoDescricao: "Descricao", apelido: "ma" }
                            );
                        }
                    });
                });

                // 🔹 ao selecionar fornecedor → foca no próximo campo (marca)
                marcaSelect.addEventListener("change", function () {
                    if (marcaSelect.value && modeloSelect) {
                        modeloSelect.focus();
                    }
                });
            }
            // 🔹 Modelo
            else if (id.includes("modeloselect")) {
                ["focus", "click"].forEach(evt => {
                    modeloSelect.addEventListener(evt, async function () {
                        if (modeloSelect.options.length <= 1) {
                            await GetEntidades(
                                modeloSelect,
                                "/Entidades/GetEntidades",
                                "MODELOS",
                                { campoDescricao: "Descricao", apelido: "mo" }
                            );
                        }
                    });
                });

                // 🔹 ao selecionar fornecedor → foca no próximo campo (marca)
                modeloSelect.addEventListener("change", function () {
                    if (modeloSelect.value && unidadeSelect) {
                        unidadeSelect.focus();

                        const descricaoProduto = document.getElementById("Descricao");
                        const selectedOption = campo.options[campo.selectedIndex];
                        if (descricaoProduto && selectedOption && selectedOption.text) {
                            descricaoProduto.value = selectedOption.text;
                        }
                    }
                });
            }
            // 🔹 Unidade
            else if (id.includes("unidadeselect")) {
                ["focus", "click"].forEach(evt => {
                    unidadeSelect.addEventListener(evt, async function () {
                        if (unidadeSelect.options.length <= 1) {
                            await GetEntidades(
                                unidadeSelect,
                                "/Entidades/GetEntidades",
                                "UNIDADES",
                                { campoDescricao: "Descricao", apelido: "un" }
                            );
                        }
                    });
                });

                // 🔹 ao selecionar fornecedor → foca no próximo campo (marca)
                unidadeSelect.addEventListener("change", function () {
                    if (unidadeSelect.value && PrecoCompra) {
                        PrecoCompra.focus();
                        if (typeof PrecoCompra.select === "function") PrecoCompra.select();
                    }
                });
            }
            else {
                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio);
            }
        });
    }
    return {
        validarCampo,
        consultaDuplicidade,
        validarFormulario,
        configurarValidacaoCampos
    };

})();