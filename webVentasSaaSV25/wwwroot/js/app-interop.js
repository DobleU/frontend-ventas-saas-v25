/**
 * app-interop.js — VentasSaaSDU
 */

// ─── PATCH: PerfectScrollbar defensivo ───────────────────────────────────────
(function () {
    if (typeof PerfectScrollbar === "undefined") return;
    var OriginalPS = PerfectScrollbar;
    window.PerfectScrollbar = function (el, options) {
        var target = typeof el === "string" ? document.querySelector(el) : el;
        if (!target) return { destroy: function () { } };
        try { return new OriginalPS(target, options || {}); }
        catch (e) { return { destroy: function () { } }; }
    };
    Object.assign(window.PerfectScrollbar, OriginalPS);
})();

// ─── PATCH: jQuery perfectScrollbar defensivo ────────────────────────────────
(function ($) {
    if (typeof $ === "undefined") return;
    var orig = $.fn.perfectScrollbar;
    if (orig) {
        $.fn.perfectScrollbar = function () {
            if (!this.length) return this;
            try { return orig.apply(this, arguments); } catch (e) { return this; }
        };
    }
})(window.jQuery || window.$);

// ─── NavMenu: toggle de dropdowns sin Bootstrap JS ───────────────────────────
window.NavMenu = {
    toggle: function (id) {
        var items = document.querySelectorAll('.primary-menu .nav-item.dropdown');
        items.forEach(function (li) {
            if (li.dataset.menuId === id) {
                var isOpen = li.classList.contains('menu-open');
                li.classList.remove('menu-open');
                if (!isOpen) {
                    li.classList.add('menu-open');
                    var trigger = li.querySelector('a');
                    var rect = trigger.getBoundingClientRect();
                    var menu = li.querySelector('.dropdown-menu');
                    menu.style.top = rect.bottom + 'px';
                    menu.style.left = rect.left + 'px';
                    setTimeout(function () {
                        var mRect = menu.getBoundingClientRect();
                        if (mRect.right > window.innerWidth - 10) {
                            menu.style.left = (window.innerWidth - mRect.width - 10) + 'px';
                        }
                    }, 0);
                }
            } else {
                li.classList.remove('menu-open');
            }
        });
    },
    closeAll: function () {
        document.querySelectorAll('.primary-menu .nav-item.dropdown').forEach(function (li) {
            li.classList.remove('menu-open');
        });
    }
};

// Cerrar NavMenu al hacer click fuera
document.addEventListener('click', function (e) {
    if (!e.target.closest('.primary-menu')) {
        window.NavMenu.closeAll();
    }
});

// Cerrar NavMenu al navegar desde una opcion del menu.
document.addEventListener('click', function (e) {
    var link = e.target.closest('.primary-menu .dropdown-menu a[href]');
    if (!link) return;

    var href = (link.getAttribute('href') || '').trim();
    if (!href || href === '#' || href.toLowerCase().startsWith('javascript:')) return;

    window.NavMenu.closeAll();
});

// ─── GridMenu: toggle de acciones ⋯ en cards, widgets y grids ───────────────
window.GridMenu = {
    toggle: function (btn, ev) {
        if (ev) {
            ev.preventDefault();
            ev.stopPropagation();
        } else if (window.event) {
            window.event.preventDefault && window.event.preventDefault();
            window.event.stopPropagation && window.event.stopPropagation();
        }

        var wrapper = btn.closest('.grid-action-dropdown') || btn.parentElement;
        if (!wrapper) return false;

        var menu = wrapper.querySelector('.grid-action-menu, .dropdown-menu');
        if (!menu && wrapper.__gridMenuPortal) {
            menu = wrapper.__gridMenuPortal;
        }
        if (!menu) return false;

        var wasOpen = menu.classList.contains('show') || menu.classList.contains('menu-open');

        if (wasOpen) {
            window.GridMenu.closeAll();
            return false;
        }

        window.GridMenu.closeAll(menu);

        {
            if (!wrapper.__gridMenuPortal) {
                wrapper.__gridMenuPortal = menu;
                wrapper.__gridMenuNextSibling = menu.nextSibling;
                wrapper.__gridMenuParent = menu.parentElement;
            }

            menu.__gridMenuWrapper = wrapper;
            document.body.appendChild(menu);

            menu.classList.add('show');
            menu.classList.add('menu-open');
            menu.setAttribute('data-bs-popper', 'static');
            menu.setAttribute('data-grid-menu-open', 'true');

            // Se fuerza por JS para no depender de que el CSS cacheado ya esté actualizado.
            menu.style.setProperty('display', 'block', 'important');
            menu.style.setProperty('position', 'fixed', 'important');
            menu.style.setProperty('z-index', '99999', 'important');
            menu.style.setProperty('min-width', '220px', 'important');
            menu.style.setProperty('max-width', '320px', 'important');
            menu.style.setProperty('visibility', 'visible', 'important');
            menu.style.setProperty('opacity', '1', 'important');
            menu.style.setProperty('transform', 'none', 'important');

            var rect = btn.getBoundingClientRect();
            var menuWidth = menu.offsetWidth || 240;
            var menuHeight = menu.offsetHeight || 160;
            var left = rect.right - menuWidth;
            var top = rect.bottom + 6;

            if (left < 10) left = 10;
            if (left + menuWidth > window.innerWidth - 10) left = window.innerWidth - menuWidth - 10;
            if (top + menuHeight > window.innerHeight - 10) {
                top = rect.top - menuHeight - 6;
            }
            if (top < 10) top = 10;

            menu.style.top = top + 'px';
            menu.style.left = left + 'px';
            menu.style.right = 'auto';

            setTimeout(function () {
                var mRect = menu.getBoundingClientRect();
                if (mRect.bottom > window.innerHeight - 10) {
                    var topAbove = rect.top - mRect.height - 6;
                    menu.style.top = Math.max(10, topAbove) + 'px';
                }
            }, 0);
        }

        return false;
    },

    closeAll: function (exceptMenu) {
        document.querySelectorAll('.grid-action-menu.show, .grid-action-menu.menu-open, .grid-action-dropdown .dropdown-menu.show, .grid-action-dropdown .dropdown-menu.menu-open').forEach(function (m) {
            if (exceptMenu && m === exceptMenu) return;
            m.classList.remove('show');
            m.classList.remove('menu-open');
            m.removeAttribute('data-bs-popper');
            m.removeAttribute('data-grid-menu-open');
            m.style.removeProperty('display');
            m.style.removeProperty('position');
            m.style.removeProperty('z-index');
            m.style.removeProperty('min-width');
            m.style.removeProperty('max-width');
            m.style.removeProperty('visibility');
            m.style.removeProperty('opacity');
            m.style.removeProperty('transform');
            m.style.removeProperty('top');
            m.style.removeProperty('left');
            m.style.removeProperty('right');

            var wrapper = m.__gridMenuWrapper;
            if (wrapper && wrapper.__gridMenuParent) {
                if (wrapper.__gridMenuNextSibling && wrapper.__gridMenuNextSibling.parentNode === wrapper.__gridMenuParent) {
                    wrapper.__gridMenuParent.insertBefore(m, wrapper.__gridMenuNextSibling);
                } else {
                    wrapper.__gridMenuParent.appendChild(m);
                }
            }
            m.__gridMenuWrapper = null;
        });
    }
};

// Cerrar GridMenu al hacer click fuera. Se usa capture=false para permitir stopPropagation en el botón.
document.addEventListener('click', function (e) {
    if (!e.target.closest('.grid-action-dropdown') && !e.target.closest('.grid-action-menu')) {
        window.GridMenu.closeAll();
    } else if (e.target.closest('.grid-action-menu .dropdown-item')) {
        setTimeout(function () { window.GridMenu.closeAll(); }, 50);
    }
});

// ─── AppNavbar: toggle móvil ──────────────────────────────────────────────────
window.AppNavbar = {
    toggle: function (btn) {
        var target = btn.closest('.navbar').querySelector('.navbar-collapse');
        if (target) target.classList.toggle('show');
    }
};

window.printTicketHtml = function (html) {
    try {
        var w = window.open('', '_blank', 'width=900,height=900');
        if (w && w.document) {
            w.document.open();
            w.document.write(html);
            w.document.close();
            w.focus && w.focus();
            return;
        }
    } catch (err) {
        // Fallback below.
    }

    var blob = new Blob([html], { type: 'text/html;charset=utf-8' });
    var url = URL.createObjectURL(blob);
    var fallback = window.open(url, '_blank');
    if (!fallback) {
        window.location.href = url;
    }
};

// ─── AppInterop ──────────────────────────────────────────────────────────────
window.AppInterop = {

    initPlugins: function () {
        var metisEl = document.querySelector(".navbar-nav");
        if (metisEl && typeof $.fn !== "undefined" && typeof $.fn.metisMenu === "function") {
            try { $(metisEl).metisMenu(); } catch (e) { }
        }
        if (typeof SimpleBar !== "undefined") {
            document.querySelectorAll("[data-simplebar]").forEach(function (el) {
                try { new SimpleBar(el); } catch (e) { }
            });
        }
    },

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

    scrollToTop: function () { window.scrollTo({ top: 0, behavior: "smooth" }); },

    closeOffcanvas: function (id) {
        var el = document.getElementById(id);
        if (el && typeof bootstrap !== "undefined") {
            var instance = bootstrap.Offcanvas.getInstance(el);
            if (instance) instance.hide();
        }
    },

    localStorageGet: function (key) { try { return localStorage.getItem(key); } catch (e) { return null; } },
    localStorageSet: function (key, value) { try { localStorage.setItem(key, value); } catch (e) { } },
    localStorageRemove: function (key) { try { localStorage.removeItem(key); } catch (e) { } }
};

// Aplicar tema guardado inmediatamente
AppInterop.applyStoredTheme();
