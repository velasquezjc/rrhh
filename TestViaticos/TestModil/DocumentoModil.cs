﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestViaticos.TestModil
{
    public class DocumentoModil
    {
        private List<ImagenModil> imagenes;
        public string descripcionEnRRHH { get; set; }
        public string jurisdiccion { get; set; }
        public string organismo { get; set; }
        public string folio { get; set; }
        public DateTime fechaDesde { get; set; }
        public string tabla { get; set; }
        public int id { get; set; }

        public DocumentoModil(  string una_tabla,
                                int un_id,
                                string unaDescripcion, 
                                string una_jurisdiccion, 
                                string un_organismo, 
                                string un_folio, 
                                DateTime una_fecha_desde )
        {
            this.descripcionEnRRHH = unaDescripcion;
            this.jurisdiccion = una_jurisdiccion;
            this.organismo = un_organismo;
            this.folio = un_folio;
            this.fechaDesde = una_fecha_desde;
            this.tabla = una_tabla;
            this.id = un_id;
        }

        public List<ImagenModil> getImagenes()
        {
            return imagenes;
        }

        public bool tieneImagenes()
        {
            return imagenes.Any();
        }

    }
}
