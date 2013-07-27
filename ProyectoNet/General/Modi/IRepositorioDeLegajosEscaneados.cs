﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Modi
{
    public interface  IRepositorioDeLegajosEscaneados
    {
        ImagenModi GetImagenPorId(int id_imagen);
        ImagenModi GetThumbnailPorId(int id_imagen, int alto, int ancho);
        void AsignarImagenADocumento(int id_imagen, string tabla, int id_documento, Usuario usuario);
        List<int> GetIdsDeImagenesSinAsignarParaElLegajo(int numero_legajo);
        List<int> GetIdsDeImagenesAsignadasAlDocumento(string tabla, int id_documento);
        void DesAsignarImagen(int id_imagen, Usuario usuario);
        int CategoriaDeUnDocumento(string tabla, int id_documento);
    }
}
