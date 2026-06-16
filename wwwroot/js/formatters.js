// Função genérica para aplicar máscara
function aplicarMascara(valor, mascara) {
    valor = valor.replace(/\D/g, "");
    let resultado = "";
    let pos = 0;

    for (let i = 0; i < mascara.length; i++) {
        if (mascara[i] === "9") {
            if (valor[pos]) {
                resultado += valor[pos];
                pos++;
            } else {
                break;
            }
        } else {
            resultado += mascara[i];
        }
    }
    return resultado;
}
// Aceita apenas números
function apenasNumeros(input) {
    input.value = input.value.replace(/\D/g, "");
}
// Valor monetário brasileiro
function formatValor(valor) {
    if (!valor) return "";
    valor = valor.toString().replace(/\D/g, "");
    let numero = (parseFloat(valor) / 100).toFixed(2);
    return new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL"
    }).format(numero);
}
// Quantidade com separador de milhar
function formatQuantidade(valor) {
    if (!valor) return "";
    valor = valor.toString().replace(/\D/g, "");
    return new Intl.NumberFormat("pt-BR").format(valor);
}
// CPF ou CNPJ
function formatCpfCnpj(valor, tipoPessoa) {
    valor = valor.replace(/\D/g, "");
    return tipoPessoa === "JURÍDICA"
        ? aplicarMascara(valor, "99.999.999/9999-99")
        : aplicarMascara(valor, "999.999.999-99");
}
// Telefone
function formatTelefone(valor) {
    valor = valor.replace(/\D/g, "");
    return valor.length > 10
        ? aplicarMascara(valor, "(99) 99999-9999")
        : aplicarMascara(valor, "(99) 9999-9999");
}
// CEP
function formatCep(valor) {
    valor = valor.replace(/\D/g, "");
    return aplicarMascara(valor, "99999-999");
}
// Email minúsculo
function converteParaMinusculo(valor) {
    if (!valor) return "";
    return valor.trim().toLowerCase();
}
// Função Valida CPF/CNPJ
function validarCpfCnpj(input, tipoPessoa) {
    let valor = input.value.replace(/\D/g, "");
    if (tipoPessoa === "JURÍDICA" && valor.length !== 14) {
        mostrarToast("CNPJ inválido. Deve conter 14 dígitos.", "erro");
        return false;
    } else if (tipoPessoa === "FÍSICA" && valor.length !== 11) {
        mostrarToast("CPF inválido. Deve conter 11 dígitos.", "erro");
        return false;
    }
    limparErro(input);
    return true;
}
// Função Valida Telefone
function validarTelefone(input) {
    let valor = input.value.replace(/\D/g, "");
    if (valor.length !== 10 && valor.length !== 11) {
        mostrarToast("Telefone inválido. Deve conter 10 ou 11 dígitos.", "erro");
        return false;
    }
    limparErro(input);
    return true;
}
// Função Valida CEP
function validarCep(input) {
    let valor = input.value.replace(/\D/g, "");
    if (valor.length !== 8) {
        mostrarToast("CEP inválido. Deve conter 8 dígitos.", "erro");
        return false;
    }
    limparErro(input);
    return true;
}
// Função Valida e-MAIL
function validarEmail(input) {
    let valor = input.value.trim();
    let regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!regex.test(valor)) {
        mostrarToast("E-mail inválido. Informe no formato usuario@dominio.com.", "erro");
        return false;
    }
    limparErro(input);
    return true;
}
// Função global para remover formatação
function SemFormatacao(valor) {
    if (!valor) return "";

    // Remove caracteres indesejados: (), ., -, /, R, $, espaços
    let limpo = valor.replace(/[()\.\-\/R\$\s]/g, "").trim();

    // Se for valor monetário com vírgula de centavos
    if (valor.includes(",")) {
        limpo = valor.replace(/[R\$\s]/g, "") // remove R$, espaços
            .replace(/\./g, "")      // remove separador de milhar
            .replace(",", ".");      // troca vírgula por ponto
    }
    return limpo;
}
// Mensagem inline (opcional)
function limparErro(input) {
    let span = input.nextElementSibling;
    if (span && span.classList.contains("text-danger")) {
        span.textContent = "";
    }
}
// Função que Mostra Mensagens
function mostrarToast(texto, tipo) {
    const toastContainer = document.querySelector(".mensagem-container");
    const toastEl = document.getElementById("toastMensagem");
    const toastBody = document.getElementById("toastTexto");

    if (!toastEl || !toastBody || !toastContainer) return;

    toastBody.innerText = texto;

    // Remove classes antigas
    toastEl.className = "mensagem-custom d-inline-flex align-items-center";

    // Aplica estilo conforme tipo
    if (tipo === "erro") {
        toastEl.classList.add("mensagem-erro");
    } else if (tipo === "aviso") {
        toastEl.classList.add("mensagem-aviso");
    } else if (tipo === "sucesso") {
        toastEl.classList.add("mensagem-sucesso");
    }

    // Exibe o container
    toastContainer.style.display = "flex";

    const toast = new bootstrap.Toast(toastEl, { delay: 4000 });
    toast.show();
}
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
                    mostrarToast("CEP encontrado e preenchido.", "sucesso");
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
function buscarCNPJ(cnpjInputId, tipoPessoaId, nomeId, enderecoId, numeroId, bairroId, municipioId, ufId, cepId, celularId, fixoId, emailId, contatoId) {
    const cnpj = document.getElementById(cnpjInputId).value.replace(/\D/g, "");
    const tipoPessoa = document.getElementById(tipoPessoaId)?.value || "";

    if (tipoPessoa.toUpperCase() === "JURÍDICA") {
        if (cnpj.length === 14) {
            fetch(`/Clientes/BuscarDadosPorCnpj?cnpj=${cnpj}`)
                .then(response => response.json())
                .then(data => {
                    if (data && data.nome) {
                        if (nomeId) document.getElementById(nomeId).value = data.nome || "";
                        if (enderecoId) document.getElementById(enderecoId).value = data.logradouro || "";
                        if (numeroId) document.getElementById(numeroId).value = data.numero || "";
                        if (bairroId) document.getElementById(bairroId).value = data.bairro || "";
                        if (municipioId) document.getElementById(municipioId).value = data.municipio || "";
                        if (ufId) document.getElementById(ufId).value = data.uf || "";   // alguns retornos usam "uf"
                        if (cepId) document.getElementById(cepId).value = data.cep || "";
                        if (celularId) document.getElementById(celularId).value = data.telefone || "";
                        if (fixoId) document.getElementById(fixoId).value = data.telefone || "";
                        if (emailId) document.getElementById(emailId).value = data.email || "";
                        if (contatoId) document.getElementById(contatoId).value = data.telefone || "";

                        mostrarToast("CNPJ encontrado e preenchido.", "sucesso");
                    } else {
                        mostrarToast("CNPJ não encontrado.", "aviso");
                    }
                })
                .catch(() => {
                    mostrarToast("Erro ao consultar CNPJ.", "erro");
                });
        } else if (cnpj.length > 0) {
            mostrarToast("CNPJ inválido. Deve conter 14 dígitos.", "erro");
        }
    }
}
