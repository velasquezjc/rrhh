﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;

namespace General.Repositorios
{
    public class ConexionBDSQL : General.Repositorios.IConexionBD
    {
        private string cadenaDeConexion;
        private SqlConnection conexion;

        public ConexionBDSQL(string StringConnection)
        {
            this.cadenaDeConexion = StringConnection;
        }

        private void AbrirBD()
        {
            conexion = new SqlConnection(this.cadenaDeConexion);
            conexion.Open();
        }

        private void CerrarBD()
        {
            conexion.Close();
            conexion.Dispose();
        }

        public bool EjecutarSinResultado(string nombreProcedimiento)
        {
            AbrirBD();
            var un_comando = CrearComando(nombreProcedimiento);
            try
            {
                //un_comando.Transaction = conexion.BeginTransaction(IsolationLevel.ReadUncommitted);
                un_comando.ExecuteNonQuery();
                //un_comando.Transaction.Commit();
                CerrarBD();
                return true;
            }
            catch (Exception Ex)
            {
                CerrarBD();
                throw Ex;
            }



        }

        public bool EjecutarSinResultado(string nombreProcedimiento, Dictionary<string, object> parametros)
        {
            AbrirBD();
            var un_comando = CrearComando(nombreProcedimiento, parametros);
            try
            {
                //un_comando.Transaction = conexion.BeginTransaction(IsolationLevel.ReadUncommitted);
                un_comando.ExecuteNonQuery();
                //un_comando.Transaction.Commit();
                CerrarBD();
                return true;
            }
            catch (Exception Ex)
            {
                CerrarBD();
                throw Ex;
            }

        }

        public object EjecutarEscalar(string nombreProcedimiento)
        {
            AbrirBD();
            var un_comando = CrearComando(nombreProcedimiento);
            //un_comando.Transaction = conexion.BeginTransaction(IsolationLevel.ReadUncommitted);
            object resultado = un_comando.ExecuteScalar();
            //un_comando.Transaction.Commit();
            CerrarBD();
            return resultado;
        }

        public object EjecutarEscalar(string nombreProcedimiento, Dictionary<string, object> parametros)
        {
            AbrirBD();
            var un_comando = CrearComando(nombreProcedimiento, parametros);
            object resultado = new object();
            try
            {
                //un_comando.Transaction = conexion.BeginTransaction(IsolationLevel.ReadUncommitted);
                resultado = un_comando.ExecuteScalar();
                //un_comando.Transaction.Commit();
            }
            catch (Exception e)
            {

                throw;
            }

            CerrarBD();
            return resultado;
        }

        private SqlCommand CrearComando(string nombreProcedimiento, int command_timeout = 30)
        {
            var un_comando = conexion.CreateCommand();
            un_comando.CommandText = nombreProcedimiento;
            un_comando.CommandType = System.Data.CommandType.StoredProcedure;
            un_comando.CommandTimeout = command_timeout;
            return un_comando;
        }

        private SqlCommand CrearComando(string nombreProcedimiento, Dictionary<string, object> parametros, int command_timeout = 30)
        {
            var un_comando = CrearComando(nombreProcedimiento, command_timeout);
            parametros.Keys.ToList().ForEach(k => un_comando.Parameters.Add(new SqlParameter(k, parametros[k])));
            return un_comando;
        }

        public TablaDeDatos Ejecutar(string procedimiento, Dictionary<string, object> parametros, int segundos_timeout = 30)
        {
            AbrirBD();

            var data_adapter = new SqlDataAdapter();
            var data_table = new TablaDeDatos();

            try
            {
                data_adapter.SelectCommand = CrearComando(procedimiento, parametros, segundos_timeout);
                //data_adapter.SelectCommand = new SqlCommand
                //{
                //    Connection = conexion,
                //    CommandText = procedimiento,
                //    CommandType = CommandType.StoredProcedure
                //};
                //parametros.Keys.ToList().ForEach(k => data_adapter.SelectCommand.Parameters.Add(new SqlParameter(k, parametros[k])));

                //this._paramzetros.ForEach(parametro => data_adapter.SelectCommand.Parameters.Add(parametro));

                data_adapter.Fill(data_table);
                return data_table;
            }
            catch (SqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CerrarBD();
                data_adapter.Dispose();
            }
        }



        public TablaDeDatos Ejecutar(string procedimiento, int command_timeout = 30)
        {
            AbrirBD();

            var data_adapter = new SqlDataAdapter();
            var data_table = new TablaDeDatos();

            try
            {
                data_adapter.SelectCommand = CrearComando(procedimiento, command_timeout);
                data_adapter.Fill(data_table);
                return data_table;
            }
            catch (SqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CerrarBD();
                data_adapter.Dispose();
            }
        }
        /*
        public void PseudoBulk(AnalisisDeLicenciaOrdinaria analisis, Persona persona)
        {
            

            analisis.lineas.ForEach(l=> {
                var un_comando = conexion.CreateCommand();
                un_comando.CommandText = "LIC_GEN_Ins_LogErroresCalculoLicencias";
                un_comando.CommandType = System.Data.CommandType.StoredProcedure;



                var parametros = new Dictionary<string, object>();

                if (error)
                {
                    parametros.Add("@comentario", "Al Actualizar la prórroga dio Error por Exceso de Cantidad de Días permitidos hasta la fecha de la solicitud");
                }
                else
                {
                parametros.Add("@comentario", "");
                /*}

                //Persona a loguear
                parametros.Add("@apellido", "N/A");
                parametros.Add("@nombre", "N/A");
                parametros.Add("@documento", persona.Documento);

                //Licencia con Conflicto
                parametros.Add("@anio_maximo_imputable", null);
                parametros.Add("@anio_minimo_imputable", null);
                parametros.Add("@fecha_desde", l.LicenciaDesde);
                parametros.Add("@fecha_hasta", l.LicenciaHasta);

                
                if (l.PeriodoAutorizado != 0) {
                }

                
                parametros.Add("@anio_imputado", l.PeriodoAutorizado);
                if (ya_imputados)
                {
                    parametros.Add("@cantidad_de_dias", aprobadas.GetDiasYaImputados());
                }
                else
                {
                    parametros.Add("@cantidad_de_dias", aprobadas.CantidadDeDias());
                }

                parametros.Add("@fecha_calculo", fecha_calculo);
                
            });
            */


        public void Bulk(System.Data.DataTable analisis, string table_name)
        {
            AbrirBD();
            SqlBulkCopy bulkCopy =
                new SqlBulkCopy
                (
                    this.conexion,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );


            bulkCopy.DestinationTableName = table_name;
            
            bulkCopy.WriteToServer(analisis);
            this.conexion.Close();

            analisis.Clear();
        }

        public IDbTransaction BeginTransaction()
        {
            conexion = new SqlConnection(this.cadenaDeConexion);
            conexion.Open();
            return conexion.BeginTransaction();
            //_Transaccion = conexion.BeginTransaction();
            //var comando = CrearComando(
           // _Comando.Transaction = _Transaccion;
        }

        //public void RollbackTransaction()
        //{
        //    _Transaccion.Rollback();
        //    CerrarBD();
        //}

        //public void CommitTransaction()
        //{
        //    _Transaccion.Commit();
        //    CerrarBD();
        //}
    }

}

