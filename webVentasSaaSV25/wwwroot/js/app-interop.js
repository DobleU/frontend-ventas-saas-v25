/**
 * app-interop.js — VentasSaaSDU
 * Puente entre Blazor WebAssembly y los plugins JS del template.
 *
 * IMPORTANTE: Este archivo se carga ANTES de blazor.webassembly.js pero
 * DESPUÉS de jQuery y los plugins del template.
 *
 * Problema conocido: main.js del template ejecuta PerfectScrollbar en
 * $(document).ready() sobre selectores que no existen en todas las páginas
 * (ej: Login no tiene sidebar). Esto genera el error:
 *   "Cannot read properties of null (reading 'classList')"
 *
 * Solución: parchamos PerfectScrollbar para que falle silenciosamente
 * cuando el elemento no existe, ANTES de que main.js corra su ready().
 */

// ─── PATCH: PerfectScrollbar defensivo ────────────────────────────────────────
// Envuelve el constructor original para ignorar elementos null/inexistentes.
(function () {
    if (typeof PerfectScrollbar === "undefined") return;

    var OriginalPS = PerfectScrollbar;
    window.PerfectScrollbar = function (el, options) {
        // Si el selector es string, resolver el elemento primero
        var target = typeof el === "string" ? document.querySelector(el) : el;
        // Solo inicializar si el elemento existe en el DOM actual
        if (!target) return { destroy: function () { } };
        try {
            return new OriginalPS(target, options || {});
        } catch (e) {
            console.warn("[VentasSaaSDU] PerfectScrollbar ignorado en:", el, e.message);
            return { destroy: function () { } };
        }
    };
    // Copiar propiedades estáticas del constructor original
    Object.assign(window.PerfectScrollbar, OriginalPS);
})();

// ─── PATCH: jQuery .ready() defensivo para plugins del template ───────────────
// main.js llama selectores que solo existen en el layout principal (no en Login).
// Este guard hace que esos intentos fallen silenciosamente.
(function ($) {
    if (typeof $ === "undefined") return;

    var originalScrollbar = $.fn.perfectScrollbar;
    if (originalScrollbar) {
        $.fn.perfectScrollbar = function () {
            if (!this.length) return this; // sin elementos → no hacer nada
            try { return originalScrollbar.apply(this, arguments); }
            catch (e) { console.warn("[VentasSaaSDU] perfectScrollbar ignorado:", e.message); return this; }
        };
    }
})(window.jQuery || window.$);

// ─── AppInterop ───────────────────────────────────────────────────────────────
window.AppInterop = {

    /**
     * Inicializa plugins del layout principal.
     * Llamar desde MainLayout.razor en OnAfterRenderAsync(firstRender=true).
     * NO llamar desde AuthLayout (Login) — los elementos no existen ahí.
     */
    initPlugins: function () {
        // MetisMenu — solo si el elemento existe
        var metisEl = document.querySelector(".navbar-nav");
        if (metisEl && typeof $.fn !== "undefined" && typeof $.fn.metisMenu === "function") {
            try { $(metisEl).metisMenu(); } catch (e) { }
        }

        // SimpleBar — solo sobre elementos marcados
        if (typeof SimpleBar !== "undefined") {
            document.querySelectorAll("[data-simplebar]").forEach(function (el) {
                try { new SimpleBar(el); } catch (e) { }
            });
        }
    },

    /**
     * Inicializa PerfectScrollbar en un selector dado.
     * @param {string} selector - selector CSS del contenedor
     */
    initPerfectScrollbar: function (selector) {
        var el = document.querySelector(selector);
        if (el && typeof PerfectScrollbar !== "undefined") {
            try { new PerfectScrollbar(el); } catch (e) { }
        }
    },

    setTheme: function (theme) {
        document.documentElement.setAttribute("data-bs-theme", theme || "semi-dark");
        localStorage.setItem("ventassaas_theme", theme || "semi-dark");
    },

    getTheme: function () {
        return localStorage.getItem("ventassaas_theme") || "semi-dark";
    },

    applyStoredTheme: function () {
        var theme = localStorage.getItem("ventassaas_theme") || "semi-dark";
        document.documentElement.setAttribute("data-bs-theme", theme);
    },

    /**
     * Toggle show/hide password sin jQuery.
     * @param {string} inputId - id del input de password
     * @param {string} iconId  - id del icono <i>
     */
    togglePasswordVisibility: function (inputId, iconId) {
        var input = document.getElementById(inputId);
        var icon = document.getElementById(iconId);
        if (!input) return;
        if (input.type === "password") {
            input.type = "text";
            if (icon) { icon.classList.remove("bi-eye-slash-fill"); icon.classList.add("bi-eye-fill"); }
        } else {
            input.type = "password";
            if (icon) { icon.classList.remove("bi-eye-fill"); icon.classList.add("bi-eye-slash-fill"); }
        }
    },

    /** Scroll al top */
    scrollToTop: function () {
        window.scrollTo({ top: 0, behavior: "smooth" });
    },

    /** Cierra un offcanvas Bootstrap por ID */
    closeOffcanvas: function (id) {
        var el = document.getElementById(id);
        if (el && typeof bootstrap !== "undefined") {
            var instance = bootstrap.Offcanvas.getInstance(el);
            if (instance) instance.hide();
        }
    },

    // ─── localStorage helpers ────────────────────────────────────────────────
    localStorageGet: function (key) {
        try { return localStorage.getItem(key); } catch (e) { return null; }
    },
    localStorageSet: function (key, value) {
        try { localStorage.setItem(key, value); } catch (e) { }
    },
    localStorageRemove: function (key) {
        try { localStorage.removeItem(key); } catch (e) { }
    }
};

// Aplicar tema guardado inmediatamente (antes de que Blazor monte el DOM)
AppInterop.applyStoredTheme();