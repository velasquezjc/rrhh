﻿var FichaGrandeDeDocumento = function (documento, ui, plantilla_transicion, lista_areas, ficha_chica) {
    this.ui = ui;
    this.documento = documento;
    this.lista_areas = lista_areas;
    this.ficha_chica = ficha_chica;
    this.plantilla_transicion = plantilla_transicion;
    this.start();
};
FichaGrandeDeDocumento.prototype = {
    start: function () {
        var self = this;
        this.area_creadora = this.ui.find("#ficha_grande_contenido_area_creadora");
        this.tiempo_en_area_actual = this.ui.find("#ficha_grande_contenido_tiempo_en_area_actual");
        this.comentarios = this.ui.find("#ficha_grande_contenido_comentarios");
        this.div_area_destino = this.ui.find("#ficha_grande_contenido_area_destino");
        this.boton_guardar_cambios = this.ui.find("#ficha_grande_boton_guardar_cambios");
        this.boton_guardar_cambios = this.ui.find("#ficha_grande_boton_guardar_cambios");
        this.panel_transiciones = this.ui.find("#ficha_grande_transiciones");

        this.boton_guardar_cambios.click(function () {
            WebService.guardarCambiosEnDocumento(self.documento.id,
                                                self.selector_de_area_destino.areaSeleccionada().id,
                                                self.comentarios.val(),
                                                function (doc) {
                                                    self.ficha_chica.mostrarDocumento(doc);
                                                    self.mostrarDocumento(doc);
                                                }
                                            );
        });

        this.selector_de_area_destino = new InputAutocompletableDeAreas(this.div_area_destino, this.lista_areas);
        this.mostrarDocumento(this.documento);
    },
    mostrarDocumento: function (documento) {
        this.documento = documento;
        this.area_creadora.text(this.documento.areaCreadora.descripcion);
        this.tiempo_en_area_actual.text(this.documento.enAreaActualHace.dias + " dias");
        this.comentarios.val(this.documento.comentarios);

        if (this.documento.areaDestino == null) this.selector_de_area_destino.limpiar();
        else this.selector_de_area_destino.setAreaSeleccionada(this.documento.areaDestino);

        this.panel_transiciones.empty();
        for (var i = 0; i < documento.historial.length; i++) {
            var trans = new TransicionDeDocumento(this.plantilla_transicion.clone(), documento.historial[i]);
            trans.dibujarEn(this.panel_transiciones);
        }
    },
    dibujarEn: function (panel) {
        panel.append(this.ui);
    },
    borrar: function () {
        this.ui.remove();
    }
};