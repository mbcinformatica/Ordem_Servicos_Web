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
    if (resultado === "(") {
        resultado = "";
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