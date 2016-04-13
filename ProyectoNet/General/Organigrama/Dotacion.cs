﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using General.Repositorios;
using System.Reflection;

namespace General
{
    public class Dotacion
    {
        public int IdPersona { get; set; }
        public int Legajo { get; set; }
        public int NroDocumento { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public int IdSexo { get; set; }
        public string Sexo { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public int IdArea { get; set; }
        public string Area { get; set; }
        public string AreaDescripCorta { get; set; }
        public string AreaDescripMedia { get; set; }
        public int IdPlanta { get; set; }
        public string Planta { get; set; }
        public int IdEstudio { get; set; }
        public string NivelEstudio { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int IdSecretaria { get; set; }
        public int IdSubSecretaria { get; set; }
        public string NombreSecretaria { get; set; }
        public string NombresubSecretaria { get; set; }
        public int OrdenArea { get; set; }


        public Dotacion() { }

        public Dotacion(int id_persona, int legajo, int nro_documento, string apellido, string nombre, int id_sexo, string sexo, string nivel, string grado, int id_area, string area, string area_descrip_corta, string area_descrip_media, int id_planta, string planta, int IdEstudio, string NivelEstudio, string Titulo, DateTime FechaNacimiento, int idSecretaria, int idSubSecretaria, string nombre_secretaria, string nombre_subsecretaria, int orden)
        {
            this.IdPersona = id_persona;
            this.Legajo = legajo;
            this.NroDocumento = nro_documento;
            this.Apellido = apellido;
            this.Nombre = nombre;
            this.IdSexo = id_sexo;
            this.Sexo = sexo;
            this.Nivel = nivel;
            this.Grado = grado;
            this.IdArea = id_area;
            this.Area = area;
            this.IdPlanta = id_planta;
            this.Planta = planta;
            this.IdEstudio = IdEstudio;
            this.NivelEstudio = NivelEstudio;
            this.Titulo = Titulo;
            this.FechaNacimiento = FechaNacimiento;
            this.AreaDescripCorta = AreaDescripCorta;
            this.AreaDescripMedia = area_descrip_media;
            this.IdSecretaria = idSecretaria;
            this.IdSubSecretaria = idSubSecretaria;
            this.NombreSecretaria = nombre_secretaria;
            this.NombresubSecretaria = nombre_subsecretaria;
            this.OrdenArea = orden;
        }

        internal int Edad(DateTime fecha)
        {
            DateTime now = fecha;
            int age = now.Year - this.FechaNacimiento.Year;
            if (this.FechaNacimiento > now.AddYears(-age)) age--;

            return age;
        }
    }
}