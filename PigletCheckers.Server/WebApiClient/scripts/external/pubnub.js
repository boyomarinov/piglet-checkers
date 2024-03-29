﻿(function () {
    window.JSON && window.JSON.stringify || function () {
        function y(s) { F.lastIndex = 0; return F.test(s) ? '"' + s.replace(F, function (t) { var j = J[t]; return typeof j === "string" ? j : "\\u" + ("0000" + t.charCodeAt(0).toString(16)).slice(-4) }) + '"' : '"' + s + '"' } function w(s, t) {
            var j, q, n, z, A = m, o, g = t[s]; if (g && typeof g === "object" && typeof g.toJSON === "function") g = g.toJSON(s); if (typeof p === "function") g = p.call(t, s, g); switch (typeof g) {
                case "string": return y(g); case "number": return isFinite(g) ? String(g) : "null"; case "boolean": case "null": return String(g);
                case "object": if (!g) return "null"; m += r; o = []; if (Object.prototype.toString.apply(g) === "[object Array]") { z = g.length; for (j = 0; j < z; j += 1) o[j] = w(j, g) || "null"; n = o.length === 0 ? "[]" : m ? "[\n" + m + o.join(",\n" + m) + "\n" + A + "]" : "[" + o.join(",") + "]"; m = A; return n } if (p && typeof p === "object") { z = p.length; for (j = 0; j < z; j += 1) { q = p[j]; if (typeof q === "string") if (n = w(q, g)) o.push(y(q) + (m ? ": " : ":") + n) } } else for (q in g) if (Object.hasOwnProperty.call(g, q)) if (n = w(q, g)) o.push(y(q) + (m ? ": " : ":") + n); n = o.length === 0 ? "{}" : m ? "{\n" + m + o.join(",\n" + m) +
                "\n" + A + "}" : "{" + o.join(",") + "}"; m = A; return n
            }
        } window.JSON || (window.JSON = {}); if (typeof String.prototype.toJSON !== "function") String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function () { return this.valueOf() }; var F = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, m, r, J = { "\u0008": "\\b", "\t": "\\t", "\n": "\\n", "\u000c": "\\f", "\r": "\\r", '"': '\\"', "\\": "\\\\" }, p; if (typeof JSON.stringify !== "function") JSON.stringify =
        function (s, t, j) { var q; r = m = ""; if (typeof j === "number") for (q = 0; q < j; q += 1) r += " "; else if (typeof j === "string") r = j; if ((p = t) && typeof t !== "function" && (typeof t !== "object" || typeof t.length !== "number")) throw Error("JSON.stringify"); return w("", { "": s }) }; if (typeof JSON.parse !== "function") JSON.parse = function (s) { return eval("(" + s + ")") }
    }();
    window.PUBNUB || function () {
        function y() { return "x" + ++X + "" + +new Date } function w() { return +new Date } function F(b, a) { var c, f = 0, k = function () { if (f + a > w()) { clearTimeout(c); c = setTimeout(k, a) } else { f = w(); b() } }; return k } function m(b) { return document.getElementById(b) } function r(b) { console.log(b) } function J(b, a) { var c = []; p(b.split(/\s+/), function (f) { p((a || document).getElementsByTagName(f), function (k) { c.push(k) }) }); return c } function p(b, a) {
            if (b && a) if (typeof b[0] != "undefined") for (var c = 0, f = b.length; c < f;) a.call(b[c],
            b[c], c++); else for (c in b) b.hasOwnProperty && b.hasOwnProperty(c) && a.call(b[c], c, b[c])
        } function s(b, a) { var c = []; p(b || [], function (f, k) { c.push(a(f, k)) }); return c } function t(b, a) { return b.replace(Y, function (c, f) { return a[f] || c }) } function j(b, a, c) {
            p(b.split(","), function (f) {
                var k = function (l) { if (!l) l = window.event; if (!c(l)) { l.cancelBubble = true; l.returnValue = false; l.preventDefault && l.preventDefault(); l.stopPropagation && l.stopPropagation() } }; if (a.addEventListener) a.addEventListener(f, k, false); else if (a.attachEvent) a.attachEvent("on" +
                f, k); else a["on" + f] = k
            })
        } function q() { return J("head")[0] } function n(b, a, c) { if (c) b.setAttribute(a, c); else return b && b.getAttribute && b.getAttribute(a) } function z(b, a) { for (var c in a) if (a.hasOwnProperty(c)) try { b.style[c] = a[c] + ("|width|height|top|left|".indexOf(c) > 0 && typeof a[c] == "number" ? "px" : "") } catch (f) { } } function A(b) { return document.createElement(b) } function o() { return K || x() ? 0 : y() } function g(b) { return s(encodeURIComponent(b).split(""), function (a) { return "-_.!~*'()".indexOf(a) < 0 ? a : "%" + a.charCodeAt(0).toString(16).toUpperCase() }).join("") }
        function B(b) { if (K || x()) return Z(b); var a = A("script"), c = b.callback, f = y(), k = 0, l = setTimeout(function () { e(1) }, L), C = b.fail || function () { }, d = b.success || function () { }, e = function (h, i) { if (!k) { k = 1; h || d(i); a.onerror = null; clearTimeout(l); setTimeout(function () { h && C(); var u = m(f), v = u && u.parentNode; v && v.removeChild(u) }, G) } }; window[c] = function (h) { e(0, h) }; a[P] = P; a.onerror = function () { e(1) }; a.src = b.url.join(Q); n(a, "id", f); q().appendChild(a); return e } function Z(b) {
            var a, c = function () {
                if (!k) {
                    k = 1; clearTimeout(l); try {
                        response =
                        JSON.parse(a.responseText)
                    } catch (i) { return e(1) } d(response)
                }
            }, f = 0, k = 0, l = setTimeout(function () { e(1) }, L), C = b.fail || function () { }, d = b.success || function () { }, e = function (i) { if (!f) { f = 1; clearTimeout(l); if (a) { a.onerror = a.onload = null; a.abort && a.abort(); a = null } i && C() } }; try { a = x() || window.XDomainRequest && new XDomainRequest || new XMLHttpRequest; a.onerror = a.onabort = function () { e(1) }; a.onload = a.onloadend = c; a.timeout = L; a.open("GET", b.url.join(Q), true); a.send() } catch (h) { e(0); K = 0; return B(b) } return e
        } function M() {
            PUBNUB.time(w);
            PUBNUB.time(function () { setTimeout(function () { if (!N) { N = 1; p(R, function (b) { b[2].subscribe(b[0], b[1]) }) } }, G) })
        } function x() { if (!S.get) return 0; var b = { id: x.id++, send: function () { }, abort: function () { b.id = {} }, open: function (a, c) { x[b.id] = b; S.get(b.id, c) } }; return b } window.console || (window.console = window.console || {}); console.log || (console.log = (window.opera || {}).postError || function () { }); var O = function () {
            var b = window.localStorage; return {
                get: function (a) {
                    try {
                        if (b) return b.getItem(a); if (document.cookie.indexOf(a) ==
                        -1) return null; return ((document.cookie || "").match(RegExp(a + "=([^;]+)")) || [])[1] || null
                    } catch (c) { }
                }, set: function (a, c) { try { if (b) return b.setItem(a, c) && 0; document.cookie = a + "=" + c + "; expires=Thu, 1 Aug 2030 20:00:00 UTC; path=/" } catch (f) { } }
            }
        }(), X = 1, Y = /{([\w\-]+)}/g, P = "async", Q = "/", L = 31E4, G = 1E3, K = navigator.userAgent.indexOf("MSIE 6") == -1, $ = function () { var b = Math.floor(Math.random() * 9) + 1; return function (a) { return a.indexOf("pubsub") > 0 && a.replace("pubsub", "ps" + (++b < 10 ? b : b = 1)) || a } }(), H = {
            list: {}, unbind: function (b) {
                H.list[b] =
                []
            }, bind: function (b, a) { (H.list[b] = H.list[b] || []).push(a) }, fire: function (b, a) { p(H.list[b] || [], function (c) { c(a) }) }
        }, D = m("pubnub") || {}, N = 0, R = [], W = function (b) {
            var a = {}, c = b.publish_key || "", f = b.subscribe_key || "", k = b.ssl ? "s" : "", l = "http" + k + "://" + (b.origin || "pubsub.pubnub.com"), C = {
                history: function (d, e) { e = d.callback || e; var h = d.limit || 100, i = d.channel, u = o(); if (!i) return r("Missing Channel"); if (!e) return r("Missing Callback"); B({ callback: u, url: [l, "history", f, g(i), u, h], success: function (v) { e(v) }, fail: function (v) { r(v) } }) },
                time: function (d) { var e = o(); B({ callback: e, url: [l, "time", e], success: function (h) { d(h[0]) }, fail: function () { d(0) } }) }, uuid: function (d) { var e = o(); B({ callback: e, url: ["http" + k + "://pubnub-prod.appspot.com/uuid?callback=" + e], success: function (h) { d(h[0]) }, fail: function () { d(0) } }) }, publish: function (d, e) {
                    e = e || d.callback || function () { }; var h = d.message, i = d.channel, u = o(); if (!h) return r("Missing Message"); if (!i) return r("Missing Channel"); if (!c) return r("Missing Publish Key"); h = JSON.stringify(h); h = [l, "publish", c, f, 0,
                    g(i), u, g(h)]; B({ callback: u, success: function (v) { e(v) }, fail: function () { e([0, "Disconnected"]) }, url: h })
                }, unsubscribe: function (d) { d = d.channel; if (d in a) { a[d].connected = 0; a[d].done && a[d].done(0) } }, subscribe: function (d, e) {
                    function h() {
                        var T = o(); if (a[i].connected) a[i].done = B({
                            callback: T, url: [aa, "subscribe", f, g(i), T, v], fail: function () { if (!E) { E = 1; ba() } setTimeout(h, G); C.time(function (I) { if (I && E) { E = 0; U() } else ca() }) }, success: function (I) {
                                if (a[i].connected) {
                                    if (!V) { V = 1; da() } if (E) { E = 0; U() } u = O.set(f + i, v = u && O.get(f +
                                    i) || I[1]); p(I[0], function (ea) { e(ea, I) }); setTimeout(h, 10)
                                }
                            }
                        })
                    } var i = d.channel; e = e || d.callback; var u = d.restore, v = 0, ca = d.error || function () { }, da = d.connect || function () { }, U = d.reconnect || function () { }, ba = d.disconnect || function () { }, E = 0, V = 0, aa = $(l); if (!N) return R.push([d, e, C]); if (!i) return r("Missing Channel"); if (!e) return r("Missing Callback"); if (!f) return r("Missing Subscribe Key"); i in a || (a[i] = {}); if (a[i].connected) return r("Already Connected"); a[i].connected = 1; h()
                }, xdr: B, ready: M, db: O, each: p, map: s, css: z,
                $: m, create: A, bind: j, supplant: t, head: q, search: J, attr: n, now: w, unique: y, events: H, updater: F, init: W
            }; return C
        }; PUBNUB = W({ publish_key: n(D, "pub-key"), subscribe_key: n(D, "sub-key"), ssl: n(D, "ssl") == "on", origin: n(D, "origin") }); z(D, { position: "absolute", top: -G }); if ("opera" in window || n(D, "flash")) D.innerHTML = "<object id=pubnubs data=https://dh15atwfs066y.cloudfront.net/pubnub.swf><param name=movie value=https://dh15atwfs066y.cloudfront.net/pubnub.swf><param name=allowscriptaccess value=always></object>"; var S =
        m("pubnubs") || {}; j("load", window, function () { setTimeout(M, 0) }); PUBNUB.rdx = function (b, a) { if (!a) return x[b].onerror(); x[b].responseText = unescape(a); x[b].onload() }; x.id = G; window.jQuery && (window.jQuery.PUBNUB = PUBNUB); typeof module !== "undefined" && (module.exports = PUBNUB) && M()
    }();
})();