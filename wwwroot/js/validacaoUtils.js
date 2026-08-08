const ValidacaoUtils = (function () {

    // Função genérica para validar campo em tempo real + blur
    function validarCampo(
        campo,
        validacaoFn = null,
        consultaServidorFn = null,
        mensagemErro = "",
        opcional = false,
        obrigatorio = false,
        form = null) {

        if (!campo) return async () => true;

        const executarValidacao = async () => {

            // 🔹 remove classes padrão do MVC
            campo.classList.remove("input-validation-error");
            campo.classList.remove("valid");

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

                let label = form.querySelector(`label[for="${campo.id}"]`);
                if (!label) {
                    label = campo.closest("div").querySelector("label");
                }
                const descricao = label ? label.textContent.trim() : campo.name || campo.id;
                mostrarToast(`O Preenchimento do Campo ${descricao} é  Obrigatório`, "erro");
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
            if (consultaServidorFn && form.classList.contains("novo")) {
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
        nomeParametro = "valor") {

        let valor = campo.value.trim();

        if (valor.length === 0) return false;

        // 🔹 Lista de campo que devem ser normalizados (apenas dígitos)
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

            if (data.existe) {
                mostrarToast(mensagemErro, "erro");
                return true;
            }
            return false;
        } catch (error) {
            mostrarToast("Erro ao Consultar Dados no Servidor.", "erro");
            return true; // 🔹 trata como duplicidade/erro
        }
    }

    // 🔹 Validação final no submit
    async function validarFormulario(form, camposObrigatorios) {
        if (Array.isArray(camposObrigatorios) && camposObrigatorios.some(id => id && id.trim() !== "")) {
            form.addEventListener("submit", function (e) {

                // pega o primeiro campo marcado como inválido
                const campoInvalido = form.querySelector(".Invalid");
                if (campoInvalido) {

                    e.preventDefault();
                    let label = form.querySelector(`label[for="${campoInvalido.id}"]`);
                    if (!label) {
                        label = campoInvalido.closest("div").querySelector("label");
                    }
                    const descricao = label ? label.textContent.trim() : campoInvalido.name || campoInvalido.id;
                    mostrarToast(`O Preenchimento do Campo ${descricao} é  Obrigatório`, "erro");
                    campoInvalido.focus();
                    return false;
                }

                return true;
            });
        }
    }

    // 🔹 Configuração automática de validação
    function configurarValidacaoCampos(form, camposObrigatorios = [], entidade) {
        const campos = form.querySelectorAll("input, select, textarea");

        campos.forEach(campo => {
            const ehObrigatorio = camposObrigatorios.includes(campo.id);

            if (campo.hasAttribute("readonly")) return;

            if (campo.classList.contains("data")) {
                ValidacaoUtils.validarCampo(campo, validarData, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("cpfcnpj")) {
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
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("cep")) {
                ValidacaoUtils.validarCampo(campo, validarCep, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("numero")) {
                ValidacaoUtils.validarCampo(campo, validarNumero, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("valor")) {
                ValidacaoUtils.validarCampo(campo, validarValor, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("quantidade")) {
                ValidacaoUtils.validarCampo(campo, validarQuantidade, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("email")) {
                ValidacaoUtils.validarCampo(campo, validarEmail, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("telefone")) {
                ValidacaoUtils.validarCampo(campo, validarTelefone, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("nomeUsuario")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Usuário já Cadastrado.",
                        {
                            entidade: entidade,
                            campo: {
                                NomeUsuario: document.getElementById("NomeUsuario").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("descricaoCategoria")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Categoria de Servicos já Cadastrada.",
                        {
                            entidade: entidade,
                            campo: {
                                Descricao: document.getElementById("Descricao").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("descricaoMarca")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Marca já Cadastrada.",
                        "Marca já Cadastrada.",
                        {
                            entidade: entidade,
                            campo: {
                                Descricao: document.getElementById("Descricao").value                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("descricaoModelo")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Modelo já Cadastrado com essa Marca.",
                        {
                            entidade: entidade,
                            campo: {
                                Descricao: document.getElementById("Descricao").value,
                                IdMarca: document.getElementById("marcaSelect").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("descricaoUnidade")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Unidade já Cadastrada.",
                        {
                            entidade: entidade,
                            campo: {
                                Descricao: document.getElementById("Descricao").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("idCodigoBase")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Código Base já Cadastrado.",
                        {
                            entidade: entidade,
                            campo: {
                                IdCodigoBase: document.getElementById("IdCodigoBase").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("idProdutoInterno")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    async c => await validarProdutoInterno(c),
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Código Interno de Produto já Cadastrado.",
                        {
                            entidade: entidade,
                            campo: {
                                IdProdutoInterno: document.getElementById("IdProdutoInterno").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("idProdutoFabricante")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Código Produto Fabricante já Cadastrado.",
                        {
                            entidade: entidade,
                            campo: {
                                IdProdutoFabricante: document.getElementById("IdProdutoFabricante").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("descricaoServico")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Serviço já Cadastrado.",
                        {
                            entidade: entidade,
                            campo: {
                                Descricao: document.getElementById("Descricao").value,
                                IdCategoriaServico: document.getElementById("categoriaServicoSelect").value
                            }
                        }
                    ),
                    "",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("login")) {
                const isLoginScreen = form.id?.toLowerCase().includes("formlogin");

                if (isLoginScreen) {
                    // Tela de login → valida se o login existe
                    ValidacaoUtils.validarCampo(
                        campo,
                        async c => await validarLogin(
                            c,
                            "/Entidades/VerificarDuplicidade",
                            "Login não Cadastrado.",
                            {
                                entidade: entidade,
                                campo: {
                                    Login: document.getElementById("Login").value
                                }
                            }
                        ),
                        null,
                        "",
                        false,
                        ehObrigatorio,
                        form
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
                            {
                                entidade: entidade,
                                campo: {
                                    Login: document.getElementById("Login").value
                                }
                            }
                        ),
                        "",
                        false,
                        ehObrigatorio,
                        form
                    );
                }
            }
            else if (campo.classList.contains("senha")) {
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
                        ehObrigatorio,
                        form
                    );
                } else {
                    if (form.classList.contains("novo")) {
                        ValidacaoUtils.validarCampo(
                            campo,
                            c => validarSenhaForte(
                                c,
                                document.getElementById("ConfirmaSenha"),
                                false
                            ),
                            null,
                            "",
                            false,
                            ehObrigatorio,
                            form
                        );
                    }
                    else {
                        ValidacaoUtils.validarCampo(
                            campo,
                            c => validarSenhaForte(
                                c,
                                document.getElementById("ConfirmaSenha"),
                                true
                            ),
                            null,
                            "",
                            false,
                            ehObrigatorio,
                            form
                        );
                    }
                }
            }
            else if (campo.classList.contains("confirmasenha")) {
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
                    ehObrigatorio,
                    form);
            }
            else if (campo.classList.contains("categoriaServicoSelect")) {
                const categoriaServicoSelect = document.getElementById("categoriaServicoSelect");
                const descricao = document.getElementById("Descricao");

                ["focus", "click"].forEach(evt => {
                    categoriaServicoSelect.addEventListener(evt, async function () {
                        if (categoriaServicoSelect.options.length <= 1) {
                            await GetEntidades(
                                categoriaServicoSelect,
                                "/Entidades/GetEntidades",
                                "CATEGORIASERVICOS",
                                { campoDescricao: "Descricao", apelido: "cs" }
                            );
                        }
                    });
                });

                // 🔹 intercepta setas para navegar dentro do select
                categoriaServicoSelect.addEventListener("keydown", function (e) {
                    const options = categoriaServicoSelect.options;
                    let index = categoriaServicoSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault(); // impede que o foco vá para o próximo campo
                        if (index < options.length - 1) {
                            categoriaServicoSelect.selectedIndex = index + 1;
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            categoriaServicoSelect.selectedIndex = index - 1;
                        }
                    }
                });

                // 🔹 ao selecionar categoria de serviço → foca no próximo campo (descrição)
                categoriaServicoSelect.addEventListener("blur", function () {
                    if (categoriaServicoSelect.value && descricao) {
//                        descricao.focus();
                    }
                });

                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("usuarioSelect")) {
                const usuarioSelect = document.getElementById("usuarioSelect");
                ["focus", "click"].forEach(evt => {
                    usuarioSelect.addEventListener(evt, async function () {
                        if (usuarioSelect.options.length <= 1) {
                            await GetEntidades(
                                usuarioSelect,
                                "/Entidades/GetEntidades",
                                "USUARIOS",
                                { campoDescricao: "NomeUsuario", apelido: "us" }
                            );
                        }
                    });
                });

                // 🔹 intercepta setas para navegar dentro do select
                usuarioSelect.addEventListener("keydown", function (e) {
                    const options = usuarioSelect.options;
                    let index = usuarioSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault(); // impede que o foco vá para o próximo campo
                        if (index < options.length - 1) {
                            usuarioSelect.selectedIndex = index + 1;
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            usuarioSelect.selectedIndex = index - 1;
                        }
                    }
                });

                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("menuSelect")) {
                const menuSelect = document.getElementById("menuSelect");
                const itensMenuSelect = document.getElementById("itensMenuSelect");

                ["focus", "click"].forEach(evt => {
                    menuSelect.addEventListener(evt, async function () {
                        if (menuSelect.options.length <= 1) {
                            await GetEntidades(
                                menuSelect,
                                "/Entidades/GetEntidades",
                                "MENUS",
                                { campoDescricao: "Descricao", apelido: "ma" }
                            );
                        }
                    });
                });

                // 🔹 intercepta setas para navegar dentro do select
                menuSelect.addEventListener("keydown", function (e) {
                    const options = menuSelect.options;
                    let index = menuSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault(); // impede que o foco vá para o próximo campo
                        if (index < options.length - 1) {
                            menuSelect.selectedIndex = index + 1;
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            menuSelect.selectedIndex = index - 1;
                        }
                    }
                });

                // 🔹 ao selecionar marca → carrega modelos
                menuSelect.addEventListener("blur", async function () {
                    if (menuSelect.value && itensMenuSelect) {
                        itensMenuSelect.innerHTML = "<option value=''>Selecione o item do menu</option>";

                        // chamada AJAX para o endpoint
                        const response = await fetch(`/Entidades/GetItensMenuPorMenu?idMenu=${menuSelect.value}`);
                        const itensMenu = await response.json();

                        itensMenu.forEach(im => {
                            const option = document.createElement("option");
                            option.value = im.idItensMenu;
                            option.text = im.descricao;
                            itensMenuSelect.appendChild(option);
                        });

                        itensMenuSelect.focus();
                    }
                });
                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("itensMenuSelect")) {
                ValidacaoUtils.validarCampo(
                    campo,
                    c => c.value.trim().length > 0,
                    c => ValidacaoUtils.consultaDuplicidade(
                        c,
                        "/Entidades/VerificarDuplicidade",
                        "Já Existe uma Permissão Cadastrada para este Usuário, Menu e SubItem.",
                        {
                            entidade: entidade,
                            campo: {
                                IdUsuario: document.getElementById("usuarioSelect").value,
                                IdMenu: document.getElementById("menuSelect").value,
                                IdItensMenu: document.getElementById("itensMenuSelect").value                            }
                        }
                    ),
                    "Item do Menu é Obrigatório.",
                    false,
                    ehObrigatorio,
                    form
                );
            }
            else if (campo.classList.contains("fornecedorSelect")) {
                const fornecedorSelect = document.getElementById("fornecedorSelect");
                const marcaSelect = document.getElementById("marcaSelect");
                ["focus", "click"].forEach(evt => {
                    fornecedorSelect.addEventListener(evt, async function () {
                        if (fornecedorSelect.options.length <= 1) {
                            await GetEntidades(
                                fornecedorSelect,
                                "/Entidades/GetEntidades",
                                "FORNECEDORES",
                                { campoDescricao: "NomeRazaoSocial", apelido: "fo" }
                            );
                        }
                    });
                });

                // 🔹 intercepta setas para navegar dentro do select
                fornecedorSelect.addEventListener("keydown", function (e) {
                    const options = fornecedorSelect.options;
                    let index = fornecedorSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault(); // impede que o foco vá para o próximo campo
                        if (index < options.length - 1) {
                            fornecedorSelect.selectedIndex = index + 1;
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            fornecedorSelect.selectedIndex = index - 1;
                        }
                    }
                });

                // 🔹 ao selecionar fornecedor → foca no próximo campo (marca)
                fornecedorSelect.addEventListener("blur", function () {
                    if (fornecedorSelect.value && marcaSelect) {
//                        marcaSelect.focus();
                    }
                });
                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("marcaSelect")) {
                const marcaSelect = document.getElementById("marcaSelect");
                const modeloSelect = document.getElementById("modeloSelect");

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

                // 🔹 intercepta setas para navegar dentro do select
                marcaSelect.addEventListener("keydown", function (e) {
                    const options = marcaSelect.options;
                    let index = marcaSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault();
                        if (index < options.length - 1) {
                            marcaSelect.selectedIndex = index + 1;
//                            marcaSelect.dispatchEvent(new Event("change")); // força carregar modelos
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            marcaSelect.selectedIndex = index - 1;
//                            marcaSelect.dispatchEvent(new Event("change")); // força carregar modelos
                        }
                    }
                });

                // 🔹 ao selecionar marca → carrega modelos
                marcaSelect.addEventListener("blur", async function () {
                    if (marcaSelect.value && modeloSelect) {
                        modeloSelect.innerHTML = "<option value=''>Selecione o modelo</option>";

                        try {
                            const response = await fetch(`/Entidades/GetModelosPorMarca?idMarca=${marcaSelect.value}`);
                            const modelos = await response.json();

                            if (modelos.length === 0) {
                                // nenhum modelo encontrado → volta foco para marca
                                mostrarToast("Nenhum Modelo Cadastrado para esta Marca.", "aviso");
                                marcaSelect.focus();
                            }
                            else {

                                modelos.forEach(m => {
                                    const option = document.createElement("option");
                                    option.value = m.idModelo;
                                    option.text = m.descricao;
                                    modeloSelect.appendChild(option);
                                });

                                modeloSelect.focus();
                            }
                        } catch (error) {
                            mostrarToast("Erro ao Carregar Modelos.", "erro");
                            marcaSelect.focus();
                        }
                    }
                });

                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("modeloSelect")) {

                // 🔹 intercepta setas para navegar dentro do select
                modeloSelect.addEventListener("keydown", function (e) {
                    const options = modeloSelect.options;
                    let index = modeloSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault(); // impede que o foco vá para o próximo campo
                        if (index < options.length - 1) {
                            modeloSelect.selectedIndex = index + 1;
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            modeloSelect.selectedIndex = index - 1;
                        }
                    }
                });

                modeloSelect.addEventListener("blur", function () {
                    if (modeloSelect.value && unidadeSelect) {
//                        unidadeSelect.focus();

                        const descricaoProduto = document.getElementById("Descricao");
                        const selectedOption = modeloSelect.options[modeloSelect.selectedIndex];
                        if (descricaoProduto && selectedOption && selectedOption.text) {
                            descricaoProduto.value = selectedOption.text;
                        }
                    }
                });
                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else if (campo.classList.contains("unidadeSelect")) {
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

                // 🔹 intercepta setas para navegar dentro do select
                unidadeSelect.addEventListener("keydown", function (e) {
                    const options = unidadeSelect.options;
                    let index = unidadeSelect.selectedIndex;

                    if (e.key === "ArrowDown") {
                        e.preventDefault(); // impede que o foco vá para o próximo campo
                        if (index < options.length - 1) {
                            unidadeSelect.selectedIndex = index + 1;
                        }
                    } else if (e.key === "ArrowUp") {
                        e.preventDefault();
                        if (index > 0) {
                            unidadeSelect.selectedIndex = index - 1;
                        }
                    }
                });                

                // 🔹 ao selecionar fornecedor → foca no próximo campo (marca)
                unidadeSelect.addEventListener("blur", function () {
                    if (unidadeSelect.value && PrecoCompra) {
//                        PrecoCompra.focus();
//                        if (typeof PrecoCompra.select === "function") PrecoCompra.select();
                    }
                });
                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
            }
            else {
                ValidacaoUtils.validarCampo(campo, null, null, "", false, ehObrigatorio, form);
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