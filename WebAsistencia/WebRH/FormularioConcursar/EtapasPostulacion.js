﻿var PanelEtapasPostulacion = function (opciones) {
    var _this = this;
    _this.ui = $("#div_cambio_etapas");
    _this.cmb_etapas_concurso = $("#cmb_etapas_concurso");
    _this.btn_buscar_etapas = $("#btn_buscar_etapas");

    _this.btn_buscar_etapas.click(function () {
        _this.BuscarPostulaciones();
    });

    _this.cmb_etapas_concurso.change(function () {
        _this.InsEtapaPostulacion();
        _this.BuscarPostulaciones();
    });


    _this.BuscarEtapas = function () {
        
        Backend.ejecutar("BuscarEtapasConcurso",
                        [""],
                        function (respuesta) {
                            _this.cmb_etapas_concurso.append($("<option>").val(-1).text("Seleccione"));
                            for (var i = 0; i < respuesta.length; i++) {
                                var el = $("<option>");
                                el.val(respuesta[i].Id);
                                el.text(respuesta[i].Descripcion);
                                _this.cmb_etapas_concurso.append(el);
                            }
                        },
                        function (errorThrown) {
                            alertify.alert(errorThrown);
                        }
        );
    }

    _this.BuscarPostulaciones = function () {
        var codigo = $("#txt_codigo_postulacion").val();
        Backend.ejecutar("GetPostulacionesPorCodigo",
                        [codigo],
                        function (respuesta) {
                            _this.CompletarDatos(JSON.parse(respuesta));
                        },
                        function (errorThrown) {
                            alertify.alert(errorThrown);
                        }
        );
    }

    _this.InsEtapaPostulacion = function () {
        var postulacion = JSON.parse($("#postulacion").val()).Postulacion;
        var id_etapa = cmb_etapas_concurso.val();
        var proveedor_ajax = new ProveedorAjax();
        Backend.ejecutar("InsEtapaPostulacion",
            [postulacion.Id, id_etapa],
            function (respuesta) {
                alertify.alert("Etapa Guardada");
            },
            function (XMLHttpRequest, textStatus, errorThrown) {
                alertify.alert("Error.");
            }
        );
    }

    _this.CompletarDatos = function (datos_postulacion) {
        var div_tabla_historial = $("#div_tabla_historial");
        var span_empleado = $("#span_empleado");
        var span_codigo = $("#span_codigo");
        var span_fecha = $("#span_fecha");
        var span_perfil = $("#span_perfil");
        var postulacion = $("#postulacion");

        var usu_etapas = datos_postulacion.UsuEtapas;

        postulacion.val(JSON.stringify(datos_postulacion));

        var columnas = [];
        columnas.push(new Columna("Fecha", { generar: function (una_etapa) { return ConversorDeFechas.deIsoAFechaEnCriollo(una_etapa.Fecha) + "__" + una_etapa.Fecha } }));
        columnas.push(new Columna("Descripción", { generar: function (una_etapa) { return una_etapa.Descripcion } }));
        columnas.push(new Columna("Usuario", { generar: function (una_etapa) { return una_etapa.UsuarioEtapa; } }));

        this.GrillaHistorial = new Grilla(columnas);
        this.GrillaHistorial.AgregarEstilo("table table-striped");
        this.GrillaHistorial.CambiarEstiloCabecera("cabecera_tabla_pantalla_cargos");
        this.GrillaHistorial.SetOnRowClickEventHandler(function (una_etapa) {
        });

        div_tabla_historial.html("");
        this.GrillaHistorial.CargarObjetos(usu_etapas);
        this.GrillaHistorial.DibujarEn(div_tabla_historial);

        span_empleado.html(datos_postulacion.UsuarioPostulacion);
        span_codigo.html(datos_postulacion.Postulacion.Numero);
        span_fecha.html(ConversorDeFechas.deIsoAFechaEnCriollo(datos_postulacion.Postulacion.FechaPostulacion));
        span_perfil.html(datos_postulacion.Postulacion.Puesto.Familia);
        $("#seccion_historial").show();
    }

    _this.BuscarEtapas();
}