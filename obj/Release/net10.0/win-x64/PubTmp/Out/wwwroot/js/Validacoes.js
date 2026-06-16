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
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2) pos = 9;
    }
    let resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0)) return false;

    tamanho++;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (let i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2) pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    return resultado == digitos.charAt(1);
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

if ($.validator && $.validator.unobtrusive) {
    $.validator.addMethod("cpfcnpj", function (value, element, params) {
        if (!value) return true;

        value = value.replace(/\D/g, "");
        var tipoPessoaProp = params.tipopessoa;
        var tipoPessoa = $("[name='" + tipoPessoaProp + "']").val();

        if (tipoPessoa === "FíSICA") {
            return ValidaCpf(value);
        } else if (tipoPessoa === "JURíDICA") {
            return ValidaCnpj(value);
        }
        return false;
    });

    $.validator.unobtrusive.adapters.add("cpfcnpj", ["tipopessoa"], function (options) {
        options.rules["cpfcnpj"] = { tipopessoa: options.params.tipopessoa };
        options.messages["cpfcnpj"] = options.message;
    });
}
