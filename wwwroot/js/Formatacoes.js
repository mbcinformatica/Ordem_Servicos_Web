// Função genérica para aplicar máscara
function aplicarMascaraCampo(inputCampo, tipo) {
    if (!inputCampo) return;
    switch (tipo.toLowerCase()) {
        case "cpf":
            $(inputCampo).inputmask("999.999.999-99");
            break;
        case "cnpj":
            $(inputCampo).inputmask("99.999.999/9999-99");
            break;
        case "cep":
            $(inputCampo).inputmask("99999-999");
            break;
        case "telefone":
            $(inputCampo).inputmask({
                mask: ["(99) 9999-9999", "(99) 99999-9999"],
                keepStatic: true
            });
            break;
        case "valor":
            $(inputCampo).inputmask("currency", {
                radixPoint: ",",
                groupSeparator: ".",
                digits: 2,
                autoGroup: true,
                prefix: "R$ ",
                rightAlign: false
            });
            break;
        case "quantidade":
            $(inputCampo).inputmask("integer", {
                groupSeparator: ".",
                autoGroup: true,
                rightAlign: false
            });
            break;
    }
}

// Função genérica para retirar máscara
function semMascaraCampo(inputCampo) {
    if (!inputCampo) return;

    // remove máscara do Inputmask se existir
    if ($(inputCampo).inputmask) {
        $(inputCampo).inputmask("remove");
    }

    let valor = (inputCampo.value || "").trim();

    // se for campo de valor/preço → normaliza para formato do banco
    if (inputCampo.classList.contains("valor")) {

        valor = valor.replace(/[R\$\s]/g, "") // remove R$, espaços
            .replace(/\./g, "")      // remove separador de milhar
            .replace(",", ".");      // troca vírgula por ponto
    } else {
        // para CPF, CNPJ, telefone, etc. → só dígitos
        valor = valor.replace(/[()\.\-\/R\$\s]/g, "").trim();
    }

    inputCampo.value = valor;
}

// Função para aplicar CPF ou CNPJ conforme seleção
function aplicarCpfCnpj(campo, tipoPessoa) {
    if (tipoPessoa === "FÍSICA") {
        aplicarMascaraCampo(campo, "cpf");
    } else {
        aplicarMascaraCampo(campo, "cnpj");
    }
}

// Função genérica para converter campos em maiúsculo
function aplicarMaiusculoCampos(seletor, evento = "blur") {
    const campos = document.querySelectorAll(seletor);

    campos.forEach(campo => {
        campo.addEventListener(evento, function () {
            this.value = this.value.trim().toUpperCase();
        });
    });
}

// Função genérica para converter campos em minusculo
function aplicarMinusculoCampos(seletor, evento = "blur") {
    const campos = document.querySelectorAll(seletor);

    campos.forEach(campo => {
        campo.addEventListener(evento, function () {
            this.value = this.value.trim().toLowerCase();
        });
    });
}

// Função para converter texto para minúsculo, removendo espaços extras
function converteParaMinusculo(valor) {
    if (!valor) return "";
    return valor.trim().toLowerCase();
}

// Função para limpar mensagens de erro associadas a um campo de input
function limparErro(input) {
    let span = input.nextElementSibling;
    if (span && span.classList.contains("text-danger")) {
        span.textContent = "";
    }
}

// Função para mostrar mensagens de toast usando Bootstrap, com tipos de erro, aviso e sucesso
function mostrarToast(texto, tipo) {
    const toastEl = document.getElementById("toastMensagem");
    const toastBody = document.getElementById("toastTexto");
    const toastIcon = document.getElementById("toastIcon");

    if (!toastEl || !toastBody || !toastIcon) return;

    toastBody.innerText = texto;


    // Reset classes
    toastEl.className = "toast mensagem-custom align-items-center border-0 toast-anim-slide";
    toastIcon.className = "me-2 ms-3 fs-4 m-auto";

    // Aplica cor e ícone conforme tipo
    if (tipo === "erro") {
        toastEl.classList.add("text-bg-danger");
        toastIcon.innerHTML = '<i class="fas fa-times-circle"></i>';
    } else if (tipo === "aviso") {
        toastEl.classList.add("text-bg-warning");
        toastIcon.innerHTML = '<i class="fas fa-exclamation-triangle"></i>';
    } else if (tipo === "sucesso") {
        toastEl.classList.add("text-bg-success");
        toastIcon.innerHTML = '<i class="fas fa-check-circle"></i>';
    } else {
        toastEl.classList.add("text-bg-info");
        toastIcon.innerHTML = '<i class="fas fa-info-circle"></i>';
    }

    // Inicializa toast Bootstrap com timeout de 4s
    const toast = new bootstrap.Toast(toastEl, { delay: 5000 });
    toast.show();
}