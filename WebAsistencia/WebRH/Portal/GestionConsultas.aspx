﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GestionConsultas.aspx.cs"
    Inherits="Portal_GestionConsultas" %>

<%@ Register Src="~/BarraMenu/BarraMenu.ascx" TagName="BarraMenu" TagPrefix="uc2" %>
<%@ Register Src="~/ConsultaIndividual.ascx" TagName="Consulta" TagPrefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Portal RRHH</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="viewport" content="width=device-width">
    <!-- CSS media query on a link element -->
    <%= Referencias.Css("../")%>
    <%= Referencias.Javascript("../")%>
    <script type="text/javascript" src="../Scripts/ConversorDeFechas.js"></script>
    <link href="../scripts/vex-2.1.1/css/vex.css" rel="stylesheet">
    <link href="../scripts/vex-2.1.1/css/vex-theme-os.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="../Reportes/Reportes.css" />
    <link rel="stylesheet" media="(max-width: 1600px)" href="estilosPortalSecciones.css" />
</head>
<body>
    <form id="form1" runat="server">
    <uc2:BarraMenu ID="BarraMenu" UrlPassword="../" runat="server" Feature="<span style='font-size:18px; font-weight: bold; padding-top:25px;'>Datos<br/>Recibo</span> <br/> "
        UrlImagenes="../Imagenes/" UrlEstilos="../Estilos/" />
    <div class="container-fluid">
        <h1 style="text-align: center; margin: 30px;">
        </h1>
        <div style="text-align: center;" class="caja_izq no-print">
        <p style="margin:25px; font-size: 2.1em; color: #fff;">ACCIONES</p>
            <input id="btn_consultas_pendientes" type="button" class="btn_gestion_consulta" style="margin: 10px; width: 150px; font-size: smaller;"
                value="CONSULTAS PENDIENTES" />
            <input id="btn_consultas_historicas" type="button" class="btn_gestion_consulta" style="margin: 10px; width: 150px; font-size: smaller;"
                value="CONSULTAS HISTORICAS" />
            <input id="Button3" type="button" class="btn_gestion_consulta" style="margin: 10px; width: 150px; font-size: smaller;"
                value="PARAMETRIA" />
            <input id="Button4" type="button" class="btn_gestion_consulta" style="margin: 10px; width: 150px; font-size: smaller;"
                value="REPORTES" />
        </div>
        <div class="caja_der papel">
            <%--DIV 1--%>
            <div id="consultas">
                <legend id="legend_gestion" style="margin-top:10px;">CONSULTAS PENDIENTES</legend>
                <input type="text" id="search" class="search" class="buscador" placeholder="Buscar" style="display: none;" />
                <div id="tablaConsultas" class="table table-striped table-bordered table-condensed">
                </div>
                <div id="div_detalle_consulta" style="display: none;">
                    <label style="margin-right: 20px;">ID:</label><input type="text" id="txt_nro_consulta"readonly style="width: 50px;"/>
                    <label style="margin-right: 20px;">Creador:</label><input type="text" id="txt_creador"readonly style="width: 250px;"/><input type="hidden" id="nroDocumentoCreador" />
                    <a id="btnConsultaIndividual" href="#" style="display: inline;" ><img src="../Imagenes/detalle.png" width="25px" height="25px" style="vertical-align: top;" /></a>
                    <label style="margin-right: 20px;margin-left: 20px;">Tipo de Consulta:</label><input type="text" id="txt_tipo" readonly style="width: 150px;"/>
                    <br />
                    <label style="margin-right: 24px;">
                        Motivo:</label>
                    <textarea id="ta_motivo" style="width: 100%; height: 150px;" readonly></textarea>
                    <br />
                    <br />
                    <label>
                        Respuesta:</label>
                    <textarea id="ta_respuesta" style="width: 100%; height: 150px;"></textarea>
                     <div style="text-align:center;">
                     <input id="btn_responder_consulta" type="button" class="btn btn-primary" style="margin: 10px; width: 100px;"
                value="Responder" />
                <input id="btn_volver_consulta" type="button" class="btn btn-primary" style="margin: 10px; width: 100px;"
                value="Volver" />
                </div>
                </div>
            </div>
        </div>
    </form>
    <div id="pantalla_consulta_individual" style="display: none">
        <h3>Consulta Individual</h3>
        <br />
        <uc3:Consulta ID="Consulta1"  runat="server"  />
    </div>
        
</body>
<script type="text/javascript" src="Legajo.js"></script>
<script type="text/javascript" src="../scripts/vex-2.1.1/js/vex.combined.min.js"></script>
<script type="text/javascript">

    $(document).ready(function ($) {
        Backend.start(function () {
            Legajo.getConsultasParaGestion();
        });

        $("#btnConsultaIndividual").click(function () {

            vex.defaultOptions.className = 'vex-theme-os';
            vex.open({
                afterOpen: function ($vexContent) {
                    var ui = $("#pantalla_consulta_individual").clone();
                    $vexContent.append(ui);
                    ui.show();
                    Legajo.getConsultaIndividual($('#nroDocumentoCreador').val(), ui);
                    /*ui.find("#btn_enviar_consulta").click(function () {
                        Backend.NuevaConsultaDePortal({
                            id_tipo_consulta: ui.find("#cmb_tipo_consulta").val(),
                            tipo_consulta: ui.find("#cmb_tipo_consulta option:selected").text(),
                            motivo: ui.find("#txt_motivo_consulta").val()
                        }).onSuccess(function (id_consulta) {
                            alertify.success("Consulta enviada con éxito");
                            vex.close();
                            Legajo.getConsultas();
                        }).onError(function (id_consulta) {
                            alertify.error("Error al enviar consulta");
                        });
                    });*/
                    return ui;
                },
                css: {
                    'padding-top': "4%",
                    'padding-bottom': "0%"
                },
                contentCSS: {
                    width: "47%",
                    height: "740px"
                }
            });

        });

    });


</script>
</html>
