﻿$(function () {
    Backend.start(function () {
        var param = document.URL.split('?')[1];

        Backend.ObtenerVehiculoPorIDVerificacion(param).onSuccess(function (respuesta_vehiculo) {

            if (respuesta_vehiculo.Respuesta == 0) {
                $("#mensaje_error").show();
                $("#mensaje_error").addClass("animated slideInDown");
                $("#Contenido").hide();
                document.getElementById("mihtml").style.height = "100%";
                return;
            }

            if (respuesta_vehiculo.vehiculo.Apellido == "Sin Asignación") {
                $("#responsable").text(respuesta_vehiculo.vehiculo.Apellido);
            }
            else {
                $("#responsable").text(respuesta_vehiculo.vehiculo.Apellido + ', ' + respuesta_vehiculo.vehiculo.Nombre);
            }

            $("#marca").text(respuesta_vehiculo.vehiculo.Marca);
            $("#Modelo").text(respuesta_vehiculo.vehiculo.Modelo);
            $("#segmento").text(respuesta_vehiculo.vehiculo.Segmento);
            $("#dominio").text(respuesta_vehiculo.vehiculo.Dominio);
            $("#año").text(respuesta_vehiculo.vehiculo.Anio);
            $("#Motor").text(respuesta_vehiculo.vehiculo.Motor);
            $("#chasis").text(respuesta_vehiculo.vehiculo.Chasis);
            $("#area").text(respuesta_vehiculo.vehiculo.Area);

            $(".tabla-principal").show();
            $(".contenedor-imagen-vehiculo").show();
            $("#mensaje_error").hide();
            $(".tabla-principal").addClass("animated slideInLeft");
            $("#contenedor-vehiculos").addClass("animated slideInRight");
            $("#contenedor-conductor").addClass("animated slideInRight");
            $(".contenedor-imagen-vehiculo").addClass("animated zoomIn");
            $("#contenedor-banner-parrafo").addClass("animated slideInDown");
            $("#barra_menu_contenedor_imagen").addClass("animated slideInDown");

           $("#myCarousel").carousel({
                interval: 3000,
                pause: false
            })

        });

    });
});
