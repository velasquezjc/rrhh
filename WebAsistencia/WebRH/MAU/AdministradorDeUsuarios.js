﻿var AdministradorDeUsuarios = function () {
    var _this = this;
    this.panel_datos_usuario = $('#panel_datos_usuario');
    this.lbl_nombre = $('#nombre');
    this.lbl_apellido = $('#apellido');
    this.lbl_documento = $('#documento');
    this.lbl_legajo = $('#legajo');
    this.txt_nombre_usuario = $('#txt_nombre_usuario');
    this.btn_reset_password = $('#btn_reset_password');

    var proveedor_ajax = new ProveedorAjax("../");
    this.autorizador = new Autorizador(proveedor_ajax);
    this.repositorioDeFuncionalidades = new RepositorioDeFuncionalidades(proveedor_ajax);
    this.repositorioDeUsuarios = new RepositorioDeUsuarios(proveedor_ajax);
    this.repositorioDePersonas = new RepositorioDePersonas(proveedor_ajax);
    this.repositorioDeAreas = new RepositorioDeAreas(proveedor_ajax);

    this.selector_usuario = new SelectorDePersonas({
        ui: $('#selector_usuario'),
        repositorioDePersonas: this.repositorioDePersonas,
        placeholder: "nombre, apellido, documento o legajo"
    });
      
    this.vista_permisos = new VistaDePermisosDeUnUsuario({
        ui: $('#vista_permisos'),
        repositorioDeFuncionalidades: this.repositorioDeFuncionalidades,
        autorizador: this.autorizador
    });

    this.vista_areas = new VistaDeAreasAdministradas({
        ui: $('#panel_areas_administradas'),
        autorizador: this.autorizador,
        repositorioDeAreas: this.repositorioDeAreas
    });

    this.selector_usuario.alSeleccionarUnaPersona = function (la_persona_seleccionada) {
        _this.panel_datos_usuario.hide();
        _this.repositorioDeUsuarios.getUsuarioPorIdPersona(
            la_persona_seleccionada.id,
            function (usuario) {
                _this.cargarUsuario(usuario);
            },
            function (error) {
                if (error == "LA_PERSONA_NO_TIENE_USUARIO") {
                    alertify.confirm(la_persona_seleccionada.nombre + " " + la_persona_seleccionada.apellido + " no tiene usuario, desea crear uno?", function (usuario_acepto) {
                        if (usuario_acepto) {
                            _this.repositorioDeUsuarios.crearUsuarioPara(la_persona_seleccionada.id,
                                function (usuario) {
                                    alertify.success("Se ha creado un usuario para " + la_persona_seleccionada.nombre + " " + la_persona_seleccionada.apellido);
                                    _this.repositorioDeUsuarios.resetearPassword(usuario.Id, function (nueva_clave) {
                                        alertify.alert("El password para el usuario: " + usuario.Alias + " es: " + nueva_clave);
                                    });
                                    _this.cargarUsuario(usuario);
                                },
                                function (error) {
                                    alertify.error("Error al crear un usuario para " + la_persona_seleccionada.nombre + " " + la_persona_seleccionada.apellido);
                                }
                            );
                        } else {
                            alertify.error("No se creó un usuario para " + la_persona_seleccionada.nombre + " " + la_persona_seleccionada.apellido);
                        }
                    });
                }
            });
    };

    this.btn_reset_password.click(function () {
        _this.repositorioDeUsuarios.resetearPassword(_this.usuario.Id, function (nueva_clave) {
            alertify.alert("El nuevo password para el usuario: " + _this.usuario.Alias + " es: " + nueva_clave);
        });
    });
};

AdministradorDeUsuarios.prototype.cargarUsuario = function (usuario) {
    this.usuario = usuario;
    this.panel_datos_usuario.show();
    this.vista_permisos.setUsuario(usuario);
    this.lbl_nombre.text(usuario.Owner.Nombre);
    this.lbl_apellido.text(usuario.Owner.Apellido);
    this.lbl_documento.text(usuario.Owner.Documento);
    this.lbl_legajo.text(usuario.Owner.Legajo);
    this.txt_nombre_usuario.text(usuario.Alias);
    this.vista_areas.setUsuario(usuario);
};