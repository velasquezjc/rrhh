﻿var PanelDetalleDeCompetenciaInformatica = {
    mostrar: function (opciones) {
        //valores default
        var competencia_informatica = opciones.competencia_informatica || {
            Pais: 9,
            Nivel: 1,
            TipoInformatica:1,
            Conocimiento:1
         };
        var alModificar = opciones.alModificar || function () { };

        var _this = this;
        this.ui = $("#un_div_modal");
        this.ui.find("#contenido_modal").load("PanelDetalleDeCompetenciaInformatica.htm", function () {
            var generador_combos = new ComboPopuladoConRepoBuilder(Repositorio);
            generador_combos.construirCombosEn(_this.ui, competencia_informatica);

            _this.txt_competencias_informaticas_diploma_certificacion = _this.ui.find("#txt_competencias_informaticas_diploma_certificacion");
            _this.txt_competencias_informaticas_diploma_certificacion.val(competencia_informatica.Diploma);

            _this.txt_competencias_informaticas_fecha_obtencion = _this.ui.find("#txt_competencias_informaticas_fecha_obtencion");
            _this.txt_competencias_informaticas_fecha_obtencion.datepicker();
            _this.txt_competencias_informaticas_fecha_obtencion.datepicker('option', 'dateFormat', 'dd/mm/yy');
            _this.txt_competencias_informaticas_fecha_obtencion.datepicker('setDate', ConversorDeFechas.deIsoAFechaEnCriollo(competencia_informatica.FechaObtencion));

            _this.txt_competencias_informaticas_establecimiento = _this.ui.find("#txt_competencias_informaticas_establecimiento");
            _this.txt_competencias_informaticas_establecimiento.val(competencia_informatica.Establecimiento);
            _this.cmb_competencias_informaticas_localidad = _this.ui.find("#cmb_competencias_informaticas_localidad");
            _this.cmb_competencias_informaticas_localidad.val(competencia_informatica.Localidad);


//            _this.txt_competencias_informaticas_conocimiento = new SuperCombo({
//                ui: _this.ui.find("#txt_competencias_informaticas_conocimiento"),
//                nombre_repositorio: "ConocimientoCompetenciaInformatica",
//                   filtro: { Tipo: competencia_informatica.Tipo },
//                id_item_seleccionado: competencia_informatica.Conocimiento
//            });


//            _this.txt_competencias_informaticas_tipo_informatica = new SuperCombo({
//                ui: _this.ui.find("#txt_competencias_informaticas_tipo_informatica"),
//                nombre_repositorio: "TiposCompetenciaInformatica",
//                id_item_seleccionado: competencia_informatica.TipoInformatica,
//                 al_seleccionar: function (id_tipo) {
//                _this.txt_competencias_informaticas_conocimiento.cambiarFiltro({ Tipo: id_tipo});
//                 }
//            });

                




            //TiposCompetenciaInformatica


         //   _this.txt_competencias_informaticas_tipo_informatica = _this.ui.find("#txt_competencias_informaticas_tipo_informatica");
         //   _this.txt_competencias_informaticas_tipo_informatica.val(competencia_informatica.TipoInformatica);
         
         
          //  _this.txt_competencias_informaticas_conocimiento = _this.ui.find("#txt_competencias_informaticas_conocimiento");
         //   _this.txt_competencias_informaticas_conocimiento.val(competencia_informatica.Conocimiento);
   //            _this.txt_competencias_informaticas_nivel = _this.ui.find("#txt_competencias_informaticas_nivel");
           
           // _this.txt_competencias_informaticas_nivel.val(competencia_informatica.Nivel);

            _this.txt_competencias_informaticas_detalle = _this.ui.find("#txt_competencias_informaticas_detalle");
            _this.txt_competencias_informaticas_detalle.val(competencia_informatica.Detalle);

             //Bt cerrar
            _this.btn_cerrar = _this.ui.find(".modal_close_concursar");
            _this.btn_cerrar.click(function () {                
               _this.ui.limpiarValidaciones();
            });


            //Bt agregar
            _this.btn_guardar = _this.ui.find("#btn_guardar");
            if (opciones.competencia_informatica) _this.btn_guardar.val("Guardar Cambios");

            _this.btn_guardar.click(function () {

             if (_this.ui.esValido()) {

                competencia_informatica.Diploma = _this.txt_competencias_informaticas_diploma_certificacion.val();
                competencia_informatica.FechaObtencion = _this.txt_competencias_informaticas_fecha_obtencion.datepicker('getDate').toISOString();
                competencia_informatica.Establecimiento = _this.txt_competencias_informaticas_establecimiento.val();
                competencia_informatica.Localidad = _this.cmb_competencias_informaticas_localidad.val();
                
               // competencia_informatica.TipoInformatica = _this.txt_competencias_informaticas_tipo_informatica.val();
                 competencia_informatica.TipoInformatica = _this.txt_competencias_informaticas_tipo_informatica.idItemSeleccionado();
                  //  domicilioLegal_nuevo.Provincia = _this.cmb_domicilio_legal_provincia.idItemSeleccionado();

              //  competencia_informatica.Conocimiento = _this.txt_competencias_informaticas_conocimiento.val();

               // competencia_informatica.Nivel = _this.txt_competencias_informaticas_nivel.val();
                    
                competencia_informatica.Detalle = _this.txt_competencias_informaticas_detalle.val();

                var proveedor_ajax = new ProveedorAjax();

                 if (opciones.competencia_informatica) {

                proveedor_ajax.postearAUrl({ url: "ActualizarCvCompetenciaInformatica",
                    data: {
                        competencia_informatica: competencia_informatica,
                    },
                    success: function (respuesta) {
                        alertify.alert("La competencia fue actualizada correctamente");
                        alModificar(respuesta);
                        $(".modal_close_concursar").click();
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alertify.alert("Error al actualizar la competencia informática.");
                     }
                    });

                    return;
                }

                proveedor_ajax.postearAUrl({ url: "GuardarCvCompetenciaInformatica",
                    data: {
                        competencia_informatica: competencia_informatica
                    },
                    success: function (respuesta) {
                        alertify.alert("La competencia fue guardada correctamente");
                        alModificar(respuesta);
                        $(".modal_close_concursar").click();
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alertify.alert("Error al guardar la competencia informática.");
                    }
                });
                }

            });

            var link_trucho = $("<a href='#un_div_modal'></a>");
            link_trucho.leanModal({ top: 300, closeButton: ".modal_close_concursar" });
            link_trucho.click();
        });
    }
}