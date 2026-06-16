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

    if (!form || typeof form.addEventListener !== 'function') {
        return function () { };
    }

    var handler = function (e) {
        if (e.key !== "Enter") return;

        var target = e.target;
        if (!(target instanceof Element)) return;

        if (allowTextarea && target.tagName === "TEXTAREA") return;

        var type = target.getAttribute && target.getAttribute("type");
        if ((target.tagName === "BUTTON" && (!type || type.toLowerCase() === "submit")) ||
            (target.tagName === "INPUT" && type && type.toLowerCase() === "submit")) {
            return;
        }

        e.preventDefault();
        var focusables = FormUtils.getFocusableElements(form);
        var idx = focusables.indexOf(target);
        var next = (idx >= 0 && idx < focusables.length - 1) ? focusables[idx + 1] : null;
        if (next) {
            next.focus();
            if (typeof next.select === "function") next.select();
        }
    };

    form.addEventListener("keydown", handler, true);

    // retorna função para remover o listener
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
