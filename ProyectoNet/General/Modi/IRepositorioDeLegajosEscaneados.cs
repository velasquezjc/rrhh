﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Modi
{
    public interface  IRepositorioDeLegajosEscaneados
    {
        List<ThumbnailImagenModi> getThumbnailsDeImagenesSinAsignarParaUnLegajo(int legajo);
        List<ThumbnailImagenModi> getThumbnailsDeImagenesAsignadasAlDocumento(string tabla, int id);
    }
}
