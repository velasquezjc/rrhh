﻿$.fn.extend({
    validaciones: {
        esEmailValido: {
            evaluar: function (control) {
                if (control.val() == "") return true;
                return control.val().length > 0 && (/^([\w\-\.]+@([\w\-]+\.)+[\w\-]{2,6})?$/).test(control.val());
            },
            mensaje: "Ingrese un mail válido"
        },

        esNumeroNatural: {
            evaluar: function (control) {
                if (control.val() == "") return true;
                return (/^\d+$/).test(control.val());
            },
            mensaje: "Ingrese dato numérico"
        },

        esUnCombo: {
            evaluar: function (control) {
                if (control.val() == "") return true;
                return (/^\d+$/).test(control.val());
            },
            mensaje: "Seleccione una opción"
        },

        esNoBlanco: {
            evaluar: function (control) {
                return control.val().toString().length > 0;
            },
            mensaje: "El campo es obligatorio"
        },

        longitudMaxima: {
            evaluar: function (control, maxLong) {
                if (control.val() == "") return true;
                this.mensaje = "La longitud del texto ingresado debe ser menor a " + maxLong.toString();
                return control.val().toString().length <= maxLong;
            },
            mensaje: ""
        },
    },
    esValido: function () {
        var control = $(this);
        var es_valido = true;

        control.find("[data-validar]").each(function (i, sub_control) {
            sub_control = $(sub_control);
            sub_control.keyup(function () {
                sub_control.aplicarValidaciones();
            });
            sub_control.change(function () {
                sub_control.aplicarValidaciones();
            });
            if (!sub_control.aplicarValidaciones()) es_valido = false;
        });
        return es_valido;
    },
    aplicarValidaciones: function () {
        var control = $(this);
        var es_valido = true;

        var v = control.attr("data-validar").split(",");
        var mensaje = "";
        $.each(v, function (indice, validacion) {
            var parseo = validacion.split("(");
            var nombre_validacion = parseo[0].trim();
            var argumento_validacion;
            if(parseo.length>1){
                argumento_validacion = parseo[1].split(")")[0];
            }
            if (!control.validaciones[nombre_validacion]) {
                console.log("La validacion " + nombre_validacion + " no existe");
                return;
            }
            if (!control.validaciones[nombre_validacion].evaluar(control, argumento_validacion)) {
                mensaje += ", " + control.validaciones[nombre_validacion].mensaje;
                es_valido = false;
            }
        });
        if (es_valido) {
            control.limpiarValidaciones();
        } else {
            control.limpiarValidaciones();
            control.addClass("control-invalido");
            control.opentip(mensaje.substring(2), {
                removeElementsOnHide: true,
                target: true,
                style: "alert",
                showOn: "creation",
                hideDelay: 0.2,
                joint: "right"
            });
        }
        return es_valido;
    },
    limpiarValidaciones: function () {
        var control = $(this);
        var str_validaciones = control.attr("data-validar");
        if (typeof str_validaciones !== typeof undefined && str_validaciones !== false) {
            control.removeClass("control-invalido");
            for (var i = 0; i < Opentip.tips.length; i++) {
                var tip = Opentip.tips[i];
                if (tip.triggerElement.attr("id") == control.attr("id")) tip.hide();
            }
        } else {
            this.find("[data-validar]").each(function (i, sub_control) {
                $(sub_control).removeClass("control-invalido");
                for (var i = 0; i < Opentip.tips.length; i++) {
                    var tip = Opentip.tips[i];
                    if (tip.triggerElement.attr("id") == $(sub_control).attr("id")) tip.hide();
                }
            });
        }
    }
});
