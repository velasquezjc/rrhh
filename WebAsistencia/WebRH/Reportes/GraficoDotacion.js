﻿var checks_activos = ["GraficoPorGenero"];
var filtro;
var spinner;

var GraficoDotacion = {

    Inicializar: function () {
        var _this = this;
        GraficoHerramientas.InicializarFecha($('#txt_fecha_desde'));
//        GraficoHerramientas.BlanquearParametrosDeBusqueda();
        GraficoHerramientas.ActivarPrimerCheck($('#cb1'), "Género");
        GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
        _this.SettearEventosDeLaPagina();
        _this.SettearEventosDelMenu();
    },

    SettearEventosDeLaPagina: function () {
        var _this = this;
        GraficoHerramientas.SettearEventosDeChecks(_this, $('.filtros'), $('#div_tabla_detalle'), $('#titulo_grafico'), "Dotación por ");

        $('#btn_armarGrafico').click(function () {
            _this.BuscarDatos();
        });
        $('#btn_excel').click(function () {
            _this.ObtenerLosDatosDeDotacionParaElExport();
        });
    },

    SettearEventosDelMenu: function () {
        var _this = this;
        $('#btn_genero').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("Genero", "GraficoPorGenero", "Dotación por " + this.innerHTML);
            $('#cb1')[0].checked = true;
        });
        $('#btn_nivel').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("Nivel", "GraficoPorNivel", "Dotación por " + this.innerHTML);
            $('#cb2')[0].checked = true;
        });
        $('#btn_estudios').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("Estudios", "GraficoPorEstudio", "Dotación por " + this.innerHTML);
            $('#cb3')[0].checked = true;
        });
        $('#btn_plantas').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("Plantas", "GraficoPorPlanta", "Dotación por " + this.innerHTML);
            $('#cb4')[0].checked = true;
        });
        $('#btn_areas').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("Areas", "GraficoPorArea", "Dotación por " + this.innerHTML);
            $('#cb5')[0].checked = true;
        });
        $('#btn_secretarias').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("Secreatarías", "GraficoPorSecretarias", "Dotación por " + this.innerHTML);
            $('#cb6')[0].checked = true;
        });
        $('#btn_subsecretarias').click(function () {
            GraficoHerramientas.OcultarTodosLosReportesExcepto("Dotacion");
            armarGraficoDesdeMenu("SubSecretarías", "GraficoPorSubSecretarias", "Dotación por " + this.innerHTML);
            $('#cb7')[0].checked = true;
        });

        function armarGraficoDesdeMenu(mi_filtro, tipo, texto) {
            checks_activos = [];
            filtro = mi_filtro;
            _this.VisualizarGraficoYTablaResumen(true);
            checks_activos.push(tipo);
            _this.BuscarDatos();
            $('#titulo_grafico').html(texto);
            $('.filtros').each(function () {
                this.checked = false;
            });
        };
    },

    BuscarDatos: function () {
        var _this = this;
        var check_seleccionado = checks_activos.slice(-1)[0];
        var fecha = $('#txt_fecha_desde').val();
        var id_area = localStorage.getItem("idArea");
        var alias = localStorage.getItem("alias");

        if (GraficoHerramientas.VerificarDatosObligatoriosParaBackend(fecha, check_seleccionado, id_area)) {
            _this.ObtenerLosDatosDeDotacion(check_seleccionado, fecha, id_area, $("#chk_incluir_dependencias").is(":checked"), "Dotación por " + filtro + " del Área " + alias, "container_grafico_torta_totales", "div_tabla_resultado_totales", "tabla_resultado_totales");
        }
    },


    //OBTENER LOS DATOS DESDE EL BACKEND
    ObtenerLosDatosDeDotacion: function (tipo, fecha, id_area, incluir_dependencias, titulo, div_grafico, div_tabla, tabla) {
        var _this = this;
        var spinner = new Spinner({ scale: 3 });
        spinner.spin($("html")[0]);

        Backend.GetGrafico({ tipo: tipo, fecha: fecha, id_area: parseInt(id_area), incluir_dependencias: incluir_dependencias })
            .onSuccess(function (grafico) {
                var tabla_resumen = grafico.tabla_resumen;
                var tabla_detalle = grafico.tabla_detalle;
                if (tabla_resumen.length > 0) {
                    _this.VisualizarTablaResumenYGrafico(true);
                    _this.DibujarElGrafico(tabla_resumen, titulo, div_grafico);
                    _this.DibujarTablaResumen(tabla_resumen, div_tabla, tabla, tabla_detalle);
                    _this.BuscadorDeTabla();
                } else {
                    _this.VisualizarTablaResumenYGrafico(false);
                    alertify.error("No hay Personal en el Área seleccionada para la generación del Gráfico");
                }
                spinner.stop();
            })
            .onError(function (e) {
                spinner.stop();
                alertify.error("Error al pedir datos. Detalle: " + e);
            });
    },

    ObtenerLosDatosDeDotacionParaElExport: function () {
        var _this = this;
        var check_seleccionado = checks_activos.slice(-1)[0];
        var fecha = $('#txt_fecha_desde').val();
        var id_area = localStorage.getItem("idArea");

        if (GraficoHerramientas.VerificarDatosObligatoriosParaBackend(fecha, check_seleccionado, id_area)) {

            var spinner = new Spinner({ scale: 3 });
            spinner.spin($("html")[0]);

            Backend.ExcelGenerado({ tipo: check_seleccionado, fecha: fecha, id_area: parseInt(id_area), incluir_dependencias: $("#chk_incluir_dependencias").is(":checked") })
            .onSuccess(function (resultado) {
                if (resultado.length > 0) {
                    var nombre_del_documento = "";
                    switch (check_seleccionado) {
                        case "GraficoPorGenero":
                            nombre_del_documento = "DOTACION_POR_GENERO_";
                            break;
                        case "GraficoPorNivel":
                            nombre_del_documento = "DOTACION__POR_NIVEL_";
                            break;
                        case "GraficoPorEstudio":
                            nombre_del_documento = "DOTACION_POR_ESTUDIO_";
                            break;
                        case "GraficoPorPlanta":
                            nombre_del_documento = "DOTACION_POR_PLANTA_";
                            break;
                        case "GraficoPorArea":
                            nombre_del_documento = "DOTACION_POR_AREA_";
                            break;
                        case "GraficoPorSecretarias":
                            nombre_del_documento = "DOTACION_POR_SECRETARIAS_";
                            break;
                        case "GraficoPorSubSecretarias":
                            nombre_del_documento = "DOTACION_POR_SUBSECRETARIAS_";
                            break;
                        default:
                            nombre_del_documento = "ERROR_DE_FILTRO_";
                    };
                    var a = window.document.createElement('a');
                    a.href = "data:application/vnd.ms-excel;base64," + resultado;
                    a.download = nombre_del_documento + fecha + "_.xlsx";
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                } else {
                    alertify.error("No se han encontrado datos para Exportar");
                }
                spinner.stop();
            })
            .onError(function (e) {
                spinner.stop();
                alertify.error("error al Exportar datos. Detalle: " + e);
            });

        }
    },


    //DIBUJAR GRAFICO
    DibujarElGrafico: function (datos_del_resumen, titulo, div_grafico) {
        var datos = this.CrearDatosDesdeElResumenParaArmarElGrafico(datos_del_resumen);
        var grafico = [{ type: 'pie', name: 'Dotación', data: datos}];
        $('#' + div_grafico).highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45,
                    beta: 0
                }
            },
            title: {
                text: titulo
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.2f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    depth: 35,
                    dataLabels: {
                        enabled: true,
                        format: '{point.name}' + ': ' + '{point.percentage:.2f}' + '%',
                        style: {
                            textShadow: ''
                        }
                    }
                }
            },
            series: grafico
        });
    },

    CrearDatosDesdeElResumenParaArmarElGrafico: function (resumen) {
        var datos = [];
        for (var i = 0; i < resumen.length; i++) {
            if (resumen[i].Id != "Total") {
                if (parseInt(resumen[i].Cantidad) > 0) {
                    if (resumen[i].DescripcionGrafico == null) {
                        nombre = resumen[i].Id.replace(/\|/g, "");
                    } else {
                        nombre = resumen[i].DescripcionGrafico;
                    }
                    var porcion = [nombre, parseInt(resumen[i].Cantidad)];
                    datos.push(porcion);
                }
            }
        };
        return datos;
    },

    FiltrarPersonasParaTablaDetalle: function (criterio, tabla) {
        var _this = this;
        var tabla_final = [];

        if (tabla.length > 0) {
            var titulo = "Tabla de Toda la Dotación del Área";
            if (criterio == "Total") {
                tabla_final = tabla;
            } else {
                switch (checks_activos[0]) {
                    case "GraficoPorGenero":
                        titulo = "Tabla de la Dotación de Sexo " + criterio;
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].Sexo == criterio) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                    case "GraficoPorNivel":
                        titulo = "Tabla de la Dotación de " + criterio;
                        var nivel = criterio.split(" ");
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].Nivel == nivel[1]) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                    case "GraficoPorEstudio":
                        titulo = "Tabla de la Dotación con Nivel de Estudios " + criterio;
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].NivelEstudio == criterio) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                    case "GraficoPorPlanta":
                        titulo = "Tabla de la Dotación con Tipo de Planta " + criterio;
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].Planta == criterio) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                    case "GraficoPorArea":
                        titulo = "Dotación del Área " + criterio;
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].Area == criterio) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                    case "GraficoPorSecretarias":
                        titulo = "Dotación de " + criterio;
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].NombreSecretaria == criterio) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                    case "GraficoPorSubSecretarias":
                        titulo = "Dotación de " + criterio;
                        for (var i = 0; i < tabla.length; i++) {
                            if (tabla[i].NombresubSecretaria == criterio) {
                                tabla_final.push(tabla[i]);
                            }
                        } break;
                }
            }
            titulo = titulo + " del Área " + localStorage.getItem("alias");
            $('#lb_titulo_tabla_detalle').text(titulo);
            _this.VisualizarTablaDetalle(true);
            _this.DibujarTablaDetalle(tabla_final, "div_tabla_detalle", "tabla_detalle");
        } else {
            _this.VisualizarTablaDetalle(false);
            alertify.error("No hay Datos para la Fila de Resumen Seleccionada");
        }
    },

    //DIBUJO DE LAS TABLAS
    DibujarTablaResumen: function (resultado, div_tabla, tabla, tabla_detalle) {
        var _this = this;
        $("#" + tabla).empty();

        var divGrilla = $('#' + tabla);
        var tabla = resultado;
        var nombre = "";
        var columnas = [];

        columnas.push(new Columna("Información", {
            generar: function (un_registro) {
                nombre = un_registro.Id.replace(/\|/g, "&nbsp;");
                un_registro.Id = un_registro.Id.replace(/\|/g, "");
                return nombre;
            }
        }));
        columnas.push(new Columna("Cantidad", { generar: function (un_registro) { return un_registro.Cantidad } }));
        columnas.push(new Columna("Porcentaje", { generar: function (un_registro) { return parseFloat(un_registro.Porcentaje).toFixed(2) + '%' } }));
        columnas.push(new Columna('Detalle', {
            generar: function (un_registro) {
                var btn_accion = $('<a>');
                var img = $('<img>');
                img.attr('src', '../Imagenes/detalle.png');
                img.attr('width', '15px');
                img.attr('height', '15px');
                btn_accion.append(img);
                btn_accion.click(function () {
                    _this.FiltrarPersonasParaTablaDetalle(un_registro.Id, tabla_detalle);
                });
                return btn_accion;
            }
        }));
        _this.GrillaResumen = new Grilla(columnas);
        _this.GrillaResumen.SetOnRowClickEventHandler(function (un_registro) { });
        _this.GrillaResumen.CargarObjetos(tabla);
        _this.GrillaResumen.DibujarEn(divGrilla);
    },

    DibujarTablaDetalle: function (resultado, div_tabla, tabla) {
        var _this = this;
        $("#" + tabla).empty();
        var divGrilla = $('#' + tabla);
        var tabla = resultado;
        var columnas = [];

        columnas.push(new Columna("Area", { generar: function (un_registro) { return un_registro.Area } }));
        columnas.push(new Columna("CUIL", { generar: function (un_registro) { return un_registro.CUIL } }));
        columnas.push(new Columna("Apellido_Nombre", { generar: function (un_registro) { return (un_registro.Apellido + ", " + un_registro.Nombre) } }));
        columnas.push(new Columna("Sexo", { generar: function (un_registro) { return un_registro.Sexo } }));
        columnas.push(new Columna("FechaNacimiento", { generar: function (un_registro) { return GraficoHerramientas.ConvertirFecha(un_registro.FechaNacimiento) } }));
        columnas.push(new Columna("FechaIngreso", { generar: function (un_registro) { return GraficoHerramientas.ConvertirFecha(un_registro.FechaIngreso) } }));
        columnas.push(new Columna("Nivel", { generar: function (un_registro) { return un_registro.Nivel } }));
        columnas.push(new Columna("Grado", { generar: function (un_registro) { return un_registro.Grado } }));
        columnas.push(new Columna("Planta", { generar: function (un_registro) { return un_registro.Planta } }));
        columnas.push(new Columna("NivelEstudio", { generar: function (un_registro) { return un_registro.NivelEstudio } }));
        columnas.push(new Columna("Titulo", { generar: function (un_registro) { return un_registro.Titulo } }));
        columnas.push(new Columna('Detalle', {
            generar: function (un_registro) {
                var btn_accion = $('<a>');
                var img = $('<img>');
                img.attr('src', '../Imagenes/detalle.png');
                img.attr('width', '15px');
                img.attr('height', '15px');
                btn_accion.append(img);
                btn_accion.click(function () {
                    console.log(un_registro);
                    localStorage.setItem("documento", un_registro.NroDocumento);
                    window.open('ConsultaIndividual.aspx', '_blank');
                });
                return btn_accion;
            }
        }));
        _this.GrillaResumen = new Grilla(columnas);
        _this.GrillaResumen.SetOnRowClickEventHandler(function (un_registro) { });
        _this.GrillaResumen.CargarObjetos(tabla);
        _this.GrillaResumen.DibujarEn(divGrilla);
        _this.BuscadorDeTablaDetalle();
    },

    //VISUALIZADORES
    VisualizarTablaResumenYGrafico: function (visualizar) {
        if (visualizar) {
            $('#container_grafico_torta_totales').show();
            $("#search").show();
            $('#div_graficos_y_tablas').show();
        } else {
            $('#container_grafico_torta_totales').hide();
            $("#search").hide();
            $('#div_graficos_y_tablas').hide();
        }
    },

    VisualizarTablaDetalle: function (visualizar) {
        if (visualizar) {
            $('#container_grafico_torta_totales').show();
            $('#search_detalle').show();
            $('#div_tabla_detalle').show();
        } else {
            $('#container_grafico_torta_totales').hide();
            $('#search_detalle').hide();
            $('#div_tabla_detalle').hide();
        }
    },

    VisualizarGraficoYTablaResumen: function (visualizar) {
        if (visualizar) {
            $('#div_grafico_de_dotacion').show();
            $('#div_filtros').show();
            $('#div_graficos_y_tablas').hide();
        }
    },

    //BUSCADORES DE LAS TALBAS
    BuscadorDeTabla: function () {
        var options = {
            valueNames: ['Información', 'Cantidad', 'Porcentaje']
        };
        var featureList = new List('div_tabla_resultado_totales', options);
    },
    BuscadorDeTablaDetalle: function () {
        var options = {
            valueNames: ['Area', 'NroDocumento', 'Apellido_Nombre', 'Sexo', 'Nivel', 'Grado', 'Planta', 'NivelEstudio', 'Titulo']
        };
        var featureList = new List('div_tabla_detalle', options);
    }
}
