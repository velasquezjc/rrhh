﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BarraMenuInicioSACC.ascx.cs" Inherits="BarraMenuInicioSACC" %>

<head>
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link id="link1" rel="stylesheet" href="<%= UrlEstilos %>EstilosBarraMenu.css" type="text/css" />
</head>

<div id= "barra_menu_contenedor">
        <div id="contenedor_imagen">
            <div style="height:69px; width:1024px; background-color:#0e1824;" id="barra_menu_contenedor_imagen">
                <%--<img src="<%= UrlImagenes %>BarraMenu/encabezado_sin_logos.png" height="69px;" width="1024px;" alt="logosistema"  />--%>
                <img src="<%= UrlImagenes %>logo_sistema.png" id="img_logo_sistema" width="130px" height="39px"  alt="logosistema"  />              
                <img src="<%= UrlImagenes %>logo_ministerio.png" id="img_logo_minis" style="float:left;" width="150px" height="27px"  alt="logosistema"  />
                <img src="<%= UrlImagenes %>logo_direccion.png" id="img_logo_direccion" width="130px" height="26px"  alt="logosistema"  />
                

                <div id="barra_menu_nombre_sistema"><%= Feature %></div>
            </div>
        </div>
        <div id="contenedor_barraInferior">
            <div id="barra_menu_inferior">
                <div id="barra_menu_inferior_usuario">
                    <div id="barra_menu_label_usuario"> USUARIO </div>
                    <asp:Label ID="LabelUsuario" runat="server"></asp:Label>  
                </div>
                <div id="barra_menu_inferior_botones">
                    <asp:Button ID="VolverAInicio" 
                                    CssClass="barra_menu_botones"
                                    runat="server" 
                                    OnClick="VolverAInicioLinkButton_Click"
                                    Text="Inicio" >
                    </asp:Button>
                    <asp:Button ID="CerrarSessionLinkButton" 
                                    CssClass="barra_menu_botones"
                                    runat="server"
                                    OnClick="CerrarSessionLinkButton_Click" 
                                    Text="Cerrar Sesión" >     
                    </asp:Button>
                </div>
            </div>
        </div>
</div>