﻿var ProveedorAjax = function (raiz) {
    var raiz_detectada = "";
    for (var i = 0; i < window.location.pathname.split('/').length - 3; i++) {
        raiz_detectada += "../";
    };
    this.raiz = raiz || raiz_detectada;
};

ProveedorAjax.prototype.postearAUrl = function (datos_del_post) {
    $.ajax({
        url: this.raiz + "AjaxWS.asmx/" + datos_del_post.url,
        type: "POST",
        data: JSON.stringify(datos_del_post.data),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (respuestaJson) {

            if (respuestaJson.hasOwnProperty('d')) {
                datos_del_post.success(JSON.parse(respuestaJson.d));
            } else {
                datos_del_post.success(respuestaJson);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            datos_del_post.error(XMLHttpRequest, textStatus, errorThrown);
        }
    });
};

//define(["jquery"], function ($) {
//    return ProveedorAjax;
//});