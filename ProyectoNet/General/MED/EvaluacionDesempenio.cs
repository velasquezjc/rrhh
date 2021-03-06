﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.MED
{
    public class EvaluacionDesempenio
    {
        public NivelEvaluacionDesempenio nivel { get; set; }
        public int id_evaluacion { get; set; }
        public int estado_evaluacion { get; set; }
        public string codigo_gde { get; set; }
        public int puntaje
        {
            get
            {
                return this.detalle_preguntas.Sum(p => 5 - p.opcion_elegida);
            }
            set
            {

            }
        }
        public string calificacion { get { return this.nivel.CalificacionPara(this.puntaje); } set { return; } }

        public List<DetallePreguntas> detalle_preguntas { get; set; }

        public EvaluacionDesempenio(int id_evaluacion, int estado_evaluacion, NivelEvaluacionDesempenio nivel, List<DetallePreguntas> detalle_preguntas, string codigo_gde)
        {
            this.id_evaluacion = id_evaluacion;
            this.estado_evaluacion = estado_evaluacion;
            this.codigo_gde = codigo_gde;
            this.detalle_preguntas = detalle_preguntas;
            this.nivel = nivel;
        }

        public EvaluacionDesempenio() { }



        public static EvaluacionDesempenio Nula()
        {
            return new EvaluacionDesempenio(0, 0, NivelEvaluacionDesempenio.Nulo(), new List<DetallePreguntas>(), String.Empty);
        }
    }
}
