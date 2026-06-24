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

window.downloadTextFile = function (fileName, content, contentType) {
    var blob = new Blob([content || ""], { type: contentType || "text/plain;charset=utf-8" });
    var url = URL.createObjectURL(blob);
    var a = document.createElement("a");
    a.href = url;
    a.download = fileName || "download.txt";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
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
window.AppInterop = Object.assign(window.AppInterop || {}, {

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
    localStorageRemove: function (key) { try { localStorage.removeItem(key); } catch (e) { } },

    _googleMapsLoader: null,
    _googleMapsApiKey: null,
    _monitorMaps: {},

    ensureGoogleMaps: function (apiKey) {
        if (!apiKey) {
            return Promise.reject(new Error("ApiKey de Google Maps no configurada."));
        }

        if (window.google && window.google.maps && this._googleMapsApiKey === apiKey) {
            return Promise.resolve(window.google.maps);
        }

        if (this._googleMapsLoader && this._googleMapsApiKey === apiKey) {
            return this._googleMapsLoader;
        }

        this._googleMapsApiKey = apiKey;
        this._googleMapsLoader = new Promise(function (resolve, reject) {
            var previous = document.getElementById("google-maps-sdk");
            if (previous) {
                previous.remove();
            }

            window.__ventasSaasGoogleMapsReady = function () {
                resolve(window.google.maps);
            };

            var script = document.createElement("script");
            script.id = "google-maps-sdk";
            script.async = true;
            script.defer = true;
            script.src = "https://maps.googleapis.com/maps/api/js?key=" + encodeURIComponent(apiKey) + "&callback=__ventasSaasGoogleMapsReady";
            script.onerror = function () {
                reject(new Error("No se pudo cargar Google Maps."));
            };
            document.head.appendChild(script);
        }).catch(function (error) {
            window.AppInterop._googleMapsLoader = null;
            throw error;
        });

        return this._googleMapsLoader;
    },

    renderMonitorMap: async function (mapId, apiKey, payload) {
        var container = document.getElementById(mapId);
        if (!container) return;

        window.__monitorMapLastError = null;

        try {
            var maps = await this.ensureGoogleMaps(apiKey);
            var state = this._monitorMaps[mapId];

            if (!state) {
                state = {
                    map: new maps.Map(container, {
                        center: { lat: payload.centerLat || 16.75, lng: payload.centerLng || -93.12 },
                        zoom: payload.zoom || 13,
                        mapTypeControl: false,
                        streetViewControl: false,
                        fullscreenControl: true,
                        mapTypeId: "roadmap"
                    }),
                    markers: []
                };
                this._monitorMaps[mapId] = state;
            }

            state.markers.forEach(function (marker) { marker.setMap(null); });
            state.markers = [];

            var selectedId = payload.selectedId || 0;
            var dotNetRef = payload.dotNetRef || null;
            var selectedMarker = null;
            var bounds = new maps.LatLngBounds();

            (payload.markers || []).forEach(function (item) {
                if (typeof item.lat !== "number" || typeof item.lng !== "number") return;

                var iconUrl = selectedId === item.id
                    ? "https://maps.google.com/mapfiles/ms/icons/red-dot.png"
                    : item.status === "done"
                        ? "https://maps.google.com/mapfiles/ms/icons/green-dot.png"
                        : item.status === "alert"
                            ? "https://maps.google.com/mapfiles/ms/icons/yellow-dot.png"
                            : "https://maps.google.com/mapfiles/ms/icons/blue-dot.png";

                var marker = new maps.Marker({
                    map: state.map,
                    position: { lat: item.lat, lng: item.lng },
                    title: item.title || "",
                    label: String(item.label || ""),
                    icon: {
                        url: iconUrl,
                        labelOrigin: new maps.Point(16, 11)
                    },
                    zIndex: selectedId === item.id ? 999 : 10
                });

                if (dotNetRef && item.id) {
                    marker.addListener("click", function () {
                        dotNetRef.invokeMethodAsync(payload.clickMethod || "HandleMapMarkerClick", item.id);
                    });
                }

                state.markers.push(marker);
                bounds.extend(marker.getPosition());

                if (selectedId === item.id) {
                    selectedMarker = marker;
                }
            });

            if (selectedMarker) {
                state.map.panTo(selectedMarker.getPosition());
                state.map.setZoom(payload.selectedZoom || 17);
            } else if (state.markers.length === 1) {
                state.map.setCenter(bounds.getCenter());
                state.map.setZoom(payload.zoom || 16);
            } else if (state.markers.length > 1) {
                state.map.fitBounds(bounds, 48);
            } else {
                state.map.setCenter({ lat: payload.centerLat || 16.75, lng: payload.centerLng || -93.12 });
                state.map.setZoom(payload.zoom || 12);
            }
        } catch (error) {
            var detail = error && error.message ? error.message : "No se pudo inicializar el mapa.";
            window.__monitorMapLastError = detail;
            container.innerHTML =
                '<div style="height:100%;min-height:320px;display:flex;align-items:center;justify-content:center;padding:24px;text-align:center;color:#667085;background:#f8fafc;border-radius:12px;">'
                + '<div><strong style="display:block;color:#344054;margin-bottom:8px;">Error al cargar el mapa</strong>'
                + '<span>' + detail + '</span></div></div>';
            console.error("Monitor map error:", error);
        }
    },

    getMonitorMapLastError: function () {
        return window.__monitorMapLastError || "";
    }
});

// Aplicar tema guardado inmediatamente
AppInterop.applyStoredTheme();
