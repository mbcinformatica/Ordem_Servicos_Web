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
                prefix: "R$ ",
                radixPoint: ",",
                groupSeparator: ".",
                autoGroup: true,
                digits: 2,
                digitsOptional: false,
                allowMinus: false,
                rightAlign: false,
                removeMaskOnSubmit: true
            });
            break;
        case "quantidade":
            // 🔹 Máscara para quantidade como número inteiro
            $(inputCampo).inputmask("9{1,}", {
                placeholder: "0",   // preenche com zero se vazio
                rightAlign: false,   // alinha à esquerda
                allowMinus: false    // não permite sinal de negativo
            });
            break;
    }
}

// Função genérica para retirar máscara
function semMascaraCampo(inputCampo) {
    if (!inputCampo) return;

    // remove máscara do Inputmask se existir
    if ($(inputCampo).inputmask) {
        $(inputCampo).inputmask("unmaskedvalue");
    }

    let valor = (inputCampo.value || "").trim();

    // se for campo de valor/preço → normaliza para formato do banco
    if (inputCampo.classList.contains("valor")) {

        // remove tudo que não for número, vírgula ou ponto
        let limpo = valor.replace(/[^0-9.,]/g, "");

        // converte vírgula para ponto
        limpo = limpo.replace(/,/g, ".");

        // mantém apenas o último ponto como separador decimal
        let lastDot = limpo.lastIndexOf(".");
        if (lastDot >= 0) {
            let inteiro = limpo.substring(0, lastDot).replace(/\./g, "");
            let decimalParte = limpo.substring(lastDot);
            limpo = inteiro + decimalParte;
        }

        valor = limpo;

    }
    else if (inputCampo.classList.contains("somentenumeros")) {
        // para CPF, CNPJ, telefone, etc. → só dígitos
        valor = valor.replace(/\D/g, "");
    }

    inputCampo.value = valor;
    return valor;
}

// Função para aplicar CPF ou CNPJ conforme seleção
function aplicarCpfCnpj(campo, tipoPessoa) {
    if (tipoPessoa === "FÍSICA") {
        aplicarMascaraCampo(campo, "cpf");
    } else {
        aplicarMascaraCampo(campo, "cnpj");
    }
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

// 🔹 Nova função: normalizar campos antes de enviar
// Função: normalizar campos antes de enviar
function normalizarCampos(form) {
    if (!form) return;

    const campos = form.querySelectorAll("input, textarea, select");

    campos.forEach(campo => {
        console.log("Campo: Antes", campo.name, "Valor:", campo.value);
        if (campo.classList.contains("maiusculo")) {
            campo.value = campo.value.trim().toUpperCase();
        }
        else if (campo.classList.contains("minusculo")) {
            campo.value = campo.value.trim().toLowerCase();
        }
        else if (campo.classList.contains("somentenumeros")) {
            campo.value = semMascaraCampo(campo);
        }
        console.log("Campo: Depois", campo.name, "Valor:", campo.value);
        // outros casos podem ser adicionados aqui
    });
}

// 🔹 Nova função: converte string hexadecimal para string numérica
function hexParaStringNumerica(hex) {
    let str = '';
    for (let i = 0; i < hex.length; i += 2) {
        str += String.fromCharCode(parseInt(hex.substr(i, 2), 16));
    }
    // 🔹 garante que só fiquem dígitos
    return str;
}