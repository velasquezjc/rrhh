﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General
{
    public class InstanciasDeEvaluacion
    {
        
        private  int _id;
        private string _descripcion;


        public int Id { get { return _id; } set { _id = value; } }
        public string Descripcion { get { return _descripcion; } set { _descripcion = value; } }
       
        
        public InstanciasDeEvaluacion()
        { }

        public InstanciasDeEvaluacion(int id, string descripcion)
        {
            this._id = id;
            this._descripcion = descripcion;
        }

    }
}
