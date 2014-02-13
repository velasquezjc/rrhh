﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Permisos.aspx.cs" Inherits="MAU_Permisos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RRHH - Permisos de usuario</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <%= Referencias.Css("../")%>
    <link rel="stylesheet" href="../estilos/estilos.css" type="text/css"/>    
    <link rel="stylesheet" href="Permisos.css" type="text/css"/>    
    <link rel="stylesheet" href="../estilos/SelectorDePersonas.css" type="text/css"/>    
    <link href="ui.dynatree.css" rel="stylesheet" type="text/css">
    <link href="../scripts/select2-3.4.4/select2.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div id="selector_personas" class="selector_personas">
                <input id="buscador" type=hidden />
            </div>
            <div id="vista_permisos">
            </div>
        </div>
    </form>
    <div id="plantillas">
        <div class="vista_persona_en_selector">
            <div id="contenedor_legajo" class="label label-warning">
                <div id="titulo_legajo">Leg:</div>
                <div id="legajo"></div>
            </div> 
            <div id="nombre"></div>
            <div id="apellido"></div>
            <div id="contenedor_doc" class="label label-default">
                <div id="titulo_doc">Doc:</div>
                <div id="documento"></div>         
            </div>   
        </div>
    </div>
</body>
<%= Referencias.Javascript("../")%>
<script type="text/javascript" src="VistaDePermisosDeUnUsuario.js"></script>
<script type="text/javascript" src="ServicioDeSeguridad.js"></script>
<script type="text/javascript" src="NodoEnArbolDeFuncionalidades.js"></script>
<script type="text/javascript" src="../Scripts/ProveedorAjax.js"></script>

<script type="text/javascript" src="../Scripts/ServicioDePersonas.js"></script>
<script type="text/javascript" src="../Scripts/Persona.js"></script>
<script type="text/javascript" src="../Scripts/SelectorDePersonas.js"></script>

<script type="text/javascript" src="../Scripts/select2-3.4.4/Select2.min.js"></script>
<script type="text/javascript" src="../Scripts/select2-3.4.4/select2_locale_es.js"></script>
<script type="text/javascript" src="jquery.dynatree.min.js" ></script>


<script type="text/javascript">
    $(document).ready(function () {
        selector_personas = new SelectorDePersonas({ 
            ui: $('#selector_personas'),
            servicioDePersonas: new ServicioDePersonas(new ProveedorAjax())
         });
        vista_permisos = new VistaDePermisosDeUnUsuario({
            ui: $('#vista_permisos'),
            servicioDeSeguridad: new ServicioDeSeguridad(new ProveedorAjax())
        });
        selector_personas.alSeleccionarUnaPersona = function (la_persona_seleccionada) {
            vista_permisos.setUsuario(la_persona_seleccionada);
        };
    });
</script>
</html>