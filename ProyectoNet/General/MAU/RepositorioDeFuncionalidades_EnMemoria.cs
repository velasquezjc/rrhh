﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdministracionDeUsuarios;


namespace General.MAU
{
    public class RepositorioDeFuncionalidades_EnMemoria:IRepositorioDeFuncionalidades
    {
        private Dictionary<Usuario, List<Funcionalidad>> diccionario_de_funcionalidades;

        public RepositorioDeFuncionalidades_EnMemoria(Dictionary<Usuario, List<Funcionalidad>> diccionario_de_funcionalidades)
        {
            this.diccionario_de_funcionalidades = diccionario_de_funcionalidades;
        }

        public List<Funcionalidad>  FuncionalidadesPara(Usuario usuario)
        {
            return diccionario_de_funcionalidades.GetValueOrDefault(usuario, new List<Funcionalidad>());
        }

        public List<Funcionalidad> FuncionalidadesPara(int id_usuario)
        {
            throw new NotImplementedException();
        }

        public void  ConcederFuncionalidadA(Usuario usuario, Funcionalidad funcionalidad)
        {
            if (diccionario_de_funcionalidades.ContainsKey(usuario))
            {
                if (!diccionario_de_funcionalidades[usuario].Contains(funcionalidad))
                {
                    diccionario_de_funcionalidades[usuario].Add(funcionalidad);
                }
            }
            else
            {
                var funcionalidades = new List<Funcionalidad>(){funcionalidad};
                diccionario_de_funcionalidades.Add(usuario,funcionalidades);
            }
            
        }
        
        public List<Funcionalidad> TodasLasFuncionalidades()
        {
            return diccionario_de_funcionalidades.SelectMany(key => key.Value).Distinct().ToList();
        }

        public void ConcederFuncionalidadA(int id_usuario, int id_funcionalidad)
        {
            throw new NotImplementedException();
        }


        public void DenegarFuncionalidadA(int id_usuario, int id_funcionalidad)
        {
            throw new NotImplementedException();
        }
    }
}