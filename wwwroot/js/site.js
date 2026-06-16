// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Utilitários de formulário reutilizáveis
window.FormUtils = window.FormUtils || {};

/**
 * Retorna elementos focoáveis dentro de um form, filtrando hidden, disabled e invisíveis.
 * @param {HTMLFormElement|Element} frm
 * @returns {HTMLElement[]}
 */
FormUtils.getFocusableElements = function (frm) {
    if (!frm || typeof frm.querySelectorAll !== 'function') return [];
    return Array.from(
        frm.querySelectorAll(
            'input:not([type=hidden]):not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), a[href], [tabindex]:not([tabindex="-1"])'
        )
    ).filter(el => el.offsetParent !== null);
};

/**
 * Habilita navegação por Enter dentro de um formulário.
 * Prevê submit indesejado, avança para o próximo elemento focoável e chama select() quando disponível.
 * Retorna uma função de cleanup para remover o listener.
 *
 * @param {HTMLFormElement} form
 * @param {Object} [options]
 * @param {boolean} [options.allowTextarea=true] se true, não intercepta Enter em TEXTAREA
 * @returns {Function} função para desregistrar o handler
 */
FormUtils.enableEnterNavigation = function (form, options) {
    options = options || {};
    var allowTextarea = options.allowTextarea !== undefined ? options.allowTextarea : true;
    var submitOnLastField = options.submitOnLastField === true; // novo parâmetro

    if (!form || typeof form.addEventListener !== 'function') {
        return function () { };
    }

    var handler = async function (e) {
        if (e.key !== "Enter") return;
        e.preventDefault();

        var target = e.target;
        if (!(target instanceof Element)) return;

        // não intercepta Enter em textarea
        if (allowTextarea && target.tagName === "TEXTAREA") return;

        // deixa submit normal em botões
        var type = target.getAttribute && target.getAttribute("type");
        if ((target.tagName === "BUTTON" && (!type || type.toLowerCase() === "submit")) ||
            (target.tagName === "INPUT" && type && type.toLowerCase() === "submit")) {
            form.submit();
            return;
        }

        var focusables = FormUtils.getFocusableElements(form);
        var idx = focusables.indexOf(target);

        // 🔹 roda validação genérica (local + duplicidade se existir)
        var valido = true;
        if (typeof target._executarValidacao === "function") {
            valido = await target._executarValidacao();
        }
        if (valido) {
            // se último campo e opção ativa → envia
            if (submitOnLastField && idx === focusables.length - 1) {
                form.submit();
                return;
            }

            // avança para próximo campo
            var next = (idx >= 0 && idx < focusables.length - 1) ? focusables[idx + 1] : null;
            if (next) {
                next.focus();
                if (typeof next.select === "function") next.select();
            }
        } else {
            // mantém foco no campo inválido
            target.classList.add("Invalid");
            target.focus();
        }
    };

    form.addEventListener("keydown", handler, true);

    return function () {
        form.removeEventListener("keydown", handler, true);
    };
};

// Foco automático no primeiro campo obrigatório do formulário
window.addEventListener("load", function () {
    const primeiroCampo = document.querySelector("form input[required], form select[required], form textarea[required]");
    if (primeiroCampo) {
        primeiroCampo.focus();
        if (typeof primeiroCampo.select === 'function') {
            primeiroCampo.select();
        }
    }
});
