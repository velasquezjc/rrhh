using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Services;
using General;
using General.Calendario;
using General.Repositorios;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using General.Modi;
using System.Net;
using System.Net.Mail;
using System.Xml.Serialization;
using AdministracionDeUsuarios;
using General.Sacc;
using General.Sacc.Seguridad;
using General.MAU;

using General.Postular;
using System.Web;

using System.Data;
using System.IO;
using ClosedXML.Excel;

[WebService(Namespace = "http://wsviaticos.gov.ar/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WSViaticos : System.Web.Services.WebService
{
    public WSViaticos()
    {
    }


    [WebMethod]
    public Usuario[] GetUsuarios()
    {
        RepositorioUsuarios repoUsuarios = new RepositorioUsuarios(Conexion());
        List<Usuario> usuarios = repoUsuarios.GetTodosLosUsuarios();
        Usuario[] retuUsuarios = new Usuario[usuarios.Count];
        for (int i = 0; i < usuarios.Count; i++)
        {
            retuUsuarios[i] = usuarios[i];
        }
        return retuUsuarios;

    }

    #region asistencia

    //INICIO: DDJJ 104 ---------------
    //[WebMethod]
    //public Area[] AreasConDDJJAdministradasPor(Usuario usuario)
    //{
    //    var responsableDDJJ = new ResponsableDDJJ(RepoPermisosSobreAreas(), Autorizador());
    //    return responsableDDJJ.AreasConDDJJAdministradasPor(usuario).ToArray();
    //}

    //[WebMethod]
    //public Area[] AreasSinDDJJInferioresA(Area area)
    //{
    //    var responsableDDJJ = new ResponsableDDJJ(RepoPermisosSobreAreas(), Autorizador());
    //    return responsableDDJJ.AreasSinDDJJInferioresA(area).ToArray(); 
    //}
    [WebMethod]
    public int DiasHabilesEntreFechas(DateTime desde, DateTime hasta)
    {
        var repo = new RepositorioLicencias(Conexion());
        return repo.DiasHabilesEntreFechas(desde, hasta);
    }

    [WebMethod]
    public AreaParaDDJJ104[] GetAreasParaDDJJ104(int mes, int anio, int id_area, Usuario usuario)
    {
        var responsableDDJJ = new ResponsableDDJJ(RepoPermisosSobreAreas(), Autorizador());
        var a = new AreaParaDDJJ104[1];

        if (id_area == 0)
        {
            //Traigo las areas formales
            a = responsableDDJJ.GetAreasParaDDJJ104(mes, anio, id_area, usuario).ToArray();    
        }
        else
        {
            a = responsableDDJJ.GetAreasParaDDJJ104InferioresA(mes, anio, id_area, usuario).ToArray();    
        }

        return a;
    }

    [WebMethod]
    public DDJJ104_2001 GenerarDDJJ104(Area area, int mes, int anio, Usuario usuario)
    {
        RepositorioDDJJ104 ddjj = new RepositorioDDJJ104();
        return ddjj.GenerarDDJJ104(usuario, area, mes, anio);
    }

    [WebMethod]
    public string GetLeyendaAnio(int anio)
    {
        return new RepositorioDeParametrosGenerales(Conexion()).GetLeyendaAnio(anio);
    }

    //[WebMethod]
    //public AreaParaDDJJ104[] ImprimirDDJJ104(List<AreaParaDDJJ104> lista)
    //{
    //    RepositorioDDJJ104 ddjj = new RepositorioDDJJ104();
    //    return ddjj.ImprimirDDJJ104(lista);
    //}

    //[WebMethod]
    //public void MarcarDDJJ104Impresa(int nroDDJJ, int estado)
    //{
    //    var responsableDDJJ = new ResponsableDDJJ(RepoPermisosSobreAreas());
    //    responsableDDJJ.MarcarDDJJ104Impresa(nroDDJJ, estado);
    //}

    [WebMethod]
    public MesDto[] GetMeses()
    {
        List<MesDto> meses = new List<MesDto>();

        DateTime fechaActual = DateTime.Now;
        if (fechaActual.Day > 22)
        {
            meses.Add(new MesDto() { Mes = fechaActual.Month, NombreMes = DateTimeFormatInfo.CurrentInfo.GetMonthName(fechaActual.Month), Anio = fechaActual.Year });
        }

        DateTime fechaAnterior = DateTime.Now.AddMonths(-1);
        meses.Add(new MesDto() { Mes = fechaAnterior.Month, NombreMes = DateTimeFormatInfo.CurrentInfo.GetMonthName(fechaAnterior.Month), Anio = fechaAnterior.Year });        

        return meses.ToArray();
    }

    //[WebMethod]
    //public MesDto[] GetMesesGenerados(AreaParaDDJJ104 ddjj, Usuario usuario)
    //{
    //    var RepositorioDDJJ = new RepositorioDDJJ104();

    //    if (ddjj == null)
    //    {
    //        ddjj = new AreaParaDDJJ104();
    //        ddjj.Mes = 0;
    //        ddjj.Anio = 0;
    //    }
    //    ddjj.Agente = new Persona() { Id = usuario.Id };

    //    List<AreaParaDDJJ104> ListDDJJ = RepositorioDDJJ.GetMesesGenerados(ddjj);

    //    List<MesDto> meses = new List<MesDto>();

    //    foreach (var item in ListDDJJ)
    //    {
    //        meses.Add(new MesDto() { Mes = item.Mes, NombreMes = DateTimeFormatInfo.CurrentInfo.GetMonthName(item.Mes), Anio = item.Anio });
    //    }

    //    return meses.ToArray();
    //}


    //FIN: DDJJ 104 ---------------

    [WebMethod]
    public void EliminarInasistenciaActual(Persona unaPersona)
    {
        RepositorioPersonas repoPersonas = new RepositorioPersonas();
        repoPersonas.EliminarInasistenciaActual(unaPersona);
    }


    [WebMethod]
    public void EliminarPasePendienteAprobacion(int id_pase)
    {
        PaseDeArea un_pase = new PaseDeArea();
        un_pase.Id = id_pase;
        this.EliminarPase(un_pase);
    }

    [WebMethod]
    public void EliminarPase(PaseDeArea unPase)
    {
        RepositorioPasesDeArea repositorio = new RepositorioPasesDeArea();
        repositorio.EliminarSolicitudDePase(unPase);
    }

    [WebMethod]
    public bool CargarPase(PaseDeArea unPase)
    {
        RepositorioPasesDeArea repositorio = new RepositorioPasesDeArea();
        return repositorio.CargarSolicitudDePase(unPase);

    }

    [WebMethod]
    public bool CargarPasePlano(int documento, int idAreaOrigen, int idAreaDestino, int idUsuarioCarga)
    {
        RepositorioPasesDeArea repositorio = new RepositorioPasesDeArea();

        Persona nuevaPersona = new Persona();
        nuevaPersona.Documento = documento;

        Area areaOrigen = new Area(idAreaOrigen, "");
        Area areaDestino = new Area(idAreaDestino, "");

        Usuario usuario = new Usuario();
        usuario.Id = idUsuarioCarga;

        Auditoria auditoria = new Auditoria();
        auditoria.UsuarioDeCarga = usuario;

        PaseDeArea nuevoPase = new PaseDeArea();
        nuevoPase.AreaDestino = areaDestino;
        nuevoPase.AreaOrigen = areaOrigen;
        nuevoPase.Auditoria = auditoria;
        nuevoPase.Persona = nuevaPersona;

        return repositorio.CargarSolicitudDePase(nuevoPase);
    }

    [WebMethod]
    public GraficoSueldo GetReporteSueldos(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        string tipo = ((JValue)criterio_deserializado["tipo"]).ToString();
        int dia = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(0, 2)));
        int mes = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(3, 2)));
        int anio = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(6, 4)));
        bool incluir_dependencias = (bool)((JValue)criterio_deserializado["incluir_dependencias"]);
        DateTime fecha = new DateTime(anio, mes, dia);
        int id_area = (int)((JValue)criterio_deserializado["id_area"]);
        RepositorioDeReportes repositorio = new RepositorioDeReportes(Conexion());
        return repositorio.GetReporteSueldosPorArea(tipo, fecha, id_area, incluir_dependencias);
    }

    [WebMethod]
    public GraficoDotacion GetGrafico(string criterio, Usuario usuario)
    {

        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        string tipo = ((JValue)criterio_deserializado["tipo"]).ToString();
        int dia = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(0, 2)));
        int mes = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(3, 2)));
        int anio = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(6, 4)));
        bool incluir_dependencias = (bool)((JValue)criterio_deserializado["incluir_dependencias"]);
        DateTime fecha = new DateTime(anio, mes, dia);
         int id_area = (int)((JValue)criterio_deserializado["id_area"]);
        RepositorioDeReportes repositorio = new RepositorioDeReportes(Conexion());
        return repositorio.GetGraficoDotacion(tipo, fecha, id_area, incluir_dependencias);

    }




    /*Grafico Excel Sueldo*/

    [WebMethod]
    public string ExcelGeneradoSueldos(string criterio, Usuario usuario)
    {
        try
        {
            var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
            string tipo = ((JValue)criterio_deserializado["tipo"]).ToString();
            int dia = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(0, 2)));
            int mes = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(3, 2)));
            int anio = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(6, 4)));
            bool incluir_dependencias = (bool)((JValue)criterio_deserializado["incluir_dependencias"]);
            DateTime fecha = new DateTime(anio, mes, dia);
            int id_area = (int)((JValue)criterio_deserializado["id_area"]);

            RepositorioDeReportes repositorio = new RepositorioDeReportes(Conexion());

            return repositorio.ExcelGeneradoSueldos(tipo, dia, mes, anio, incluir_dependencias, id_area);
        }
        catch (Exception ex)
        {

            throw ex;
        }


    }


    /**/
    
    /*Grafico Excel*/
    [WebMethod]
    public string ExcelGenerado(string criterio, Usuario usuario)
    {      
            try
            {
                var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
                string tipo = ((JValue)criterio_deserializado["tipo"]).ToString();
                int dia = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(0, 2)));
                int mes = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(3, 2)));
                int anio = Int32.Parse((((JValue)criterio_deserializado["fecha"]).ToString().Substring(6, 4)));
                bool incluir_dependencias = (bool)((JValue)criterio_deserializado["incluir_dependencias"]);
                DateTime fecha = new DateTime(anio, mes, dia);
                int id_area = (int)((JValue)criterio_deserializado["id_area"]);

                RepositorioDeReportes repositorio = new RepositorioDeReportes(Conexion());

                return repositorio.ExcelGenerado(tipo, dia, mes, anio, incluir_dependencias,fecha, id_area);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    



    /*Excel DDJJ104*/
    [WebMethod]
    public string ExcelDDJJ104(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);

        int mes = (int)((JValue)criterio_deserializado["mes"]);
        int anio = (int)((JValue)criterio_deserializado["anio"]);
        int id_area = (int)((JValue)criterio_deserializado["id_area"]);


        AreaParaDDJJ104[] areas = GetAreasParaDDJJ104(mes, anio, id_area, usuario);

        try
        {
           
            DataTable table_resumen = new DataTable();
            table_resumen.TableName = "Areas";

            table_resumen.Columns.Add("Areas");
            table_resumen.Columns.Add("Estado");

            foreach (var item in areas)
            {
                string ColEstado = "";
                string ColNombreArea = "";

                if (item.DDJJ != null)
                {
                    switch (item.DDJJ.Estado)
	                {
                        case 1:
                            ColEstado = "Impresa no recepcionada";
                            break;
                        case 2:
                            ColEstado = "Recepcionada";
                            break;

                        default:
                            ColEstado = "";
                            break;
	                }
                }
                else
                {
                    ColEstado = "Sin Generar";
                }


                //switch (item.Jerarquia)
                //{
                //    case 1000:	//Unidad Ministro
                //        ColNombreArea = item.Nombre;
                //        break;
                //    case 900:	//Secretaría
                //        ColNombreArea = "   " + item.Nombre;
                //        break;
                //    case 800:	//SubSecretaría
                //        ColNombreArea = "       " + item.Nombre;
                //        break;
                //    case 700:	//Dir. Nac/General
                //        ColNombreArea = "           " + item.Nombre;
                //        break;
                //    case 600:	//Dirección
                //        ColNombreArea = "               " + item.Nombre;
                //        break;
                //    case 500:   //Coordinación
                //        ColNombreArea = "                   " + item.Nombre;
                //        break;
                //    case 400:	//Departamento
                //        ColNombreArea = "                       " + item.Nombre;
                //        break;
                //    case 300:	//Lugar de Trabajo
                //        ColNombreArea = "                           " + item.Nombre;
                //        break;
                //}

                
                int EspaciosEnBlanco = (int) Math.Truncate(((decimal)(1000-item.Jerarquia)/20));
                string cadena = "";
                for (int i = 0; i < EspaciosEnBlanco; i++)
                {
                    cadena = cadena + " ";
                }
                ColNombreArea = cadena + item.Nombre;

                table_resumen.Rows.Add(ColNombreArea, ColEstado);
            }

            var workbook = new XLWorkbook();

            var dataTable_resumen = table_resumen;
            
            var ws = workbook.Worksheets.Add("DDJJ104");

            ws.Style.Font.FontSize = 11;
            ws.Style.Font.FontName = "Verdana";

            //ws.Column("A").Width = 115;
            //ws.Column("B").Width = 50;
            
            ws.Cell(1, 1).Value = "AREA:";
            ws.Cell(1, 2).Value = "ESTADO:";

            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 2).Style.Font.Bold = true;

            //ws.Cell(1, 2).Value = fecha.ToShortDateString();
            //ws.Cell(2, 2).Value = area.Nombre.ToUpper();

            //ws.Range(4, 1, 4, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(79, 129, 189);
            //ws.Range(4, 1, 4, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            //ws.Range(4, 1, 4, 3).Style.Font.FontColor = XLColor.White;

            //ws.Cell(4, 1).Value = "Informacion";
            //ws.Cell(4, 2).Value = "Cantidad";
            //ws.Cell(4, 3).Value = "Porcentaje %";

            var rangeWithData = ws.Cell(2, 1).InsertData(dataTable_resumen.AsEnumerable());

            //var lastCell = ws.LastCellUsed();
            //ws.Range(4, 1, lastCell.Address.RowNumber, lastCell.Address.ColumnNumber).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            //ws.Range(4, 1, lastCell.Address.RowNumber, lastCell.Address.ColumnNumber).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

           
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return Convert.ToBase64String(ms.ToArray());
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    /**/
    
    
    [WebMethod]
    public Area[] GetAreas()
    {
        //var repositorio = new RepositorioDeAreas(Conexion());

        var repositorio = new RepositorioDeOrganigrama(Conexion());
        List<Area> areas = repositorio.GetOrganigrama().ObtenerAreas(true);

        Area[] returnAreas = new Area[areas.Count];

        for (int i = 0; i < areas.Count; i++)
        {
            returnAreas[i] = areas[i];
        }
        return returnAreas;
    }

    [WebMethod]
    public AreaArbolDTO GetArbolOrganigrama()
    {
        //var repositorio = new RepositorioDeAreas(Conexion());

        var repositorio = RepositorioDeOrganigrama.NuevoRepositorioOrganigrama(Conexion());
        return repositorio.GetOrganigrama().GetArbol();
    }

    [WebMethod]
    public Area GetAreaSuperiorA(Area unArea)
    {
        RepositorioDeOrganigrama repo = new RepositorioDeOrganigrama(Conexion());

        var organigrama = repo.GetOrganigrama();

        var area = organigrama.AreaSuperiorDe(unArea);
        while (!area.PresentaDDJJ)
            area = organigrama.AreaSuperiorDe(area);

        return area;
    }

    [WebMethod]
    public Area GetAreaPreviaDe(ComisionDeServicio una_comision)
    {
        var repositorio = new RepositorioDeComisionesDeServicio(Conexion());
        return repositorio.GetAreaPreviaDe(una_comision);
    }

    [WebMethod]
    public Area RecargarArea(Area unArea)
    {
        var repositorio = RepositorioDeAreas();
        repositorio.ReloadArea(unArea);
        return unArea;
    }

    [WebMethod]
    public SaldoLicencia GetSaldoLicencia(Persona unaPersona, ConceptoDeLicencia concepto)
    {
        var concepto_subclasificado = concepto.InstanciaDeSubclase();

        DateTime fecha_de_consulta = DateTime.Today;

        ServicioDeLicencias servicioLicencias = new ServicioDeLicencias(RepoLicencias());

        SaldoLicencia saldo = servicioLicencias.GetSaldoLicencia(unaPersona, concepto_subclasificado, fecha_de_consulta, RepositorioDePersonas());

        return saldo;
    }

    [WebMethod]
    public SaldoLicencia GetSaldoLicencia14FoH(Persona unaPersona, ConceptoDeLicencia concepto, DateTime fecha)
    {

        return RepoLicencias().CargarSaldoLicencia14FoHDe(concepto, unaPersona, fecha);


    }

    [WebMethod]
    public bool DiasHabilitadosEntreFechas(DateTime desde, DateTime hasta, int idconcepto)
    {
        RepositorioLicencias repositorio = new RepositorioLicencias(Conexion());

        return repositorio.DiasHabilitadosEntreFechas(desde, hasta, idconcepto);

    }

    [WebMethod]
    public SaldoLicencia GetSaldoLicenciaPlano(int documento, int idConcepto)
    {
        Persona persona = new Persona();
        SaldoLicencia saldo;
        persona.Documento = documento;
        ConceptoDeLicencia concepto = new ConceptoDeLicencia();
        concepto.Id = idConcepto;
        saldo = this.GetSaldoLicencia(persona, concepto);
        return saldo;
    }

    [WebMethod]
    public string CargarLicencia(Licencia unaLicencia)
    {
        RepositorioLicencias repositorio = new RepositorioLicencias(Conexion());
        if (repositorio.GetLicenciasQueSePisanCon(unaLicencia))
            return "Ya existe una licencia en el periodo especificado";
        if (repositorio.GetSolicitudesQueSePisanCon(unaLicencia))
            return "Ya existe una solicitud de licencia en el periodo especificado";

        return repositorio.Guardar(unaLicencia);
    }

    [WebMethod]
    public GrupoConceptosDeLicencia[] GetGruposConceptosLicencia()
    {
        GrupoConceptosDeLicencia grupo = new GrupoConceptosDeLicencia();
        RepositorioConceptosDeLicencia repositorio = new RepositorioConceptosDeLicencia();
        List<GrupoConceptosDeLicencia> grupos = repositorio.GetGruposConceptosLicencia();



        GrupoConceptosDeLicencia[] returnGrupos = new GrupoConceptosDeLicencia[grupos.Count];




        for (int i = 0; i < grupos.Count; i++)
        {
            returnGrupos[i] = grupos[i];
        }
        return returnGrupos;
    }

    [WebMethod]
    public Persona[] GetPersonas(Area unArea, int imprimeplanillafirma )
    {

        RepositorioPersonas repositorio = new RepositorioPersonas();
        List<Persona> personas = repositorio.GetPersonasDelArea(unArea, imprimeplanillafirma);
        Persona[] returnPersonas = new Persona[personas.Count];

        for (int i = 0; i < personas.Count; i++)
        {
            returnPersonas[i] = personas[i];
        }
        return returnPersonas;
    }


    [WebMethod]
    public Persona[] GetPersonasACargo(Area unArea)
    {

        RepositorioPersonas repositorio = new RepositorioPersonas();
        List<Persona> personas = repositorio.GetPersonasDelAreaACargo(unArea);
        Persona[] returnPersonas = new Persona[personas.Count];

        for (int i = 0; i < personas.Count; i++)
        {
            returnPersonas[i] = personas[i];
        }
        return returnPersonas;
    }


    [WebMethod]
    public Persona[] GetAusentesEntreFechasPara(Persona[] personas, DateTime desde, DateTime hasta)
    {
        RepositorioLicencias repositorio = new RepositorioLicencias(Conexion());

        return repositorio.GetAusentesEntreFechasPara(personas.ToList(), desde, hasta).ToArray();
    }


    [WebMethod]
    public void EliminarLicenciaPendienteAprobacion(int id)
    {
        RepositorioLicencias repositorio = new RepositorioLicencias(Conexion());

        repositorio.EliminarLicenciaPendienteAprobacion(id);
    }



    [WebMethod]
    public Persona[] GetPasesEntreFechasPara(Persona[] personas, DateTime desde, DateTime hasta)
    {
        RepositorioLicencias repositorio = new RepositorioLicencias(Conexion());

        return repositorio.GetPasesEntreFechasPara(personas.ToList(), desde, hasta).ToArray();
    }

    #endregion

    #region viaticos

    [WebMethod]
    public void AltaDeComisionesDeServicio(ComisionDeServicio unaComision)
    {
        var repositorio = new RepositorioDeComisionesDeServicio(Conexion());
        List<ComisionDeServicio> comisiones = new List<ComisionDeServicio>();

        comisiones.Add(unaComision);
        repositorio.GuardarComisionesDeServicio(comisiones);
    }


    [WebMethod]
    public void AltaDeComisionDeServicio(ComisionDeServicio unaComision)
    {
        var repositorio = new RepositorioDeComisionesDeServicio(Conexion());

        repositorio.GuardarComisionDeServicio(unaComision);
    }

    [WebMethod]
    public Zona[] ZonasDeViaticos()
    {
        var repositorio = new RepositorioZonas(Conexion());

        List<Zona> zonas = repositorio.GetTodasLasZonas();
        Zona[] dtos = new Zona[zonas.Count];

        for (int i = 0; i < zonas.Count; i++)
        {
            dtos[i] = zonas[i];
        }
        return dtos;
    }

    [WebMethod]
    public Zona GetZonaDe(Provincia provincia)
    {
        var repositorio = new RepositorioZonas(Conexion());
        Zona zona = repositorio.GetZonaFromProvincia(provincia);

        return zona;
    }


    [WebMethod]
    public MedioDeTransporte[] MediosDeTransporte()
    {
        RepositorioMediosDeTransporte repositorio = new RepositorioMediosDeTransporte();
        List<MedioDeTransporte> medios = repositorio.GetTodosLosMediosDeTransporte();
        MedioDeTransporte[] dtos = new MedioDeTransporte[medios.Count];

        for (int i = 0; i < medios.Count; i++)
        {
            dtos[i] = medios[i];
        }
        return dtos;
    }

    [WebMethod]
    public MedioDePago[] MediosDePago()
    {
        RepositorioMediosDePago repositorio = new RepositorioMediosDePago();
        List<MedioDePago> medios = repositorio.GetTodosLosMediosDePago();
        MedioDePago[] dtos = new MedioDePago[medios.Count];

        for (int i = 0; i < medios.Count; i++)
        {
            dtos[i] = medios[i];
        }
        return dtos;
    }

    [WebMethod]
    public string CargarLicenciaPlana(int documentoAgente, int idConcepto, string LicenciaDesde, string LicenciaHasta, int idArea, int idUsuarioAuditoria)
    {
        Licencia licencia = new Licencia();
        licencia.Persona = new Persona();
        licencia.Persona.Documento = documentoAgente;
        licencia.Concepto = new ConceptoDeLicencia();
        licencia.Concepto.Id = idConcepto;
        licencia.Desde = DateTime.Parse(LicenciaDesde);
        licencia.Hasta = DateTime.Parse(LicenciaHasta);
        licencia.Auditoria = new Auditoria();
        licencia.Auditoria.UsuarioDeCarga = new Usuario();
        licencia.Auditoria.UsuarioDeCarga.Id = idUsuarioAuditoria;
        licencia.Persona.Area = new Area();
        licencia.Persona.Area.Id = idArea;

        return this.CargarLicencia(licencia);
    }

    //FC: implementar para traer las listas de solicitudes
    [WebMethod]
    public ComisionDeServicio[] GetTodasLasComisionesDeServicios()
    {

        var repositorio = new RepositorioDeComisionesDeServicio(Conexion());

        return repositorio.ObtenerTodosLosViaticos().ToArray();
    }

    [WebMethod]
    public ComisionDeServicio[] GetComisionesDeServicioPorUsuario(Usuario usuario)
    {
        var repositorio = new RepositorioDeComisionesDeServicio(Conexion());
        //Se puede mejorar
        var lista_de_todos_los_viaticos = repositorio.ObtenerTodosLosViaticos();
        var lista_viaticos_usuario = new List<ComisionDeServicio>();
        Autorizador().AreasAdministradasPor(usuario).ForEach(a => lista_viaticos_usuario.AddRange(lista_de_todos_los_viaticos.FindAll(v => v.TransicionesRealizadas.Select(t => t.AreaOrigen.Id).Contains(a.Id) ||
                                                                                                            v.TransicionesRealizadas.Select(t => t.AreaDestino.Id).Contains(a.Id))));

        return lista_viaticos_usuario.Distinct().ToArray();
    }

    [WebMethod]
    public void ReasignarComision(ComisionDeServicio una_comision, Int32 id_area, Int32 id_accion, String comentarios)
    {
        var repo_viaticos = new RepositorioDeComisionesDeServicio(Conexion());
        var repo_areas = new RepositorioDeOrganigrama(Conexion());
        //Se cambio el Repositorio de Áreas por el Repositorio de Organigrama para refactorizar
        var area = repo_areas.GetAreaById(id_area);
        //Se puede mejorar
        repo_viaticos.ReasignarComision(una_comision, area, id_accion, comentarios);
    }

    [WebMethod]
    public List<AccionDeTransicion> GetAccionesDeTransicion()
    {
        var repositorio = new RepositorioDeAccionesDeTransicion();

        var acciones = repositorio.GetAccionesDeTransicion();
        return acciones;
    }

    [WebMethod]
    public List<AccionDeTransicion> GetAccionesDeTransicionParaUnViaticoEnCirculacion()
    {
        var repositorio = new RepositorioDeAccionesDeTransicion();

        var acciones = repositorio.GetAccionesDeTransicionParaUnViaticoEnCirculacion();
        return acciones;
    }

    [WebMethod]
    public Decimal CalcularViaticoPara(Estadia estadia, Persona persona)
    {
        RepositorioCalculadorDeViatico repo = new RepositorioCalculadorDeViatico();
        RepositorioTipoDeViatico repo_tipoViatico = new RepositorioTipoDeViatico();

        //si no encuentra un tipo de viatico rompe...o.O
        persona.TipoDeViatico = repo_tipoViatico.GetTipoDeViaticoDe(persona);

        return repo.GetValorDeViatico(estadia, persona);

    }

    [WebMethod]
    public float CalcularDiasPara(Estadia estadia)
    {
        CalculadorDeDias calculadorDeDias = new CalculadorDeDias();

        return calculadorDeDias.CalcularDiasDe(estadia);

    }



    [WebMethod]
    public Persona CompletarDatosDeContratacion(Persona persona)
    {
        RepositorioTipoDeViatico repo_tipoViatico = new RepositorioTipoDeViatico();
        return repo_tipoViatico.GetNivelGradoDeContratacionDe(persona);
    }

    [WebMethod]
    public Area SiguientePasoDelCircuitoDelArea(Area area_actual)
    {
        RepositorioDeOrganigrama repo = new RepositorioDeOrganigrama(Conexion());
        var organigrama = repo.GetOrganigrama();
        var excepciones = repo.ExcepcionesDeCircuitoViaticos();
        var area_de_viaticos = new Area(1073, "Área de Viáticos y Pasajes", true);
        var circuito = new CircuitoDeAprobacionDeViatico(organigrama, excepciones, area_de_viaticos);
        return circuito.SiguienteAreaDe(area_actual);
    }

    [WebMethod]
    public bool PuedeGuardarseComision(ComisionDeServicio comision)
    {
        return comision.PuedeGuardarse();
    }

    [WebMethod]
    public List<List<string>> GetComisionesPorFiltro(List<Area> lista_areas, List<Provincia> lista_provincias, DateTime fechaDesde, DateTime fechaHasta, List<ComisionDeServicio> comisiones)
    {
        var generadorDeReporte = new GeneradorDeReportes();

        var filtro_area = new FiltroDeComisiones(lista => lista.FindAll(c => c.TuAreaCreadoraEstaEn(lista_areas)));
        var filtro_provincia = new FiltroDeComisiones(lista => lista.FindAll(c => c.Estadias.Any(e => lista_provincias.Any(p => p.Id == e.Provincia.Id))));
        var filtro_periodo = new FiltroDeComisiones(lista => lista.FindAll(c => c.TenesAlgunaEstadiaEnElPeriodo(fechaDesde, fechaHasta)));
        List<FiltroDeComisiones> filtros = new List<FiltroDeComisiones>() { filtro_area, filtro_provincia, filtro_periodo };

        return generadorDeReporte.ViaticoPorAreasPorProvincia(filtros, comisiones, lista_provincias);

    }

    //El web service acepta metodos sobrecargados??
    [WebMethod]
    public List<List<string>> GetComisionesPorProvincia(List<Provincia> lista_provincias, DateTime fechaDesde, DateTime fechaHasta, List<ComisionDeServicio> comisiones)
    {
        var generadorDeReporte = new GeneradorDeReportes();


        var filtro_provincia = new FiltroDeComisiones(lista => lista.FindAll(c => c.Estadias.Any(e => lista_provincias.Any(p => p.Id == e.Provincia.Id))));
        var filtro_periodo = new FiltroDeComisiones(lista => lista.FindAll(c => c.TenesAlgunaEstadiaEnElPeriodo(fechaDesde, fechaHasta)));
        List<FiltroDeComisiones> filtros = new List<FiltroDeComisiones>() { filtro_provincia, filtro_periodo };

        return generadorDeReporte.ViaticosPorProvincia(filtros, comisiones, lista_provincias);

    }

    [WebMethod]
    public List<List<string>> GetComisionesPorAgente(List<Area> areas_autorizadas, Persona persona, DateTime fechaDesde, DateTime fechaHasta, List<ComisionDeServicio> comisiones)
    {
        var generadorDeReporte = new GeneradorDeReportes();

        var filtro_area = new FiltroDeComisiones(lista => lista.FindAll(c => c.TuAreaCreadoraEstaEn(areas_autorizadas)));
        var filtro_persona = new FiltroDeComisiones(lista => lista.FindAll(c => c.Persona.Documento == persona.Documento));
        var filtro_periodo = new FiltroDeComisiones(lista => lista.FindAll(c => c.TenesAlgunaEstadiaEnElPeriodo(fechaDesde, fechaHasta)));
        List<FiltroDeComisiones> filtros = new List<FiltroDeComisiones>() { filtro_area, filtro_persona, filtro_periodo };

        return generadorDeReporte.ViaticosDePersonaPorAreas(filtros, comisiones);

    }

    #endregion

    #region sicoi


    //[WebMethod]
    //public string GuardarDocumento_Ajax(string documento_DTO, Usuario usuario)
    //{
    //    var propiedades_doc = JsonConvert.DeserializeObject<Dictionary<String, Object>>(documento_DTO);
    //    var documento = new Documento();
    //    documento.extracto = (string)propiedades_doc["extracto"];
    //    documento.tipoDeDocumento = new TipoDeDocumentoSICOI(int.Parse(propiedades_doc["tipo"].ToString()), "");
    //    documento.categoriaDeDocumento = new CategoriaDeDocumentoSICOI(int.Parse(propiedades_doc["categoria"].ToString()), "");
    //    documento.numero = (string)propiedades_doc["numero"];
    //    documento.comentarios = (string)propiedades_doc["comentarios"];

    //    this.GuardarDocumento(documento, 
    //        int.Parse(propiedades_doc["id_area_origen"].ToString()), 
    //        int.Parse(propiedades_doc["id_area_actual"].ToString()),
    //        usuario);
    //    try
    //    {
    //        var id_area_destino = int.Parse(propiedades_doc["id_area_destino"].ToString());
    //        if (id_area_destino >= 0) this.CrearTransicionFuturaParaDocumento(documento.Id, id_area_destino, usuario);
    //    }
    //    catch (Exception e) { }
    //    return JsonConvert.SerializeObject(new {ticket = documento.ticket});
    //}

    [WebMethod]
    public string GuardarDocumento_Ajax(string documento_JSON, Usuario usuario)
    {
        try
        {
            var documento_dto_alta = JsonConvert.DeserializeObject<Documento_DTO_Alta>(documento_JSON);
            var documento = documento_dto_alta.toDocumento();

            this.GuardarDocumento(documento, int.Parse(documento_dto_alta.id_area_origen), int.Parse(documento_dto_alta.id_area_actual), usuario);


            int id_area_destino = -1;
            if (documento_dto_alta.id_area_destino != "")
            {
                id_area_destino = int.Parse(documento_dto_alta.id_area_destino);
                if (id_area_destino >= 0) this.CrearTransicionFuturaParaDocumento(documento.Id, id_area_destino, usuario);
            }

            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "altaDeDocumento.ok",
                ticket = documento.ticket
            });

        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "altaDeDocumento.error",
                error = e.Message
            });
        }
    }

    [WebMethod]
    public Documento GuardarDocumento(Documento un_documento, int id_area_creadora, int id_area_actual, Usuario usuario)
    {
        var conexion = Conexion();
        var repo_documentos = new RepositorioDeDocumentos(conexion);
        var repo_organigrama = RepositorioDeOrganigrama.NuevoRepositorioOrganigrama(conexion);
        var repo_transiciones = new RepositorioMensajeria(conexion, repo_documentos, repo_organigrama);
        var repo_usuarios = new RepositorioUsuarios(conexion);
        var mensajeria = repo_transiciones.GetMensajeria();

        repo_documentos.GuardarDocumento(un_documento, usuario);

        mensajeria.SeEnvioDirectamente(un_documento, repo_organigrama.GetAreaById(id_area_creadora), repo_organigrama.GetAreaById(id_area_actual), DateTime.Now);

        repo_transiciones.GuardarTransicionesDe(mensajeria);
        return un_documento;
    }

    [WebMethod]
    public string GuardarCambiosEnDocumento(int id_documento, int id_area_destino, string comentario, Usuario usuario)
    {
        try
        {
            var conexion = Conexion();
            var repo_documentos = new RepositorioDeDocumentos(conexion);
            var repo_organigrama = new RepositorioDeOrganigrama(conexion);
            var repo_transiciones = new RepositorioMensajeria(conexion, repo_documentos, repo_organigrama);
            var mensajeria = repo_transiciones.GetMensajeria();

            var un_documento = repo_documentos.GetDocumentoPorId(id_documento);
            un_documento.comentarios = comentario;
            repo_documentos.UpdateDocumento(un_documento, usuario);

            var area_destino = new Area();

            if (id_area_destino != -1)
            {
                area_destino = repo_organigrama.GetAreaById(id_area_destino);
                mensajeria.SeEnviaAFuturo(un_documento, Autorizador().AreasAdministradasPor(usuario)[0], area_destino);
            }
            else
            {
                mensajeria.YaNoSeEnviaAFuturo(un_documento);
                repo_transiciones.BorrarTransicionFuturaPara(un_documento);
            }

            repo_transiciones.GuardarTransicionesDe(mensajeria);


            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "guardarDocumento.ok",
                documento = new DocumentoDTO(un_documento, mensajeria)
            });

        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "guardarDocumento.error",
                error = e.Message
            });
        }
    }

    [WebMethod]
    public Documento CrearTransicionFuturaParaDocumento(int id_documento, int id_area_destino, Usuario usuario)
    {
        var conexion = Conexion();
        var repo_documentos = new RepositorioDeDocumentos(conexion);
        var repo_organigrama = new RepositorioDeOrganigrama(conexion);
        var repo_transiciones = new RepositorioMensajeria(conexion, repo_documentos, repo_organigrama);
        var mensajeria = repo_transiciones.GetMensajeria();

        var un_documento = repo_documentos.GetDocumentoPorId(id_documento);
        var area_destino = repo_organigrama.GetAreaById(id_area_destino);

        mensajeria.SeEnviaAFuturo(un_documento, Autorizador().AreasAdministradasPor(usuario)[0], area_destino);

        repo_transiciones.GuardarTransicionesDe(mensajeria);
        return un_documento;
    }

    [WebMethod]
    public List<Area> AreasCompletas()
    {

        List<Area> areas = new List<Area>();
        var repositorio = RepositorioDeAreas();

        return areas = repositorio.GetTodasLasAreasCompletas();
    }

    [WebMethod]
    public string AreasFormalesConInformales_JSON()
    {
        var repositorio = new RepositorioDeOrganigrama(Conexion());
        var areasFormales = repositorio.GetOrganigrama().ObtenerAreas(true);
        var areasFormales_dto = new List<Object>();
        areasFormales.ForEach(delegate(Area a)
        {
            areasFormales_dto.Add(new AreaDTO(a));
        });
        return JsonConvert.SerializeObject(areasFormales_dto);
    }

    [WebMethod]
    public List<Area> AreasInferioresDe(Area area)
    {
        List<Area> areas = new List<Area>();
        var repositorio = new RepositorioDeOrganigrama(Conexion());
        return areas = repositorio.GetOrganigrama().GetAreasInferioresDelArea(area);
    }

    [WebMethod]
    public Documento[] GetTodosLosDocumentos()
    {
        var repositorio = new RepositorioDeDocumentos(Conexion());
        return repositorio.GetTodosLosDocumentos().ToArray();
    }

    [WebMethod]
    public TipoDeDocumentoSICOI[] TiposDeDocumentosSICOI()
    {
        var repositorio = new RepositorioDeDocumentos(Conexion());
        return repositorio.GetTiposDeDocumentos().ToArray();
    }

    [WebMethod]
    public CategoriaDeDocumentoSICOI[] CategoriasDocumentosSICOI()
    {
        var repositorio = new RepositorioDeDocumentos(Conexion());
        var categorias_ordenadas = repositorio.GetCategoriasDeDocumentos();
        categorias_ordenadas.Sort(delegate(CategoriaDeDocumentoSICOI c1, CategoriaDeDocumentoSICOI c2)
            {
                return c1.descripcion.CompareTo(c2.descripcion);
            }
        );
        return categorias_ordenadas.ToArray();
    }

    [WebMethod]
    public List<DocumentoDTO> GetDocumentosFiltrados(String filtros)
    {
        var filtrosDesSerializados = desSerializadorDeFiltros().DesSerializarFiltros(filtros);
        var documentos = RepositorioDocumentos().GetDocumentosFiltrados(filtrosDesSerializados);
        var documentos_dto = new List<DocumentoDTO>();
        var mensajeria = Mensajeria();
        if (documentos.Count > 50) documentos.RemoveRange(51, documentos.Count - 51);
        documentos.ForEach(delegate(Documento doc)
        {
            documentos_dto.Add(new DocumentoDTO(doc, mensajeria));
        });

        return documentos_dto;
    }

    [WebMethod]
    public string GetDocumentosEnAlerta()
    {
        var documentos = DocumentosEnAlerta();
        var documentos_dto = new List<Object>();
        var mensajeria = Mensajeria();

        documentos.ForEach(delegate(Documento doc)
        {
            documentos_dto.Add(new DocumentoDTO(doc, mensajeria));
        });
        return JsonConvert.SerializeObject(documentos_dto);
    }

    private List<Documento> DocumentosEnAlerta()
    {
        var alertas = new RepositorioDeAlertas(Mensajeria()).GetAlertas();
        var documentos = new List<Documento>();

        alertas.ForEach(a => documentos = RepositorioDocumentos().GetDocumentosFiltrados(a));

        documentos = documentos.Distinct().ToList();
        return documentos;
    }

    [WebMethod]
    public Boolean HayDocumentosEnAlerta()
    {
        return DocumentosEnAlerta().Any();
    }

    [WebMethod]
    public void IniciarServicioDeAlertas(string HtmlHead, string HtmlBody)
    {
        reportadorDeDocumentosEnAlerta().start(HtmlHead, HtmlBody);
    }

    [WebMethod]
    public void DetenerServicioDeAlertas()
    {
        reportadorDeDocumentosEnAlerta().stop();
    }

    [WebMethod]
    public string EstadoServicioDeAlertas()
    {
        return reportadorDeDocumentosEnAlerta().estado;
    }

    private ReportadorDeDocumentosEnAlerta reportadorDeDocumentosEnAlerta()
    {
        if (Application["reportadorDeDocumentosEnAlerta"] == null)
        {
            var filtros = new List<FiltroDeDocumentos>();
            filtros.Add(new FiltroDeDocumentosPorAreaActual(Mensajeria(), 1));
            filtros.Add(new FiltroDeDocumentosPorTipoDocumento(39));
            var enviador = new EnviadorDeMails();
            Application["reportadorDeDocumentosEnAlerta"] = new ReportadorDeDocumentosEnAlerta(filtros, "arielzambrano@gmail.com", enviador, RepositorioDocumentos());
        }
        return (ReportadorDeDocumentosEnAlerta)Application["reportadorDeDocumentosEnAlerta"];
    }

    private DesSerializadorDeFiltros desSerializadorDeFiltros()
    {
        return new DesSerializadorDeFiltros(Mensajeria());
    }

    public RepositorioDeDocumentos RepositorioDocumentos()
    {
        return new RepositorioDeDocumentos(Conexion());
    }

    public ConexionBDSQL Conexion()
    {
        return new ConexionBDSQL(ConfigurationManager.ConnectionStrings["SQLConection"].ConnectionString);
    }

    public RepositorioMensajeria RepoMensajeria()
    {
        var conexion = Conexion();
        var repo_documentos = new RepositorioDeDocumentos(conexion);
        var repo_organigrama = RepositorioDeOrganigrama.NuevoRepositorioOrganigrama(conexion);//new RepositorioDeOrganigrama(conexion);
        return new RepositorioMensajeria(conexion, repo_documentos, repo_organigrama);
    }

    public Mensajeria Mensajeria()
    {
        var repo_transiciones = RepoMensajeria();
        return repo_transiciones.GetMensajeria();
    }

    [WebMethod]
    public string TransicionarDocumento(int id_documento, int id_area_origen, int id_area_destino)
    {
        try
        {
            var documento = RepositorioDocumentos().GetTodosLosDocumentos().Find(d => d.Id == id_documento);
            var area_origen = new RepositorioDeOrganigrama(Conexion()).GetAreaById(id_area_origen);
            var area_destino = new RepositorioDeOrganigrama(Conexion()).GetAreaById(id_area_destino);
            var mensajeria = Mensajeria();
            mensajeria.SeEnvioDirectamente(documento, area_origen, area_destino, DateTime.Now);
            RepoMensajeria().GuardarTransicionesDe(mensajeria);
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "envioDeDocumento.ok",
                documento = new DocumentoDTO(documento, mensajeria)
            });
        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "envioDeDocumento.error",
                error = e.Message
            });
        }
    }

    [WebMethod]
    public string TransicionarDocumentoConAreaIntermedia(int id_documento, int id_area_origen, int id_area_intermedia, int id_area_destino)
    {
        try
        {
            var documento = RepositorioDocumentos().GetTodosLosDocumentos().Find(d => d.Id == id_documento);
            var area_origen = new RepositorioDeOrganigrama(Conexion()).GetAreaById(id_area_origen);
            var area_intermedia = new RepositorioDeOrganigrama(Conexion()).GetAreaById(id_area_intermedia);
            var area_destino = new RepositorioDeOrganigrama(Conexion()).GetAreaById(id_area_destino);
            var mensajeria = Mensajeria();
            mensajeria.TransicionarConAreaIntermedia(documento, area_origen, area_intermedia, area_destino, DateTime.Now);
            RepoMensajeria().GuardarTransicionesDe(mensajeria);
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "envioDeDocumento.ok",
                documento = new DocumentoDTO(documento, mensajeria)
            });
        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "envioDeDocumento.error",
                error = e.Message
            });
        }
    }

    [WebMethod]
    public Area EstaEnElArea(Documento documento)
    {
        var mensajeria = Mensajeria();
        return mensajeria.EstaEnElArea(documento);
    }

    #endregion

    #region sacc
    //////////////////////////
    [WebMethod]
    public CalendarioDeCurso GetCalendarioDeCurso(Curso un_curso)
    {
        var manager_de_calendarios = new ManagerDeCalendarios(new CalendarioDeFeriados());
        manager_de_calendarios.AgregarCalendarioPara(un_curso);
        return manager_de_calendarios.CalendarioPara(un_curso);

    }


    public PlanillaMensual GetPlanillaMensual(Curso un_curso, DateTime fecha_desde, DateTime fecha_hasta, CalendarioDeCurso calendario)
    {
        return new GeneradorDePlanillas().GenerarPlanillaMensualPara(un_curso, fecha_desde, fecha_hasta, calendario);
    }

    [WebMethod]
    public MesDto[] GetMesesCursoDto(int id_curso, Usuario usuario)
    {
        var curso = RepositorioDeCursos().GetCursoById(id_curso);
        var mes_inicio = curso.FechaInicio.Month;
        var mes_fin = curso.FechaFin.Month;
        List<MesDto> meses = new List<MesDto>();
        for (int i = mes_inicio; i <= mes_fin; i++)
        {
            meses.Add(new MesDto() { Mes = i, NombreMes = DateTimeFormatInfo.CurrentInfo.GetMonthName(i) });
        }
        return meses.ToArray();
    }

    [WebMethod]
    public PlanillaAsistenciasDto GuardarAsistencias(AcumuladorDto[] asistencias_nuevas_dto, AcumuladorDto[] asistencias_originales_dto, Usuario usuarioLogueado)
    {
        List<AcumuladorAsistencia> asistencias_nuevas = new List<AcumuladorAsistencia>();
        List<AcumuladorAsistencia> asistencias_originales = new List<AcumuladorAsistencia>();

        foreach (var a in asistencias_nuevas_dto)
        {
            if (a.Valor == "-" || a.Valor == "")
                asistencias_nuevas.Add(new AsistenciaDiaNoCursado(a.Id, a.Valor, 0, a.Fecha, a.IdAlumno, a.IdCurso));
            else
                asistencias_nuevas.Add(new AsistenciaDiaCursado(a.Id, a.Valor, 0, a.Fecha, a.IdAlumno, a.IdCurso));
        }
        foreach (var a in asistencias_originales_dto)
        {
            if (a.Valor == "-" || a.Valor == "")
                asistencias_originales.Add(new AsistenciaDiaNoCursado(a.Id, a.Valor, 0, a.Fecha, a.IdAlumno, a.IdCurso));
            else
                asistencias_originales.Add(new AsistenciaDiaCursado(a.Id, a.Valor, 0, a.Fecha, a.IdAlumno, a.IdCurso));
        }
        RepoAsistencias().GuardarAsistencias(asistencias_nuevas, asistencias_originales, usuarioLogueado);
        return null;
    }



    [WebMethod]
    public PlanillaAsistenciasDto GetPlanillaAsistencias(int id_curso, DateTime fecha_desde, DateTime fecha_hasta, Usuario usuario)
    {
        var detalle_asistencias = new List<DetalleAsistenciasDto>();
        var horas_catedra = 0;
        var curso = RepositorioDeCursos().GetCursoById(id_curso);
        var organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();

        DateTime fecha_inicio_planilla = curso.FechaInicio > fecha_desde ? curso.FechaInicio : fecha_desde;
        if (fecha_hasta == DateTime.MinValue)
            fecha_hasta = new DateTime(fecha_inicio_planilla.Year, fecha_inicio_planilla.Month, DateTime.DaysInMonth(fecha_inicio_planilla.Year, fecha_inicio_planilla.Month));
        DateTime fecha_fin_planilla = curso.FechaFin < fecha_hasta ? curso.FechaFin : fecha_hasta;

        var calendario = GetCalendarioDeCurso(curso);
        var planilla_mensual = GetPlanillaMensual(curso, fecha_inicio_planilla, fecha_fin_planilla, calendario);
        var dias_planilla = planilla_mensual.GetDiasDeCursadaEntre(fecha_inicio_planilla, fecha_fin_planilla);
        var dias_curso = planilla_mensual.GetDiasDeCursadaEntre(curso.FechaInicio, curso.FechaFin);

        var asistencias = RepoAsistencias().GetAsistencias();
        var alumnos = curso.Alumnos();
        alumnos = FiltrarAlumnosPorUsuarioLogueado(usuario, alumnos, organigrama, new AutorizadorSacc(Autorizador()));

        foreach (var a in alumnos)
        {
            int asist_per = 0;
            int inasist_per = 0;
            int asist_acum = 0;
            int inasist_acum = 0;
            var asist_dto = new List<AcumuladorDto>();
            //ver asistencias a dto
            var asist = asistencias.FindAll(x => x.IdCurso.Equals(curso.Id) && x.IdAlumno.Equals(a.Id) && x.Fecha >= fecha_inicio_planilla && x.Fecha <= fecha_fin_planilla);
            var asist_totales = asistencias.FindAll(x => x.IdCurso.Equals(curso.Id) && x.IdAlumno.Equals(a.Id) && x.Fecha >= curso.FechaInicio && x.Fecha <= fecha_hasta);
            foreach (var item in asist)
            {
                asist_per = item.AcumularHorasAsistidas(asist_per);
                inasist_per = item.AcumularHorasNoAsistidas(inasist_per);
                //
                asist_dto.Add(new AcumuladorDto() { Id = item.Id, Fecha = item.Fecha, IdAlumno = item.IdAlumno, IdCurso = item.IdCurso, Valor = item.Valor });
            }
            foreach (var item in asist_totales)
            {
                asist_acum = item.AcumularHorasAsistidas(asist_acum);
                inasist_acum = item.AcumularHorasNoAsistidas(inasist_acum);
            }
            var detalle_asist = new DetalleAsistenciasDto()
            {
                IdAlumno = a.Id,
                IdCurso = curso.Id,
                Asistencias = asist_dto.ToArray(),
                AsistenciasPeriodo = asist_per,
                InasistenciasPeriodo = inasist_per,
                AsistenciasTotal = asist_acum,
                InasistenciasTotal = inasist_acum
            };
            detalle_asistencias.Add(detalle_asist);
        }

        var horarios_de_cursada = curso.GetHorariosDeCursada();
        var fechas_planilla = new List<FechaDeCursada>();
        dias_curso.ForEach(d => horas_catedra += horarios_de_cursada.Find(h => h.Dia.Equals(d.DayOfWeek)).HorasCatedra);

        dias_planilla.ForEach(d =>
        {
            var fecha_cursada = new FechaDeCursada()
            {
                Dia = d.ToString("dd"),
                NombreDia = d.ToString("ddd"),
                Fecha = d,
                HorasCatedra = planilla_mensual.Curso.GetHorariosDeCursada().Find(h => h.Dia == d.DayOfWeek).HorasCatedra
            };
            fechas_planilla.Add(fecha_cursada);
        });


        var planilla_asistencias_dto = new PlanillaAsistenciasDto()
        {
            Docente = curso.Docente.Nombre + " " + curso.Docente.Apellido,
            Alumnos = alumnos.ToArray(),
            FechasDeCursada = fechas_planilla.ToArray(),
            HorasCatedra = horas_catedra,
            DetalleAsistenciasPorAlumno = detalle_asistencias.ToArray(),
            CodigoError = 0,
            MensajeError = "",
            Observaciones = curso.Observaciones
        };
        return planilla_asistencias_dto;
    }

    [WebMethod]
    public int GetMaxHorasCatedraCurso(Usuario usuario)
    {
        return RepositorioDeCursos().GetMaxHorasCatedraCurso();
    }

    [WebMethod]
    public string Personas()
    {
        var personas = RepoPersonas().GetPersonas();

        var persoas_dto = new List<Object>();

        if (personas.Count() > 0)
        {
            personas.ForEach(delegate(Persona persona)
            {
                Inasistencia inasistenciadto = new Inasistencia();
                inasistenciadto.Aprobada = persona.Inasistencias.First().Aprobada;
                inasistenciadto.Descripcion = persona.Inasistencias.First().Descripcion;
                inasistenciadto.Desde = persona.Inasistencias.First().Desde;
                inasistenciadto.Hasta = persona.Inasistencias.First().Hasta;
                inasistenciadto.Estado = persona.Inasistencias.First().Estado;

                persoas_dto.Add(new
                {
                    label = persona.Apellido + ", " + persona.Nombre + " (DNI: " + persona.Documento + ")",
                    value = persona.Documento.ToString(),
                    nombre = persona.Apellido + ", " + persona.Nombre,
                    apellido = persona.Apellido,
                    documento = persona.Documento,
                    area = new AreaDTO(persona.Area),
                    inasistencia = inasistenciadto
                });
            });
        }

        return JsonConvert.SerializeObject(persoas_dto);

    }

    [WebMethod]
    public string GetAlumnos(Usuario usuario)
    {
        var alumnos = RepoAlumnos().GetAlumnos();
        Organigrama organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();
        var autorizador = new AutorizadorSacc(Autorizador());
        alumnos = autorizador.FiltrarAlumnosPorUsuario(alumnos, organigrama, usuario);

        var alumnos_dto = new List<Object>();

        if (alumnos.Count() > 0)
        {
            alumnos.ForEach(delegate(Alumno alumno)
            {
                alumnos_dto.Add(new
                {
                    Id = alumno.Id,
                    Nombre = alumno.Nombre,
                    Apellido = alumno.Apellido,
                    Documento = alumno.Documento,
                    Telefono = alumno.Telefono,
                    Mail = alumno.Mail,
                    Direccion = alumno.Direccion,
                    //   Area = AreaDtoPara(alumno.Area),
                    Modalidad = ModalidadPara(alumno.Modalidad),
                    Baja = alumno.Baja,
                });
            });
        }

        return JsonConvert.SerializeObject(alumnos_dto);

    }

    [WebMethod]
    public string GetAlumnoByDNI(int dni)
    {
        var alumno = RepoAlumnos().GetAlumnoByDNI(dni);

        var alumno_dto = new AlumnoDto
        {
            Id = alumno.Id,
            Apellido = alumno.Apellido,
            Nombre = alumno.Nombre,
            Documento = alumno.Documento,
            Areas = alumno.Areas,
            Modalidad = ModalidadPara(alumno.Modalidad),
            Telefono = alumno.Telefono,
            Mail = alumno.Mail,
            Direccion = alumno.Direccion,
            LugarDeTrabajo = alumno.LugarDeTrabajo,
            FechaDeNacimiento = alumno.FechaDeNacimiento.ToShortDateString(),
            EstadoDeAlumno = alumno.EstadoDeAlumno.Descripcion,
            CicloCursado = alumno.CicloCursado.Nombre,
            FechaDeIngreso = alumno.FechaDeIngreso.ToShortDateString(),
            Baja = alumno.Baja

        };

        return JsonConvert.SerializeObject(alumno_dto);
    }

    [WebMethod]
    public List<CursoDto> GetCursosDelAlumno(int dni)
    {
        var alumno = RepoAlumnos().GetAlumnoByDNI(dni);
        var cursos = RepositorioDeCursos().GetCursos();
        var evaluaciones = RepoEvaluaciones().GetEvaluacionesAlumno(alumno);
        var cursos_del_alumno = RepositorioDeCursos().GetCursosParaElAlumno(alumno, cursos);
        var articulador = new Articulador();

        var cursos_dto = new List<CursoDto>();

        if (cursos_del_alumno.Count() > 0)
        {
            cursos_del_alumno.ForEach(delegate(Curso curso)
            {
                var un_curso = new CursoDto();
                un_curso.Id = curso.Id;
                un_curso.Nombre = curso.Nombre;
                un_curso.Materia = curso.Materia;
                un_curso.Docente = curso.Docente;
                un_curso.Alumnos = curso.Alumnos();
                un_curso.EspacioFisico = curso.EspacioFisico;
                un_curso.EstadoDelAlumno = articulador.EstadoDelAlumnoParaElCurso(curso, evaluaciones);
                un_curso.FechaInicio = curso.FechaInicio.ToShortDateString();
                un_curso.FechaFin = curso.FechaFin.ToShortDateString();
                var horarios = new List<HorarioDto>();
                foreach (var h in curso.GetHorariosDeCursada())
                {
                    horarios.Add(new HorarioDto() { NumeroDia = (int)h.Dia, Dia = DateTimeFormatInfo.CurrentInfo.GetDayName(h.Dia), HoraDeInicio = h.HoraDeInicio.ToString().Substring(0, 5), HoraDeFin = h.HoraDeFin.ToString().Substring(0, 5), HorasCatedra = h.HorasCatedra });
                }
                un_curso.Horarios = horarios;
                cursos_dto.Add(un_curso);
            });
        }
        return cursos_dto;

        //return JsonConvert.SerializeObject(alumno_dto);
    }



    //private List<Area> AreasDto(List<Area> areas)
    //{
    //    var lista_area = new List<Area>();
    //    foreach (var a in areas)
    //    {

    //    }
    //    return new 
    //    {
    //        Id = modalidad.Id,
    //        Descripcion = modalidad.Descripcion
    //    };
    //}


    //[WebMethod]
    //public List<Alumno> GetTodosLosAlumnos()
    //{
    //    var alumnos = RepoAlumnos().GetAlumnos();
    //    return alumnos;

    //}

    private object EdificioPara(Edificio edificio)
    {
        return new
        {
            id = edificio.Id,
            nombre = edificio.Nombre,
            direccion = edificio.Direccion
        };
    }

    //private object ObtenerAsistentes(List<Persona> responsables) HACER DPS BEL
    //{

    //    string responsable_to_string = " yoo ";
    //    if (!(responsables == null))
    //    {
    //        foreach (Persona responsable in responsables)
    //        {
    //            responsable_to_string = responsable_to_string + responsable.Apellido + responsable.Nombre + "blah";
    //        }
    //        return responsable_to_string;
    //    }
    //    return responsable_to_string;
    //}

    private object ObtenerResponsable(Responsable responsable)
    {
        return (responsable.Apellido + ", " + responsable.Nombre);
    }


    private object ModalidadPara(Modalidad modalidad)
    {
        return new
        {
            Id = modalidad.Id,
            Descripcion = modalidad.Descripcion
        };
    }

    [WebMethod]
    public Alumno GuardarAlumno(Alumno un_alumno, Usuario usuario)
    {
        var conexion = Conexion();
        var repo_alumnos = RepoAlumnos();

        repo_alumnos.GuardarAlumno(un_alumno, usuario);

        return un_alumno;
    }

    [WebMethod]
    public Materia GuardarMateria(Materia una_materia, Usuario usuario)
    {
        var conexion = Conexion();
        una_materia = RepositorioDeMaterias().GuardarMaterias(una_materia, usuario);

        return una_materia;
    }

    [WebMethod]
    public void ModificarMateria(Materia una_materia, Usuario usuario)
    {
        var conexion = Conexion();
        RepositorioDeMaterias().ActualizarMaterias(una_materia, usuario);
    }

    [WebMethod]
    public bool QuitarMateria(Materia materia, Usuario usuario)
    {
        if (RepositorioDeMaterias().MateriaAsignadaACurso(materia))
        {
            return false;
        }
        RepositorioDeMaterias().QuitarMateria(materia, usuario);
        return true;
    }

    [WebMethod]
    public Docente GuardarDocente(Docente un_docente, Usuario usuario)
    {
        var conexion = Conexion();
        RepositorioDeDocentes().GuardarDocente(un_docente, usuario);

        return un_docente;
    }

    [WebMethod]
    public bool QuitarDocente(Docente docente, Usuario usuario)
    {
        if (RepositorioDeDocentes().DocenteAsignadoACurso(docente))
        {
            return false;
        }
        RepositorioDeDocentes().QuitarDocente(docente, usuario);
        return true;
    }

    [WebMethod]
    public void ActualizarAlumno(Alumno un_alumno, Usuario usuario)
    {
        var conexion = Conexion();
        var repo_alumnos = RepoAlumnos();

        repo_alumnos.ActualizarAlumno(un_alumno, usuario);
    }

    [WebMethod]
    public string InscribirAlumnosACurso(List<Alumno> alumnos_a_inscribir, int idCurso, Usuario usuario)
    {
        var conexion = Conexion();

        try
        {
            Curso curso = RepositorioDeCursos().GetCursoById(idCurso);
            var alumnos_a_procesar = new List<Alumno>();
            var alumnos_desincriptos = new List<Alumno>();
            var alumnos_nuevos = new List<Alumno>();
            var alumnos_que_ya_estaban = new List<Alumno>();

            if (alumnos_a_inscribir.Count == 0)
                alumnos_desincriptos = RepositorioDeCursos().ObtenerAlumnosDelCurso(curso);
            else
            {
                alumnos_nuevos = alumnos_a_inscribir.FindAll(a => !RepositorioDeCursos().ObtenerAlumnosDelCurso(curso).Contains(a));
                alumnos_que_ya_estaban = alumnos_a_inscribir.FindAll(a => RepositorioDeCursos().ObtenerAlumnosDelCurso(curso).Contains(a));
                alumnos_desincriptos = RepositorioDeCursos().ObtenerAlumnosDelCurso(curso).FindAll(a => !alumnos_a_inscribir.Contains(a));
            }
            var asistencias = RepoAsistencias().GetAsistencias();
            var alumnos_que_se_pueden_desinscribir = alumnos_desincriptos.FindAll(a => !asistencias.Exists(asist => asist.IdAlumno == a.Id && asist.IdCurso == idCurso));
            var alumnos_que_no_se_pueden_desinscribir = alumnos_desincriptos.FindAll(a => asistencias.Exists(asist => asist.IdAlumno == a.Id && asist.IdCurso == idCurso));

            alumnos_a_procesar.AddRange(alumnos_nuevos);
            alumnos_a_procesar.AddRange(alumnos_que_ya_estaban);
            alumnos_a_procesar.AddRange(alumnos_que_no_se_pueden_desinscribir);

            RepositorioDeCursos().ActualizarInscripcionesACurso(alumnos_a_procesar, curso, usuario);

            if (alumnos_que_no_se_pueden_desinscribir.Count > 0)
            {
                var alumnos_dto = new List<Object>();

                alumnos_que_no_se_pueden_desinscribir.ForEach(delegate(Alumno alumno)
                {
                    alumnos_dto.Add(new
                    {
                        Id = alumno.Id,
                        Nombre = alumno.Nombre,
                        Apellido = alumno.Apellido,
                        Documento = alumno.Documento,
                        Telefono = alumno.Telefono,
                        Mail = alumno.Mail,
                        Direccion = alumno.Direccion,
                        Modalidad = ModalidadPara(alumno.Modalidad),
                        Baja = alumno.Baja,
                    });
                });

                return JsonConvert.SerializeObject(new { tipoDeRespuesta = "inscripcionAlumno.parcial", alumnos = alumnos_dto });
            }
            else
            {
                return JsonConvert.SerializeObject(new { tipoDeRespuesta = "inscripcionAlumno.ok" });
            }
        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "inscripcionAlumno.error",
                error = e.Message
            });
        }

    }

    [WebMethod]
    public bool QuitarAlumno(Alumno alumno, Usuario usuario)
    {
        if (RepoAlumnos().AlumnoAsignadoACurso(alumno))
        {
            return false;
        }
        RepoAlumnos().QuitarAlumno(alumno, usuario);
        return true;

    }

    [WebMethod]
    public string GetCursoById(int id)
    {
        var curso = RepositorioDeCursos().GetCursoById(id);
        return JsonConvert.SerializeObject(curso);
    }

    [WebMethod]
    public CursoDto GetCursoDtoById(int id, Usuario usuario)
    {
        var curso = this.GetCursosDto(usuario).Find(c => c.Id == id);
        return curso;
    }

    [WebMethod]
    public EspacioFisico GetEspacioFisicoById(int id)
    {
        return RepoEspaciosFisicos().GetEspacioFisicoById(id); //JsonConvert.SerializeObject(espacio_fisico);
    }

    [WebMethod]
    [System.Xml.Serialization.XmlInclude(typeof(DocenteNull))]
    public List<CursoDto> GetCursosDto(Usuario usuario)
    {
        var cursos = new RepositorioDeCursos(Conexion()).GetCursos();
        var organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();
        var autorizador = new AutorizadorSacc(Autorizador());

        cursos = autorizador.FiltrarCursosPorUsuario(cursos, organigrama, usuario);


        var cursos_dto = new List<CursoDto>();

        if (cursos.Count() > 0)
        {
            cursos.ForEach(delegate(Curso curso)
            {
                var un_curso = new CursoDto();
                un_curso.Id = curso.Id;
                un_curso.Nombre = curso.Nombre;
                un_curso.Materia = curso.Materia;
                un_curso.Docente = curso.Docente;
                un_curso.Alumnos = FiltrarAlumnosPorUsuarioLogueado(usuario, curso.Alumnos(), organigrama, autorizador);
                un_curso.EspacioFisico = curso.EspacioFisico;
                un_curso.FechaInicio = curso.FechaInicio.ToShortDateString();
                un_curso.FechaFin = curso.FechaFin.ToShortDateString();
                var horarios = new List<HorarioDto>();
                foreach (var h in curso.GetHorariosDeCursada())
                {
                    horarios.Add(new HorarioDto() { NumeroDia = (int)h.Dia, Dia = DateTimeFormatInfo.CurrentInfo.GetDayName(h.Dia), HoraDeInicio = h.HoraDeInicio.ToString().Substring(0, 5), HoraDeFin = h.HoraDeFin.ToString().Substring(0, 5), HorasCatedra = h.HorasCatedra });
                }
                un_curso.Horarios = horarios;
                cursos_dto.Add(un_curso);
            });
        }
        return cursos_dto;
    }


    [WebMethod]
    public string GetMaterias()
    {
        var materias = RepositorioDeMaterias().GetMaterias();
        var materias_dto = new List<object>();

        if (materias.Count > 0)
        {
            materias.ForEach(delegate(Materia materia)
            {
                materias_dto.Add(new
                {
                    id = materia.Id,
                    nombre = materia.Nombre,
                    modalidad = ModalidadPara(materia.Modalidad),
                    ciclo = materia.Ciclo,
                });
            });
        };
        return JsonConvert.SerializeObject(materias_dto);
    }

    [WebMethod]
    public Docente GetDocenteById(int id)
    {
        var docente = RepositorioDeDocentes().GetDocenteById(id);
        return docente;
    }

    [WebMethod]
    public string GetDocentes()
    {
        var docentes = RepositorioDeDocentes().GetDocentes();
        var docentes_dto = new List<object>();

        if (docentes.Count > 0)
        {
            docentes.ForEach(delegate(Docente docente)
            {
                docentes_dto.Add(new
                {
                    id = docente.Id,
                    dni = docente.Dni,
                    nombre = docente.Nombre,
                    apellido = docente.Apellido,
                    telefono = docente.Telefono,
                    mail = docente.Mail,
                    domicilio = docente.Direccion
                });
            });
        };
        return JsonConvert.SerializeObject(docentes_dto);
    }


    [WebMethod]
    public List<AlumnoDto> ReporteAlumnosPorModalidad(Modalidad modalidad)
    {
        Reportes reportes = new Reportes();
        List<AlumnoDto> alumnos_dto = new List<AlumnoDto>();
        var alumnos_reporte = reportes.ObtenerAlumnosQueEstanCursandoConModalidad(modalidad, RepositorioDeCursos());
        foreach (Alumno alumno in alumnos_reporte)
        {

            var alumno_dto = new AlumnoDto();
            alumno_dto.Id = alumno.Id;
            alumno_dto.Apellido = alumno.Apellido;
            alumno_dto.Nombre = alumno.Nombre;
            alumno_dto.Documento = alumno.Documento;
            alumno_dto.Modalidad = alumno.Modalidad;
            alumno_dto.Telefono = alumno.Telefono;
            alumno_dto.Organismo = alumno.Organismo.Id;

            alumnos_dto.Add(alumno_dto);
        }

        return alumnos_dto;
    }

    //[WebMethod]
    //public List<AlumnoDto> ReporteAlumnosDeCursos(DateTime fecha_desde, DateTime fecha_hasta)
    //{
    //    Reportes reportes = new Reportes();
    //    List<AlumnoDto> alumnos_dto = new List<AlumnoDto>();
    //    var alumnos_reporte = reportes.ObtenerAlumnosDeLosCursos(fecha_desde, fecha_hasta, RepositorioDeCursos());
    //    foreach (Alumno alumno in alumnos_reporte)
    //    {
    //        var alumno_dto = new AlumnoDto();
    //        alumno_dto.Id = alumno.Id;
    //        alumno_dto.Apellido = alumno.Apellido;
    //        alumno_dto.Nombre = alumno.Nombre;
    //        alumno_dto.Documento = alumno.Documento;
    //        alumno_dto.Modalidad = alumno.Modalidad;
    //        alumno_dto.Telefono = alumno.Telefono;
    //        alumno_dto.Organismo = alumno.Organismo.Id;

    //        alumnos_dto.Add(alumno_dto);
    //    }

    //    return alumnos_dto;
    //}

    [WebMethod]
    public List<AlumnoDto> ReporteAlumnos(string fecha_desde, string fecha_hasta, Usuario usuario)
    {
        Reportes reportes = new Reportes();
        List<AlumnoDto> alumnos_dto = new List<AlumnoDto>();
        var organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();

        DateTime fecha_desde_formateada;
        DateTime.TryParse(fecha_desde, out fecha_desde_formateada);

        DateTime fecha_hasta_formateada;
        DateTime.TryParse(fecha_hasta, out fecha_hasta_formateada);

        if (fecha_desde_formateada.Year.Equals(0001))
        {
            fecha_desde_formateada = new DateTime(1900, 01, 01);
        }

        if (fecha_hasta_formateada.Year.Equals(0001))
        {
            fecha_hasta_formateada = new DateTime(1900, 01, 01);
        }
        var alumnos_reporte = reportes.ObtenerAlumnosDeLosCursos(fecha_desde_formateada, fecha_hasta_formateada, RepositorioDeCursos());

        alumnos_reporte = FiltrarAlumnosPorUsuarioLogueado(usuario, alumnos_reporte, organigrama, new AutorizadorSacc(Autorizador()));

        foreach (Alumno alumno in alumnos_reporte)
        {
            var alumno_dto = new AlumnoDto();
            alumno_dto.Id = alumno.Id;
            alumno_dto.Apellido = alumno.Apellido;
            alumno_dto.Nombre = alumno.Nombre;
            alumno_dto.Documento = alumno.Documento;
            alumno_dto.Modalidad = alumno.Modalidad;
            alumno_dto.Telefono = alumno.Telefono;
            alumno_dto.Organismo = alumno.Organismo.Id;
            alumno_dto.CicloCursado = alumno.CicloCursado.Id.ToString();

            alumnos_dto.Add(alumno_dto);
        }

        return alumnos_dto;
    }

    private List<Alumno> FiltrarAlumnosPorUsuarioLogueado(Usuario usuario, List<Alumno> alumnos, Organigrama organigrama, AutorizadorSacc autorizador)
    {
        alumnos = autorizador.FiltrarAlumnosPorUsuario(alumnos, organigrama, usuario);
        return alumnos;
    }




    [WebMethod]
    public Modalidad[] Modalidades()
    {
        return RepoModalidades().GetModalidades().ToArray();
    }

    [WebMethod]
    public Organismo[] Organismos()
    {
        return RepoAlumnos().GetOrganismos().ToArray();
    }

    [WebMethod]
    public Ciclo[] Ciclos()
    {
        return RepositorioDeMaterias().GetCiclos().ToArray();
    }

    [WebMethod]
    public Materia GetMateriaById(int id)
    {
        return RepositorioDeMaterias().GetMateriaById(id);

    }

    [WebMethod]
    public bool QuitarCurso(CursoDto curso, Usuario usuario)
    {
        var un_curso = RepositorioDeCursos().GetCursoById(curso.Id);
        if (un_curso.Alumnos().Count == 0)
            return RepositorioDeCursos().QuitarCurso(un_curso, usuario);
        else return false;
    }

    [WebMethod]
    public bool AgregarCurso(CursoDto curso)
    {
        var un_curso =
            new Curso(curso.Materia, curso.Docente, curso.EspacioFisico, DateTime.Parse(curso.FechaInicio), DateTime.Parse(curso.FechaFin), curso.Observaciones);
        var horarios = curso.Horarios;
        horarios.ForEach(h =>
        {
            un_curso.AgregarHorarioDeCursada(new HorarioDeCursada((DayOfWeek)h.NumeroDia, h.HoraDeInicio, h.HoraDeFin, h.HorasCatedra, h.IdCurso));
        });

        return RepositorioDeCursos().AgregarCurso(un_curso);
    }

    [WebMethod]
    public bool GuardarObservacionesCurso(int id_curso, string observaciones, Usuario usuario)
    {
        var un_curso = RepositorioDeCursos().GetCursoById(id_curso);
        un_curso.Observaciones = observaciones;
        return RepositorioDeCursos().ModificarCurso(un_curso);
    }
    [WebMethod]
    public bool ModificarCurso(CursoDto curso)
    {
        var un_curso =
            new Curso(curso.Id, curso.Materia, curso.Docente, curso.EspacioFisico, DateTime.Parse(curso.FechaInicio), DateTime.Parse(curso.FechaFin), curso.Observaciones)
            {
                Observaciones = curso.Observaciones
            };
        var horarios = curso.Horarios;
        horarios.ForEach(h =>
        {
            un_curso.AgregarHorarioDeCursada(new HorarioDeCursada((DayOfWeek)h.NumeroDia, h.HoraDeInicio, h.HoraDeFin, h.HorasCatedra, h.IdCurso));
        });

        return RepositorioDeCursos().ModificarCurso(un_curso);
    }

    [WebMethod]
    public string GetPersonaByDNI(int dni, Usuario usuario)
    {
        RepositorioDeAlumnos repo = new RepositorioDeAlumnos(Conexion(), RepositorioDeCursos(), RepoModalidades());
        Alumno persona = repo.GetAlumnoByDNI(dni);

        Organigrama organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();
        var autorizador = new AutorizadorSacc(Autorizador());
        var persona_dto = new Object();

        if (!autorizador.AlumnoVisibleParaUsuario(persona, organigrama, usuario))
        {

            throw new Exception();
        }
        else
        {
            persona_dto =
                       new
                       {
                           id = persona.Id,
                           nombre = persona.Nombre,
                           apellido = persona.Apellido,
                           documento = persona.Documento,
                           telefono = persona.Telefono,
                           direccion = persona.Direccion,
                           mail = persona.Mail,
                           //area = persona.Areas,
                           modalidad = persona.Modalidad.Id,
                           baja = persona.Baja,
                       };
        }

        return JsonConvert.SerializeObject(persona_dto);
    }


    [WebMethod]
    public Edificio[] Edificios()
    {
        return RepoEspaciosFisicos().GetEdificios().ToArray();
    }

    //[WebMethod]
    //public void EspacioFisico(EspacioFisico espacio_fisico)
    //{

    //}

    [WebMethod]
    public bool QuitarEspacioFisico(EspacioFisico espacio_fisico, Usuario usuario)
    {
        if (RepoEspaciosFisicos().EspacioFisicoAsignadoACurso(espacio_fisico))
        {
            return false;
        }
        RepoEspaciosFisicos().QuitarEspacioFisico(espacio_fisico, usuario);
        return true;
    }

    [WebMethod]
    public void ModificarEspacioFisico(EspacioFisico espacio_fisico, Usuario usuario)
    {
        var conexion = Conexion();
        RepoEspaciosFisicos().ModificarEspacioFisico(espacio_fisico, usuario);
    }

    [WebMethod]
    public void GuardarEspacioFisico(EspacioFisico un_espacio_fisico, Usuario usuario)
    {
        var conexion = Conexion();
        RepoEspaciosFisicos().ActualizarEspacioFisico(un_espacio_fisico, usuario);
    }

    [WebMethod]
    public string GetEspaciosFisicos(Usuario usuario)
    {

        var espacios_fisicos = new RepositorioDeEspaciosFisicos(Conexion(), RepositorioDeCursos()).GetEspaciosFisicos();
        var organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();
        var autorizador = new AutorizadorSacc(Autorizador());

        espacios_fisicos = autorizador.FiltrarEspaciosFisicosPorUsuario(espacios_fisicos, organigrama, usuario);

        var espacios_fisicos_dto = new List<object>();

        if (espacios_fisicos.Count > 0)
        {
            espacios_fisicos.ForEach(delegate(EspacioFisico espacio_fisico)
            {
                espacios_fisicos_dto.Add(new
                {
                    id = espacio_fisico.Id,
                    aula = espacio_fisico.Aula,
                    edificio = EdificioPara(espacio_fisico.Edificio),
                    capacidad = espacio_fisico.Capacidad
                });
            });
        };
        return JsonConvert.SerializeObject(espacios_fisicos_dto);

    }

    [WebMethod]
    public List<Area> GetAreasParaProtocolo()
    {
        return RepositorioDeAreas().GetAreasParaProtocolo();
    }

    [WebMethod]
    public string GetUsuarioWindows()
    {
        return Environment.UserName;
    }

    [WebMethod]
    public List<Area> GetAreasParaLugaresDeTrabajo()
    {
        return RepositorioDeAreas().GetAreasParaLugaresDeTrabajo();
    }

    [WebMethod]
    public string GetAreasParaProtocoloJSON(Usuario usuario)
    {

        List<Area> areas = RepositorioDeAreas().GetAreasParaProtocolo();
        //var organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();
        //var autorizador = Autorizador();

        //areas = autorizador.FiltrarEspaciosFisicosPorUsuario(areas, organigrama, usuario);

        var areas_dto = new List<object>();

        if (areas.Count > 0)
        {
            areas.ForEach(delegate(Area area)
            {
                areas_dto.Add(new
                {
                    id = area.Id,
                    nombre = area.Nombre,
                    responsable = ObtenerResponsable(area.datos_del_responsable),
                    //asistentes = ObtenerAsistentes(COMPLETAR DPS, CAMBIARRRRRR
                    telefono = ObtenerDato(area, ConstantesDeDatosDeContacto.TELEFONO),
                    fax = ObtenerDato(area, ConstantesDeDatosDeContacto.FAX),
                    mail = ObtenerDato(area, ConstantesDeDatosDeContacto.MAIL),
                    direccion = area.Direccion
                    //ANALIZAR DESPUES
                    //aula = area.Aula,
                    //edificio = EdificioPara(area.Edificio),
                    //capacidad = area.Capacidad
                });
            });
        };
        return JsonConvert.SerializeObject(areas_dto);

    }

    private string ObtenerDato(Area UnArea, int id_dato)
    {
        string datos_listados = "";

        if (UnArea.DatosDeContacto.Any(d => d.Id == id_dato))
        {
            List<DatoDeContacto> datos = UnArea.DatosDeContacto.ToList().FindAll(dc => dc.Id == id_dato);

            foreach (DatoDeContacto dato in datos)
            {
                datos_listados += " " + dato.Dato + " / ";
            }

        }

        if (datos_listados.Length > 2)
        {
            return datos_listados.Substring(0, (datos_listados.Length - 2));
        }
        else
        {
            return datos_listados;
        }
    }

    [WebMethod]
    public ItemDeMenu[] ItemsDelMenu(Usuario usuario, string menu)
    {
        //List<ItemDeMenu> items_permitidos_dto = new List<ItemDeMenu>();
        //var repo_usuarios = new RepositorioUsuarios(Conexion());
        //var items_permitidos = from i in repo_usuarios.AutorizadorPara(usuario).ItemsPermitidos(menu)
        //                       orderby i.Orden
        //                       select i;

        //foreach (var item in items_permitidos)
        //{
        //    items_permitidos_dto.Add(new ItemDeMenu() { NombreItem = item.NombreItem, Url = item.Url });
        //}

        //return items_permitidos_dto.ToArray();
        return new List<ItemDeMenu>().ToArray();
    }

    [WebMethod]
    public RespuestaABusquedaDeLegajos BuscarLegajosParaDigitalizacion(string criterio, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        return servicioDeDigitalizacionDeLegajos().BuscarLegajos(criterio);
    }

    [WebMethod]
    public ImagenModi GetImagenPorId(int id_imagen, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        return servicioDeDigitalizacionDeLegajos().GetImagenPorId(id_imagen);
    }

    [WebMethod]
    public int AgregarImagenSinAsignarAUnLegajo(int id_interna, string nombre_imagen, string bytes_imagen, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        return servicioDeDigitalizacionDeLegajos().AgregarImagenSinAsignarAUnLegajo(id_interna, nombre_imagen, bytes_imagen);
    }

    [WebMethod]
    public int AgregarImagenAUnFolioDeUnLegajo(int id_interna, int numero_folio, string nombre_imagen, string bytes_imagen, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        return servicioDeDigitalizacionDeLegajos().AgregarImagenAUnFolioDeUnLegajo(id_interna, numero_folio, nombre_imagen, bytes_imagen);
    }

    [WebMethod]
    public ImagenModi GetThumbnailPorId(int id_imagen, int alto, int ancho, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        return servicioDeDigitalizacionDeLegajos().GetThumbnailPorId(id_imagen, alto, ancho);
    }

    [WebMethod]
    public int AsignarImagenAFolioDeLegajo(int id_imagen, int nro_folio, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        return servicioDeDigitalizacionDeLegajos().AsignarImagenAFolioDeLegajo(id_imagen, nro_folio, usuario);
    }

    [WebMethod]
    public void AsignarImagenAFolioDeLegajoPasandoPagina(int id_imagen, int nro_folio, int pagina, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        servicioDeDigitalizacionDeLegajos().AsignarImagenAFolioDeLegajoPasandoPagina(id_imagen, nro_folio, pagina, usuario);
    }

    [WebMethod]
    public void AsignarCategoriaADocumento(int id_categoria, string tabla, int id_documento, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        servicioDeDigitalizacionDeLegajos().AsignarCategoriaADocumento(id_categoria, tabla, id_documento, usuario);
    }

    [WebMethod]
    public void DesAsignarImagen(int id_imagen, Usuario usuario)
    {
        if (!Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 2)) throw (new Exception("El usuario no tiene permisos para MODI"));
        servicioDeDigitalizacionDeLegajos().DesAsignarImagen(id_imagen, usuario);
    }

    private ServicioDeDigitalizacionDeLegajos servicioDeDigitalizacionDeLegajos()
    {
        return new ServicioDeDigitalizacionDeLegajos(Conexion());
    }

    #endregion

    #region mau


    [WebMethod]
    public bool Login(string alias, string clave)
    {
        return Autorizador().Login(alias, clave);
    }

    [WebMethod]
    public string CambiarPassword(Usuario usuario, string PasswordActual, string PasswordNuevo)
    {
        var repoUsuarios = RepositorioDeUsuarios();

        if (repoUsuarios.CambiarPassword(usuario.Id, PasswordActual, PasswordNuevo))
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "cambioPassword.ok"
            });
        }
        else
        {
            return JsonConvert.SerializeObject(new
            {
                tipoDeRespuesta = "cambioPassword.error"
                //error = e.Message
            });
        }
    }

    [WebMethod]
    public string ResetearPassword(int id_usuario, Usuario usuario)
    {
        if (Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 25)) return RepositorioDeUsuarios().ResetearPassword(id_usuario);
        else throw new Exception("El usuario no tiene permisos para resetear contraseñas");
    }

    [WebMethod]
    public Usuario GetUsuarioPorAlias(string alias)
    {
        return RepositorioDeUsuarios().GetUsuarioPorAlias(alias);
    }

    [WebMethod]
    public Usuario GetUsuarioPorId(int id_usuario)
    {
        return RepositorioDeUsuarios().GetUsuarioPorId(id_usuario);
    }

    [WebMethod]
    public Usuario GetUsuarioPorIdPersona(int id_persona)
    {
        return RepositorioDeUsuarios().GetUsuarioPorIdPersona(id_persona);
    }

    [WebMethod]
    public Usuario CrearUsuarioPara(int id_persona)
    {
        return RepositorioDeUsuarios().CrearUsuarioPara(id_persona);
    }

    [WebMethod]
    public UsuarioNulo GetUsuarioNulo()
    {
        return new UsuarioNulo();
    }

    [WebMethod]
    public Persona GetPersonaUsuarioLogueado(Usuario usuario)
    {
        return RepositorioDeUsuarios().GetPersonaPorIdUsuario(usuario.Id);
    }

    [WebMethod]
    public bool ElUsuarioPuedeAccederALaURL(Usuario usuario, string url)
    {
        return Autorizador().ElUsuarioPuedeAccederALaURL(usuario, url);
    }

    [WebMethod]
    public Funcionalidad[] TodasLasFuncionalidades()
    {
        var funcionalidades = RepositorioDeFuncionalidades().TodasLasFuncionalidades().ToArray();
        return funcionalidades;
    }


    [WebMethod]
    public bool ElUsuarioTienePermisosPara(int id_usuario, int id_funcionalidad)
    {
        return Autorizador().ElUsuarioTienePermisosPara(id_usuario, id_funcionalidad);

    }

    [WebMethod]
    public bool ElUsuarioLogueadoTienePermisosPara(int id_funcionalidad, Usuario usuario)
    {
        return Autorizador().ElUsuarioTienePermisosPara(usuario.Id, id_funcionalidad);

    }

    [WebMethod]
    public Funcionalidad[] FuncionalidadesPara(int id_usuario)
    {
        var funcionalidades = RepositorioDeFuncionalidadesDeUsuarios().FuncionalidadesPara(id_usuario).ToArray();
        return funcionalidades;
    }

    [WebMethod]
    public Persona[] BuscarPersonas(string criterio)
    {
        var personas = RepositorioDePersonas().BuscarPersonas(criterio).ToArray();
        return personas;
    }

    [WebMethod]
    public Area[] BuscarAreas(string criterio)
    {
        var areas = RepositorioDeAreas().BuscarAreas(criterio).ToArray();
        return areas;
    }

    [WebMethod]
    public Persona[] BuscarPersonasConLegajo(string criterio)
    {
        var personas = RepositorioDePersonas().BuscarPersonasConLegajo(criterio).ToArray();
        return personas;
    }

    [WebMethod]
    public Usuario[] BuscarPersonaDeBajaConPermisos()
    {
        var usuarios = RepositorioDeUsuarios().GetUsuariosConPersonasDeBaja().ToArray();
        return usuarios;
    }

    [WebMethod]
    public Usuario[] BuscarUsuariosPorArea(string nombre_area)
    {
        var usuarios = RepositorioDeUsuarios().GetUsuariosPorArea(nombre_area).ToArray();
        return usuarios;
    }

    
    [WebMethod]
    public Area AreaCompleta(int id_area)
    {
        return RepositorioDeAreas().GetAreaCompletaPorId(id_area);
    }

    [WebMethod]
    public Area[] AreasAdministradasPor(Usuario usuario)
    {
        //GRANI 20151228: Se cambia por un error de referencia circular.
        //return Autorizador().AreasAdministradasPor(usuario).ToArray();

        JsonSerializerSettings settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        var listaDeAreas = Autorizador().AreasAdministradasPor(usuario).ToArray();
        var stringDeAreas = JsonConvert.SerializeObject(listaDeAreas, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
        
        Area[] areas = JsonConvert.DeserializeObject<Area[]>(stringDeAreas);
        return areas;
    }

    //[WebMethod]
    //public string AreasAdministradasPor2(Usuario usuario)
    //{
    //    //return JsonConvert.SerializeObject(blah,settings);
    //    JsonSerializerSettings settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
    //    var blah = Autorizador().AreasAdministradasPor(usuario).ToArray();
    //    return JsonConvert.SerializeObject(blah, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
    //}

    [WebMethod]
    public Area[] AreasAdministradasPorIdUsuario(int id_usuario)
    {
        return Autorizador().AreasAdministradasPor(id_usuario).ToArray();
    }

    [WebMethod]
    public void AsignarAreaAUnUsuario(int id_usuario, int id_area, Usuario usuario)
    {
        if (Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 24)) Autorizador().AsignarAreaAUnUsuario(id_usuario, id_area);
        else throw new Exception("No está habilitado para modificar permisos");
    }

    [WebMethod]
    public void DesAsignarAreaAUnUsuario(int id_usuario, int id_area, Usuario usuario)
    {
        if (Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 24)) Autorizador().DesAsignarAreaAUnUsuario(id_usuario, id_area);
        else throw new Exception("No está habilitado para modificar permisos");
    }

    // [WebMethod]
    //public bool ElUsuarioTienePermisosPara(Usuario usuario, string nombre_funcionalidad)
    //{
    //    return Autorizador().ElUsuarioTienePermisosPara(usuario, nombre_funcionalidad);
    //}

    [WebMethod]
    public void ConcederFuncionalidadA(int id_usuario, int id_funcionalidad, Usuario usuario)
    {
        if (Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 24)) Autorizador().ConcederFuncionalidadA(id_usuario, id_funcionalidad);
        else throw new Exception("No está habilitado para modificar permisos");
    }

    [WebMethod]
    public void DenegarFuncionalidadA(int id_usuario, int id_funcionalidad, Usuario usuario)
    {
        if (Autorizador().ElUsuarioTienePermisosPara(usuario.Id, 24)) Autorizador().DenegarFuncionalidadA(id_usuario, id_funcionalidad);
        else throw new Exception("No está habilitado para modificar permisos");
    }

    [WebMethod]
    public MenuDelSistema GetMenuPara(string nombre_menu, Usuario usuario)
    {
        return Autorizador().GetMenuPara(nombre_menu, usuario);
    }

    #endregion

    #region protocolo

    [WebMethod]
    public Combo[] ObtenerCargosFunciones()
    {
        Combo[] combo = RepositorioDeAreas().ObtenerCargosFunciones().ToArray();
        return combo;
    }

    [WebMethod]
    public Combo[] ObtenerTratamientoPersonas()
    {
        Combo[] combo = RepositorioDeAreas().ObtenerTratamientoPersonas().ToArray();
        return combo;
    }

    [WebMethod]
    public Combo[] ObtenerTratamientoTitulos()
    {
        Combo[] combo = RepositorioDeAreas().ObtenerTratamientoTitulos().ToArray();
        return combo;
    }

    [WebMethod]
    public void ModificarResponsable( Area area, Usuario usuario)
    {
        //RepositorioDeAreas().ModificarResponsable(area, usuario.Id);
    }

    [WebMethod]
    public Combo[] ObtenerEdificiosPorLocalidad(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int id_localidad = (int)((JValue)criterio_deserializado["IdLocalidad"]);
        //int id_provincia = (int)((JValue)criterio_deserializado["IdProvincia"]);
        return RepositorioDeAreas().ObtenerEdificiosPorLocalidad(id_localidad, usuario).ToArray();
    }
    [WebMethod]
    public Combo[] ObtenerEdificiosPorCodigoPostal(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int codigo_postal = (int)((JValue)criterio_deserializado["CodigoPostal"]);
        return RepositorioDeAreas().ObtenerEdificiosPorCodigoPostal(codigo_postal, usuario).ToArray();
    }

    [WebMethod]
    public Edificio ObtenerEdificioPorId(string criterio)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int id_edificio = (int)((JValue)criterio_deserializado["IdEdificio"]);
        return RepositorioDeAreas().ObtenerEdificioPorId(id_edificio);
    }

    [WebMethod]
    public Oficina ObtenerOficinaPorId(string criterio)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int id_oficina = (int)((JValue)criterio_deserializado["IdOficina"]);
        return RepositorioDeAreas().ObtenerOficinaPorId(id_oficina);
    }

    [WebMethod]
    public Combo[] ObtenerOficinaPorEdificio(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int id_edificio = (int)((JValue)criterio_deserializado["IdEdificio"]);
        int id_area = (int)((JValue)criterio_deserializado["IdArea"]);  
        return RepositorioDeAreas().ObtenerOficinaPorEdificio(id_edificio, id_area, usuario).ToArray();
    }

    [WebMethod]
    public int GuardarEdificioPendienteDeAptobacion(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int id_provincia = (int)((JValue)criterio_deserializado["IdProvincia"]);
        string nombre_provincia = (string)((JValue)criterio_deserializado["NombreProvincia"]);
        int id_localiad = (int)((JValue)criterio_deserializado["IdLocalidad"]);
        string nombre_localidad = (string)((JValue)criterio_deserializado["NombreLocalidad"]);
        int codigo_postal = (int)((JValue)criterio_deserializado["CodigoPostal"]);
        string calle = (string)((JValue)criterio_deserializado["Calle"]);
        string numero = (string)((JValue)criterio_deserializado["Numero"]);
       return  RepositorioDeAreas().GuardarEdificioPendienteDeAptobacion(id_provincia, nombre_provincia, id_localiad, nombre_localidad, codigo_postal, calle, numero, usuario);
    }

    [WebMethod]
    public int GuardarOficinaPendienteDeAptobacion(string criterio, Usuario usuario)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int id_edificio = (int)((JValue)criterio_deserializado["IdEdificio"]);
        string piso = (string)((JValue)criterio_deserializado["Piso"]);
        string oficina = (string)((JValue)criterio_deserializado["Oficina"]);
        string uf = (string)((JValue)criterio_deserializado["UF"]);

        return RepositorioDeAreas().GuardarOficinaPendienteDeAptobacion(id_edificio, piso, oficina, uf, usuario);
    }

    
         [WebMethod]
    public Localidad CargarDatosDeCodigoPostal(string criterio)
    {
        var criterio_deserializado = (JObject)JsonConvert.DeserializeObject(criterio);
        int codigo_postal = (int)((JValue)criterio_deserializado["CodigoPostal"]);

        return RepositorioDeAreas().CargarDatosDeCodigoPostal(codigo_postal);
    }

    #endregion

    [WebMethod]
    public bool RefrescarCacheMAU(Usuario usuario)
    {
        RepositorioDeFuncionalidades().Refresh();
        RepositorioDeFuncionalidadesDeUsuarios().Refresh();
        RepositorioDeAccesosAURL.NuevoRepositorioDeAccesosAURL(Conexion(), RepositorioDeFuncionalidades()).Refresh();
        return true;
    }

    [WebMethod]
    public InstanciaDeEvaluacion[] GetInstanciasDeEvaluacion(int id_curso)
    {
        var instancias = RepositorioDeCursos().GetInstanciasDeEvaluacion(id_curso).ToArray();
        return instancias;
    }

    [WebMethod]
    public EvaluacionDto[] GuardarEvaluaciones(EvaluacionDto[] evaluaciones_nuevas_dto, EvaluacionDto[] evaluaciones_originales_dto, Usuario usuario)
    {
        var evaluaciones_no_procesadas = new List<EvaluacionDto>();
        var repo_alumnos = RepoAlumnos();
        var repo_cursos = RepositorioDeCursos();
        var evaluaciones_a_guardar = new List<Evaluacion>();
        foreach (var e in evaluaciones_nuevas_dto)
        {
            var un_curso = repo_cursos.GetCursoById(e.IdCurso);
            var una_instancia = un_curso.Materia.Modalidad.InstanciasDeEvaluacion.Find(i => i.Id == e.IdInstancia);
            var un_alumno = repo_alumnos.GetAlumnoByDNI(e.DNIAlumno);
            var una_calificacion = new CalificacionNoNumerica { Descripcion = e.Calificacion };
            DateTime una_fecha;
            DateTime.TryParse(e.Fecha, out una_fecha);
            evaluaciones_a_guardar.Add(new Evaluacion(e.Id, una_instancia, un_alumno, un_curso, una_calificacion, una_fecha));
        }

        var evaluaciones_originales = new List<Evaluacion>();
        foreach (var e in evaluaciones_originales_dto)
        {
            var un_curso = repo_cursos.GetCursoById(e.IdCurso);
            var una_instancia = un_curso.Materia.Modalidad.InstanciasDeEvaluacion.Find(i => i.Id == e.IdInstancia);
            var un_alumno = repo_alumnos.GetAlumnoByDNI(e.DNIAlumno);
            var una_calificacion = new CalificacionNoNumerica { Descripcion = e.Calificacion };
            DateTime una_fecha;
            DateTime.TryParse(e.Fecha, out una_fecha);
            evaluaciones_originales.Add(new Evaluacion(e.Id, una_instancia, un_alumno, un_curso, una_calificacion, una_fecha));
        }

        var evaluaciones_nuevas_posta = evaluaciones_a_guardar.FindAll(e => e.Calificacion.Descripcion != "" && e.Fecha.Date != DateTime.MinValue);
        var evaluaciones_originales_posta = evaluaciones_originales.FindAll(e => e.Calificacion.Descripcion != "" && e.Fecha.Date != DateTime.MinValue);

        var res = RepoEvaluaciones().GuardarEvaluaciones(evaluaciones_originales_posta, evaluaciones_nuevas_posta, usuario);
        foreach (var e in res)
        {
            evaluaciones_no_procesadas.Add(new EvaluacionDto()
            {
                Id = e.Id,
                DNIAlumno = e.Alumno.Documento,
                IdCurso = e.Curso.Id,
                IdInstancia = e.InstanciaEvaluacion.Id,
                Calificacion = e.Calificacion.Descripcion,
                Fecha = e.Fecha.ToShortDateString(),
                DescripcionInstancia = e.InstanciaEvaluacion.Descripcion
            });
        }
        return evaluaciones_no_procesadas.ToArray();
    }

    [WebMethod]
    public List<FichaAlumnoAsistenciaPorCursoDto> GetAsistenciasDelAlumno(int id_alumno)
    {
        var articulador = new Articulador();
        var detalle_asistencias_alumno_por_curso = new List<FichaAlumnoAsistenciaPorCursoDto>();

        var alumno = RepoAlumnos().GetAlumnoByDNI(id_alumno);
        var total_cursos = RepositorioDeCursos().GetCursos();
        var cursos_del_alumno = RepositorioDeCursos().GetCursosParaElAlumno(alumno, total_cursos);

        var asistencias = RepoAsistencias().GetAsistencias();

        foreach (var curso in cursos_del_alumno)
        {
            //int asist_per = 0;
            //int inasist_per = 0;
            int asist_acum = 0;
            int inasist_acum = 0;
            int dias_no_cursados_acum = 0;
            var asist_dto = new List<AcumuladorDto>();
            var calendario = articulador.CalendarioDelCurso(curso);
            var dias_de_cursada = articulador.GetDiasDeCursadaEntre(curso.FechaInicio, curso.FechaFin, calendario);
            var total_horas_catedra = articulador.TotalDeHorasCatedra(curso, dias_de_cursada);
            //ver asistencias a dto
            //var asist = asistencias.FindAll(x => x.IdCurso.Equals(curso.Id) && x.IdAlumno.Equals(a.Id) && x.Fecha >= fecha_inicio_planilla && x.Fecha <= fecha_fin_planilla);
            var asist_totales = asistencias.FindAll(asis => asis.IdCurso.Equals(curso.Id) && asis.IdAlumno.Equals(alumno.Id));
            //foreach (var item in asist)
            //{
            //    asist_per = item.AcumularHorasAsistidas(asist_per);
            //    inasist_per = item.AcumularHorasNoAsistidas(inasist_per);
            //    //
            //    asist_dto.Add(new AcumuladorDto() { Id = item.Id, Fecha = item.Fecha, IdAlumno = item.IdAlumno, IdCurso = item.IdCurso, Valor = item.Valor });
            //}
            foreach (var item in asist_totales)
            {
                asist_acum = item.AcumularHorasAsistidas(asist_acum);
                inasist_acum = item.AcumularHorasNoAsistidas(inasist_acum);
                if (item.Valor.Equals("-"))
                {
                    dias_no_cursados_acum += 1;
                }

            }

            var detalle_asist = new FichaAlumnoAsistenciaPorCursoDto()
            {
                Materia = curso.Materia.Nombre,
                Ciclo = curso.Materia.Ciclo.Nombre,
                AsistenciasTotal = asist_acum,
                InasistenciasTotal = inasist_acum,
                TotalHorasCatedra = total_horas_catedra,
                FechaInicio = curso.FechaInicio.ToShortDateString(),
                FechaFin = curso.FechaFin.ToShortDateString(),
                DiasSinCursarTotal = dias_no_cursados_acum,
            };

            //var detalle_asist = new DetalleAsistenciasDto()
            //{
            //    IdAlumno = a.Id,
            //    IdCurso = curso.Id,
            //    Asistencias = asist_dto.ToArray(),
            //    AsistenciasPeriodo = asist_per,
            //    InasistenciasPeriodo = inasist_per,
            //    AsistenciasTotal = asist_acum,
            //    InasistenciasTotal = inasist_acum
            //};
            detalle_asistencias_alumno_por_curso.Add(detalle_asist);
        }

        return detalle_asistencias_alumno_por_curso;

    }

    [WebMethod]
    public List<FichaAlumnoEvaluacionPorCursoDto> GetEvaluacionesDeAlumno(int dni)
    {

        var alumno = RepoAlumnos().GetAlumnoByDNI(dni);
        var cursos = RepositorioDeCursos().GetCursos();
        var cursos_del_alumno = RepositorioDeCursos().GetCursosParaElAlumno(alumno, cursos);
        var articulador = new Articulador();

        List<Evaluacion> evaluaciones = RepoEvaluaciones().GetEvaluacionesAlumno(alumno);
        List<FichaAlumnoEvaluacionPorCursoDto> CursosConEvaluacionesDto = new List<FichaAlumnoEvaluacionPorCursoDto>();
        //Curso curso = RepositorioDeCursos().GetCursoById(id_curso);



        foreach (var c in cursos_del_alumno)
        {

            CursosConEvaluacionesDto.Add(new FichaAlumnoEvaluacionPorCursoDto()
            {
                CodigoError = 0,
                MensajeError = "",
                Materia = c.Materia.Nombre,
                Ciclo = c.Materia.Ciclo.Nombre,
                Docente = c.Docente.Nombre,
                CalificacionFinal = articulador.CalificacionDelCurso(c, evaluaciones),
                Estado = articulador.EstadoDelAlumnoParaElCurso(c, evaluaciones),
                FechaFin = c.FechaFin.ToShortDateString(),
                Evaluaciones = EvaluacionesDTOPorCurso(evaluaciones, c).ToArray(),
            });


        }

        return CursosConEvaluacionesDto;
    }

    private List<EvaluacionDto> EvaluacionesDTOPorCurso(List<Evaluacion> evaluaciones, Curso curso)
    {
        List<EvaluacionDto> evaluacionesDto = new List<EvaluacionDto>();

        evaluaciones.FindAll(e => e.Curso.Id.Equals(curso.Id)).ForEach(e =>
        {
            evaluacionesDto.Add(new EvaluacionDto()
            {
                Id = e.Id,
                DNIAlumno = e.Alumno.Documento,
                IdCurso = e.Curso.Id,
                Calificacion = e.Calificacion.Descripcion,
                Fecha = e.Fecha.ToShortDateString(),
                IdInstancia = e.InstanciaEvaluacion.Id,
                DescripcionInstancia = e.InstanciaEvaluacion.Descripcion
            });
        });

        return evaluacionesDto;
    }

    [WebMethod]
    public PlanillaEvaluacionesDto GetPlanillaEvaluaciones(int id_curso, int id_instancia, Usuario usuario)
    {
        var curso = RepositorioDeCursos().GetCursoById(id_curso);
        List<Evaluacion> evaluaciones = RepoEvaluaciones().GetEvaluacionesPorCurso(curso);
        var organigrama = new RepositorioDeOrganigrama(Conexion()).GetOrganigrama();

        List<EvaluacionDto> EvaluacionesDto = new List<EvaluacionDto>();

        evaluaciones.ForEach(e =>
        {
            EvaluacionesDto.Add(new EvaluacionDto()
            {
                Id = e.Id,
                DNIAlumno = e.Alumno.Documento,
                IdCurso = e.Curso.Id,
                Calificacion = e.Calificacion.Descripcion,
                Fecha = e.Fecha.ToShortDateString(),
                IdInstancia = e.InstanciaEvaluacion.Id,
                DescripcionInstancia = e.InstanciaEvaluacion.Descripcion
            });
        });

        var alumnos = FiltrarAlumnosPorUsuarioLogueado(usuario, curso.Alumnos(), organigrama, new AutorizadorSacc(Autorizador())).ToArray();
        var Instancias = curso.Materia.Modalidad.InstanciasDeEvaluacion;
        if (id_instancia > 0)
        {
            Instancias = Instancias.FindAll(i => i.Id.Equals(id_instancia));
        }
        var Calificaciones = evaluaciones.Select(e => e.Calificacion.Descripcion).ToList();

        foreach (var a in alumnos)
        {
            foreach (var i in Instancias)
            {
                if (EvaluacionesDto.FindAll(e => e.DNIAlumno == a.Documento && e.IdInstancia == i.Id).Count == 0)
                {
                    EvaluacionesDto.Add(new EvaluacionDto()
                    {
                        Id = 0,
                        DNIAlumno = a.Documento,
                        IdCurso = id_curso,
                        Calificacion = string.Empty,
                        Fecha = string.Empty,
                        IdInstancia = i.Id,
                        DescripcionInstancia = i.Descripcion
                    });
                }
            }
        }

        var Planilla = new PlanillaEvaluacionesDto()
        {
            CodigoError = 0,
            MensajeError = "",
            Alumnos = alumnos,
            Evaluaciones = EvaluacionesDto.ToArray(),
            Instancias = Instancias.ToArray()
        };

        return Planilla;
    }

    [WebMethod]
    public ObservacionDTO[] GetObservaciones()
    {
        var observaciones_dto = new List<ObservacionDTO>();
        var observaciones = RepositorioDeCursos().GetObservaciones();

        foreach (var o in observaciones)
        {
            observaciones_dto.Add(new ObservacionDTO()
            {
                id = o.Id,
                FechaCarga = o.FechaCarga.ToShortDateString(),
                Relacion = o.Relacion,
                PersonaCarga = o.PersonaCarga,
                Pertenece = o.Pertenece,
                Asunto = o.Asunto,
                ReferenteMDS = o.ReferenteMDS,
                Seguimiento = o.Seguimiento,
                Resultado = o.Resultado,
                FechaResultado = o.FechaResultado.ToShortDateString(),
                ReferenteRespuestaMDS = o.ReferenteRespuestaMDS
            });
        }
        return observaciones_dto.ToArray();
    }

    [WebMethod]
    public ObservacionDTO[] GuardarObservaciones(ObservacionDTO[] observaciones_nuevas_dto, ObservacionDTO[] observaciones_originales_dto, Usuario usuario)
    {

        var observaciones_no_procesadas = new List<ObservacionDTO>();
        var repo_cursos = RepositorioDeCursos();

        var observaciones_a_guardar = new List<Observacion>();
        foreach (var o in observaciones_nuevas_dto)
        {
            DateTime fecha_carga;
            DateTime.TryParse(o.FechaCarga, out fecha_carga);
            DateTime fecha_rta;
            DateTime.TryParse(o.FechaResultado, out fecha_rta);

            if (fecha_carga.Year.Equals(0001))
            {
                fecha_carga = new DateTime(1900, 01, 01);
            }
            if (fecha_rta.Year.Equals(0001))
            {
                fecha_rta = new DateTime(1900, 01, 01);
            }

            observaciones_a_guardar.Add(new Observacion(o.id, fecha_carga, o.Relacion, o.PersonaCarga, o.Pertenece, o.Asunto, o.ReferenteMDS, o.Seguimiento, o.Resultado, fecha_rta, o.ReferenteRespuestaMDS));
        }

        var observaciones_originales = new List<Observacion>();
        foreach (var o in observaciones_originales_dto)
        {
            DateTime fecha_carga;
            DateTime.TryParse(o.FechaCarga, out fecha_carga);
            DateTime fecha_rta;
            DateTime.TryParse(o.FechaResultado, out fecha_rta);
            observaciones_originales.Add(new Observacion(o.id, fecha_carga, o.Relacion, o.PersonaCarga, o.Pertenece, o.Asunto, o.ReferenteMDS, o.Seguimiento, o.Resultado, fecha_rta, o.ReferenteRespuestaMDS));
        }

        var res = repo_cursos.GuardarObservaciones(observaciones_originales, observaciones_a_guardar, usuario);
        foreach (var o in res)
        {
            observaciones_no_procesadas.Add(new ObservacionDTO()
            {
                id = o.Id,
                FechaCarga = o.FechaCarga.ToShortDateString(),
                Relacion = o.Relacion,
                PersonaCarga = o.PersonaCarga,
                Pertenece = o.Pertenece,
                Asunto = o.Asunto,
                ReferenteMDS = o.ReferenteMDS,
                Seguimiento = o.Seguimiento,
                Resultado = o.Resultado,
                FechaResultado = o.FechaResultado.ToShortDateString(),
                ReferenteRespuestaMDS = o.ReferenteRespuestaMDS
            });
        }
        return observaciones_no_procesadas.ToArray();

    }





    #region mau


    [WebMethod]
    public int GetDniPorAlias(string alias)
    {
        return RepositorioDeUsuarios().GetDniPorAlias(alias);
    }

    [WebMethod]
    public bool RecuperarUsuario(string criterio)
    {
        bool ejecucion_ok = Autorizador().RecuperarUsuario(criterio);
        return ejecucion_ok;
    }


    [WebMethod]
    public bool RegistrarNuevoUsuario(AspiranteAUsuario aspirante)
    {
        return Autorizador().RegistrarNuevoUsuario(aspirante);
    }

    [WebMethod]
    public bool VerificarUsuario(int id_usuario, Usuario usuario)
    {
        return Autorizador().VerificarUsuario(id_usuario, usuario);
    }

    #endregion


    #region CV

    [WebMethod]
    [XmlInclude(typeof(CurriculumVitaeNull))]
    public CurriculumVitae GetCurriculum(int id)
    {
        CurriculumVitae curriculum = RepoCurriculum().GetCV(id);

        return curriculum;
    }

    [WebMethod]
    [XmlInclude(typeof(CurriculumVitaeNull))]
    public CurriculumVitae GetCurriculumVersion(int id, DateTime fechaDeLaVersion)
    {
        CurriculumVitae curriculum = RepoCurriculum().GetCV(id, fechaDeLaVersion);

        return curriculum;
    }

    [WebMethod]
    public void GuardarCvDatosPersonales(CvDatosPersonales datosPersonalesDTO, Usuario usuario)
    {
        RepoCurriculum().GuardarCVDatosPersonales(datosPersonalesDTO, usuario);
    }

    [WebMethod]
    public Postulacion PostularseA(Postulacion postulacion, Usuario usuario)
    {
        // var postulaciones = new Postulacion();
        return RepoPostulaciones().PostularseA(postulacion, usuario);
    }

    [WebMethod]
    public string GuardarPostulacionManual(string postulacion, string datosPersonales, string folio, Usuario usuario)
    {
        // var postulaciones = new Postulacion();
        return RepoPostulaciones().InscripcionManual(postulacion, datosPersonales, folio, usuario);
    }

    

    [WebMethod]
    public Postulacion[] GetPostulaciones(Usuario usuario)
    {
        return RepoPostulaciones().GetPostulacionesDe(usuario.Owner.Id).ToArray();
    }

    [WebMethod]
    public Postulacion GetPostulacionesPorCodigo(string codigo)
    {
        var postulacion = RepoPostulaciones().GetPostulacionesPorCodigo(codigo);
        /*var usu_etapas = (from etapa in postulacion.Etapas
                          select new
                          {
                              IdUsuario = etapa.IdUsuario,
                              UsuarioEtapa = RepositorioDeUsuarios().GetUsuarioPorIdPersona(etapa.IdUsuario).Owner.Nombre + " " + RepositorioDeUsuarios().GetUsuarioPorIdPersona(etapa.IdUsuario).Owner.Apellido,
                              IdEtapa = etapa.Etapa.Id,
                              Descripcion = etapa.Etapa.Descripcion,
                              Fecha = etapa.Fecha
                          }).ToList();


        var usu = RepositorioDeUsuarios().GetUsuarioPorIdPersona(postulacion.Etapas[0].IdUsuario);
        object datos_postulacion = new
        {
            Postulacion = postulacion,
            UsuarioPostulacion = usu.Owner.Nombre + " " + usu.Owner.Apellido,
            UsuEtapas = usu_etapas
        };
        return Newtonsoft.Json.JsonConvert.SerializeObject(datos_postulacion);*/
        return postulacion;
    }

    [WebMethod(EnableSession = true)]
    public EtapaConcurso[] BuscarEtapasConcurso(string criterio)
    {
        var etapas = RepoPostulaciones().BuscarEtapasConcurso().ToArray();
        return etapas;
    }

    [WebMethod]
    public Comite GetComite(int idComite)
    {
        return RepoComites().GetComiteById(idComite);
    }

    [WebMethod]
    public void InsEtapaPostulacion(int id_postulacion, int id_etapa_postulacion, Usuario usuario)
    {
        RepoPostulaciones().InsEtapaPostulacion(id_postulacion, id_etapa_postulacion, usuario.Id);
    }

    #region CVAntecedentesAcademicos
    [WebMethod]
    public CvEstudios GuardarCvAntecedenteAcademico(CvEstudios antecedentesAcademicos_nuevo, Usuario usuario)
    {
        return (CvEstudios)RepoCurriculum().GuardarItemCV(antecedentesAcademicos_nuevo, usuario);
    }

    [WebMethod]
    public CvEstudios ActualizarCvAntecedenteAcademico(CvEstudios antecedentesAcademicos_nuevo, Usuario usuario)
    {
        return (CvEstudios)RepoCurriculum().ActualizarCv(antecedentesAcademicos_nuevo, usuario);
    }

    [WebMethod]
    public bool EliminarCvAntecedenteAcademico(CvEstudios antecedente_a_borrar, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(antecedente_a_borrar, usuario);
    }
    #endregion

    #region CVActividadesCapacitacion
    [WebMethod]
    public CvCertificadoDeCapacitacion GuardarCvActividadCapacitacion(CvCertificadoDeCapacitacion actividad_capacitacion, Usuario usuario)
    {
        return (CvCertificadoDeCapacitacion)RepoCurriculum().GuardarItemCV(actividad_capacitacion, usuario);
    }

    [WebMethod]
    public CvCertificadoDeCapacitacion ActualizarCvActividadCapacitacion(CvCertificadoDeCapacitacion actividad_capacitacion, Usuario usuario)
    {
        return (CvCertificadoDeCapacitacion)RepoCurriculum().ActualizarCv(actividad_capacitacion, usuario);
    }

    [WebMethod]
    public bool EliminarCvActividadCapacitacion(CvCertificadoDeCapacitacion actividad, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(actividad, usuario);
    }
    #endregion

    #region CVActividadesDocentes
    [WebMethod]
    public CvDocencia GuardarCvActividadDocente(CvDocencia docencia_nuevo, Usuario usuario)
    {
        return (CvDocencia)RepoCurriculum().GuardarItemCV(docencia_nuevo, usuario);
    }

    [WebMethod]
    public CvDocencia ActualizarCvActividadDocente(CvDocencia docencia_nuevo, Usuario usuario)
    {
        return (CvDocencia)RepoCurriculum().ActualizarCv(docencia_nuevo, usuario);
    }

    [WebMethod]
    public bool EliminarCvActividadDocente(CvDocencia actividades_docentes_a_borrar, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(actividades_docentes_a_borrar, usuario);
    }

    #endregion

    #region CvEventoAcademico
    [WebMethod]
    public CvEventoAcademico GuardarCvEventoAcademico(CvEventoAcademico eventoAcademico_nuevo, Usuario usuario)
    {
        return (CvEventoAcademico)RepoCurriculum().GuardarItemCV(eventoAcademico_nuevo, usuario);
    }

    [WebMethod]
    public CvEventoAcademico ActualizarCvEventoAcademico(CvEventoAcademico eventoAcademico_nuevo, Usuario usuario)
    {
        return (CvEventoAcademico)RepoCurriculum().ActualizarCv(eventoAcademico_nuevo, usuario);
    }

    [WebMethod]
    public bool EliminarCvEventosAcademicos(CvEventoAcademico evento, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(evento, usuario);
    }


    [WebMethod]
    public CVTiposDeEventoAcademico[] BuscarTiposEventosAcademicos(string criterio)
    {
        return RepositorioDeTiposDeEventoAcademico.Nuevo(Conexion()).Find(criterio).ToArray();
    }


    [WebMethod]
    public CVCaracterDeParticipacionEvento[] BuscarCaracterParticipacionEvento(string criterio, Usuario usuario)
    {
        return RepositorioDeCaracterDeEventoAcademico.Nuevo(Conexion()).Find(criterio).FindAll(i => i.SoloVisiblePara == usuario.Id || i.SoloVisiblePara == -1).ToArray();
    }

    [WebMethod]
    public CVCaracterDeParticipacionEvento AgregarCaracterParticipacionEvento(string descripcion, Usuario usuario)
    {
        var obj = new CVCaracterDeParticipacionEvento();
        obj.Descripcion = descripcion;
        obj.SoloVisiblePara = usuario.Id;
        return RepositorioDeCaracterDeEventoAcademico.Nuevo(Conexion()).Guardar(obj);
    }

    [WebMethod]
    public CVInstitucionesEventos[] BuscarInstitucionesEvento(string criterio, Usuario usuario)
    {
        return RepositorioDeInstitucionesEventosAcademicos.Nuevo(Conexion()).Find(criterio).FindAll(i => i.SoloVisiblePara == usuario.Id || i.SoloVisiblePara == -1).ToArray();
    }

    [WebMethod]
    public CVInstitucionesEventos AgregarInstitucionesEvento(string descripcion, Usuario usuario)
    {
        var obj = new CVInstitucionesEventos();
        obj.Descripcion = descripcion;
        obj.SoloVisiblePara = usuario.Id;
        return RepositorioDeInstitucionesEventosAcademicos.Nuevo(Conexion()).Guardar(obj);
    }

    [WebMethod]
    public CVTitulosAntecedentesAcademicos[] BuscarTitulosAntecedentesAcademicos(string criterio, Usuario usuario)
    {
        return RepositorioDeTitulosAntecedentesAcademicos.Nuevo(Conexion()).Find(criterio).FindAll(i => i.SoloVisiblePara == usuario.Id || i.SoloVisiblePara == -1).ToArray();
    }

    [WebMethod]
    public CVTitulosAntecedentesAcademicos AgregarTitulosAntecedentesAcademicos(string descripcion, Usuario usuario)
    {
        var obj = new CVTitulosAntecedentesAcademicos();
        obj.Descripcion = descripcion;
        obj.SoloVisiblePara = usuario.Id;
        return RepositorioDeTitulosAntecedentesAcademicos.Nuevo(Conexion()).Guardar(obj);
    }

    #endregion

    #region CvPublicaciones

    [WebMethod]
    public CvPublicaciones GuardarCvPublicacionesTrabajos(CvPublicaciones publicaciones_nuevas, Usuario usuario)
    {
        return (CvPublicaciones)RepoCurriculum().GuardarItemCV(publicaciones_nuevas, usuario);
    }

    [WebMethod]
    public CvPublicaciones ActualizarCvPublicaciones(CvPublicaciones publicacion_nueva, Usuario usuario)
    {
        return (CvPublicaciones)RepoCurriculum().ActualizarCv(publicacion_nueva, usuario);
    }

    [WebMethod]
    public bool EliminarCvPublicacionesTrabajos(CvPublicaciones publicaciones_trabajos_a_borrar, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(publicaciones_trabajos_a_borrar, usuario);
    }

    #endregion

    #region CVMatriculas
    [WebMethod]
    public CvMatricula GuardarCvMatricula(CvMatricula matricula, Usuario usuario)
    {
        return (CvMatricula)RepoCurriculum().GuardarItemCV(matricula, usuario);
    }

    [WebMethod]
    public CvMatricula ActualizarCvMatricula(CvMatricula matricula, Usuario usuario)
    {
        return (CvMatricula)RepoCurriculum().ActualizarCv(matricula, usuario);
    }

    [WebMethod]
    public bool EliminarCvMatricula(CvMatricula id_matricula, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(id_matricula, usuario);
    }
    #endregion

    #region CvInstituciones
    [WebMethod]
    public CvInstitucionesAcademicas GuardarCvInstitucionAcademica(CvInstitucionesAcademicas competencia_informatica, Usuario usuario)
    {
        return (CvInstitucionesAcademicas)RepoCurriculum().GuardarItemCV(competencia_informatica, usuario);
    }

    [WebMethod]
    public CvInstitucionesAcademicas ActualizarCvInstitucionAcademica(CvInstitucionesAcademicas competencia_informatica, Usuario usuario)
    {
        return (CvInstitucionesAcademicas)RepoCurriculum().ActualizarCv(competencia_informatica, usuario);
    }

    [WebMethod]
    public bool EliminarCvInstitucionAcademica(CvInstitucionesAcademicas id_competencia, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(id_competencia, usuario);
    }
    #endregion

    #region CvExperiencias
    [WebMethod]
    public CvExperienciaLaboral GuardarCvExperienciaLaboral(CvExperienciaLaboral experiencia, Usuario usuario)
    {
        return (CvExperienciaLaboral)RepoCurriculum().GuardarItemCV(experiencia, usuario);
    }

    [WebMethod]
    public CvExperienciaLaboral ActualizarCvExperienciaLaboral(CvExperienciaLaboral experiencia, Usuario usuario)
    {
        return (CvExperienciaLaboral)RepoCurriculum().ActualizarCv(experiencia, usuario);
    }

    [WebMethod]
    public bool EliminarCvExperienciaLaboral(CvExperienciaLaboral id_experiencia, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(id_experiencia, usuario);
    }

    #endregion

    #region CvIdiomas

    [WebMethod]
    public CvIdiomas GuardarCvIdiomaExtranjero(CvIdiomas idioma_extranjero, Usuario usuario)
    {
        return (CvIdiomas)RepoCurriculum().GuardarItemCV(idioma_extranjero, usuario);
    }

    [WebMethod]
    public CvIdiomas ActualizarCvIdiomaExtranjero(CvIdiomas idioma_extranjero, Usuario usuario)
    {
        return (CvIdiomas)RepoCurriculum().ActualizarCv(idioma_extranjero, usuario);
    }

    [WebMethod]
    public bool EliminarCvIdiomaExtranjero(CvIdiomas id_capacidad, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(id_capacidad, usuario);
    }


    #endregion

    #region CvComptenciasInformaticas
    [WebMethod]
    public CvCompetenciasInformaticas GuardarCvCompetenciaInformatica(CvCompetenciasInformaticas competencia_informatica, Usuario usuario)
    {
        return (CvCompetenciasInformaticas)RepoCurriculum().GuardarItemCV(competencia_informatica, usuario);
    }

    [WebMethod]
    public CvCompetenciasInformaticas ActualizarCvCompetenciaInformatica(CvCompetenciasInformaticas competencia_informatica, Usuario usuario)
    {
        return (CvCompetenciasInformaticas)RepoCurriculum().ActualizarCv(competencia_informatica, usuario);
    }

    [WebMethod]
    public bool EliminarCvCompetenciaInformatica(CvCompetenciasInformaticas id_competencia, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(id_competencia, usuario);
    }

    #endregion

    #region CvOtrasCapacidades

    [WebMethod]
    public CvCapacidadPersonal GuardarCvOtraCapacidad(CvCapacidadPersonal capacidad_personal, Usuario usuario)
    {
        return (CvCapacidadPersonal)RepoCurriculum().GuardarItemCV(capacidad_personal, usuario);
    }

    [WebMethod]
    public CvCapacidadPersonal ActualizarCvOtraCapacidad(CvCapacidadPersonal capacidad_personal, Usuario usuario)
    {
        return (CvCapacidadPersonal)RepoCurriculum().ActualizarCv(capacidad_personal, usuario);
    }

    [WebMethod]
    public bool EliminarCvOtraCapacidad(CvCapacidadPersonal id_capacidad, Usuario usuario)
    {
        return RepoCurriculum().EliminarCV(id_capacidad, usuario);
    }
    #endregion

    //[WebMethod]
    //public Puesto[] GetCvPuestos()
    //{
    //    return RepoPuestos().GetPuestos().ToArray();
    //}

    [WebMethod]
    public Perfil[] GetCvPerfiles()
    {
        return RepoPerfiles().GetPerfiles().ToArray();
    }



    [WebMethod]
    public Postulacion GetPostulacionById(int idpersona, int idpostulacion)
    {
        Postulacion postulacion = RepoPostulaciones().GetPostulacionById(idpersona, idpostulacion);
        // postulacion.Perfil.DocumentacionRequerida = RepoPerfiles().GetFoliablesDelPerfil(postulacion.Perfil.Id);
        CurriculumVitae cv = RepoCurriculum().GetCV(idpersona);
        // postulacion.CrearDocumentacionARecibir(postulacion.Perfil.DocumentacionRequerida, cv);

        return postulacion;
    }

    [WebMethod]
    public Postulacion[] BuscarPostulacionesDeInscriptos(int id_comite)
    {

        List<EtapaConcurso> etapas = new List<EtapaConcurso> { ConstantesConcursar.EtapaInscripcionDocumental ,
                                                               ConstantesConcursar.EtapaAdmitidos,
                                                               ConstantesConcursar.EtapaNoAdmitidos
                                                                };

        return RepoPostulaciones().BuscarPostulacionesPorEtapas(id_comite, etapas).ToArray();
    }


    [WebMethod]
    public void GuardarFolios(string nro_inscripcion, int nro_ficha_inscripcion, int nro_foto, int nro_foto_dni, int nro_foto_titulo, int nro_cv, int nro_doc_respaldo, Usuario usuario)
    {

        RepoPostulaciones().GuardarFolios(nro_inscripcion, DateTime.Today, nro_ficha_inscripcion, nro_foto, nro_foto_dni, nro_foto_titulo, nro_cv, nro_doc_respaldo, usuario.Id);
    }

    [WebMethod]
    public Folios ObtenerFolios(string nro_inscripcion, string dni_postulante, string fecha_postulacion)
    {

        return RepoPostulaciones().ObtenerFolios(nro_inscripcion, dni_postulante, fecha_postulacion);
    }

    [WebMethod]
    public void GuardarCambiosEnAdmitidos(List<Postulacion> postulaciones, Usuario usuario)
    {

        RepoPostulaciones().GuardarCambiosEnAdmitidos(postulaciones, usuario.Id);
    }

    [WebMethod]
    public Postulacion[] BuscarPostulacionesDePreInscriptos(int id_comite)
    {
        List<EtapaConcurso> etapas = new List<EtapaConcurso> { ConstantesConcursar.EtapaPreinscripcionDocumental };
        return RepoPostulaciones().BuscarPostulacionesPorEtapas(id_comite, etapas).ToArray();
    }

    [WebMethod]
    public bool GuardarEtapaAPostulaciones(int[] idpostulaciones, int id_etapa_postulacion, Usuario usuario)
    {
        try
        {
            idpostulaciones.ToList().ForEach(id => RepoPostulaciones().InsEtapaPostulacion(id, id_etapa_postulacion, usuario.Id));
        }
        catch (Exception e)
        {

            throw e;
        }

        return true;

    }

    [WebMethod]
    public ResumenDePostulaciones[] GetTableroDeControlDePostulaciones(int id_usuario)
    {

        return RepoPostulaciones().TableroDeControlPostulaciones().ToArray();

    }


    [WebMethod]
    public PantallaRecepcionDocumentacion GetPantallaRecepcionDocumentacion(Postulacion postulacion)
    {
        CurriculumVitae cv = RepoCurriculum().GetCV(postulacion.Postulante.Id);
        //RepositorioDePerfiles repoPerfiles = new RepositorioDePerfiles(Conexion());
        //repoPerfiles.GetRequisitosDelPerfil(postulacion.Perfil.Id).ForEach(r => postulacion.Perfil.Requiere(r));

        RepositorioDeFoliados repo = new RepositorioDeFoliados(Conexion());
        CreadorDePantallas creador = new CreadorDePantallas();
        List<DocumentacionRecibida> listaDocRecibida = repo.GetDocumentacionRecibidaByPostulacion(postulacion);

        PantallaRecepcionDocumentacion pantalla = creador.CrearPantalla(cv, postulacion.Perfil, postulacion, listaDocRecibida);

        return pantalla;
    }

    [WebMethod]
    public bool GuardarDocumentacionRecibida(int id_postulacion, DocumentacionRecibida[] lista_doc_recibida, Usuario usuario)
    {
        RepositorioDeFoliados repo = new RepositorioDeFoliados(Conexion());

        //List<DocumentacionRecibida> listaDocRecibida = repo.GetDocumentacionRecibidaByPostulacion(postulacion);

        repo.GuardarDocumentacionRecibida(lista_doc_recibida.ToList(), usuario);

        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("@idPostulacion", id_postulacion);
        var etapas = RepoPostulaciones().GetPostulaciones(parametros).First().Etapas;

        //VALIDO que hayan documentos para guardar y que la postulacion tenga solo la etapa de preinscripcion
        //Le paso ETAPA 2 que es la de PREINSCRIPCION DOCUMENTAL
        if (etapas.Last().Etapa.Id.Equals(ConstantesConcursar.EtapaPreinscripcionWeb.Id))
        {
            RepoPostulaciones().InsEtapaPostulacion(id_postulacion, 2, usuario.Id);
            return true;
        }
        else
        {
            return false;
        }

    }

    [WebMethod]
    public bool PasarAEtapaInscripto(int id_postulacion, Usuario usuario)
    {

        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("@idPostulacion", id_postulacion);
        var etapas = RepoPostulaciones().GetPostulaciones(parametros).First().Etapas;

        //VALIDO que hayan documentos para guardar y que la postulacion tenga solo la etapa de preinscripcion
        //Le paso ETAPA 2 que es la de PREINSCRIPCION DOCUMENTAL
        if (etapas.Last().Etapa.Id.Equals(ConstantesConcursar.EtapaPreinscripcionWeb.Id))
        {
            RepoPostulaciones().InsEtapaPostulacion(id_postulacion, ConstantesConcursar.EtapaPreinscripcionDocumental.Id, usuario.Id);
            return true;
        }
        else
        {
            return false;
        }

    }


    [WebMethod]
    public bool EliminarPostulacionPorUsuario(Postulacion postulacion, Usuario usuario)
    {
        return RepoPostulaciones().EliminarPostulacionPorUsuario(postulacion, usuario);

    }

    [WebMethod]
    public ModalidadDeInscripcion[] BuscarModalidadDeInscripcion(string criterio)
    {
        var repo = new RepositorioDePostulaciones(Conexion());
        return repo.BuscarModalidadesDeInscripcion().ToArray();
    }

    [WebMethod]
    public Provincia[] BuscarProvincias(string criterio)
    {
        return RepositorioDeProvincias.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public Nacionalidad[] BuscarNacionalidades(string criterio)
    {
        return RepositorioDeNacionalidades.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public NivelDeIdioma[] BuscarNivelesDeIdioma(string criterio)
    {
        return RepositorioDeNivelesDeIdioma.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public CvNivelCompetenciaInformatica[] BuscarNivelCompetenciaInformatica(string criterio)
    {
        return RepositorioDeNivelesCompetenciasInformaticas.Nuevo(Conexion()).Find(criterio).ToArray();
    }


    [WebMethod]
    public CvTipoCompetenciaInformatica[] BuscarTiposCompetenciaInformatica(string criterio)
    {
        return RepositorioDeTiposCompetenciaInformatica.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public CvConocimientoCompetenciaInformatica[] BuscarConocimientoCompetenciaInformatica(string criterio, Usuario usuario)
    {
        return RepositorioDeConocimientosCompetenciasInformaticas.Nuevo(Conexion()).Find(criterio).FindAll(i => i.SoloVisiblePara == usuario.Id || i.SoloVisiblePara == -1).ToArray();
    }

    [WebMethod]
    public CvConocimientoCompetenciaInformatica AgregarConocimientoCompetenciaInformatica(string descripcion, int tipo, Usuario usuario)
    {
        var obj = new CvConocimientoCompetenciaInformatica();
        obj.Descripcion = descripcion;
        obj.SoloVisiblePara = usuario.Id;
        obj.Tipo = tipo;
        return RepositorioDeConocimientosCompetenciasInformaticas.Nuevo(Conexion()).Guardar(obj);
    }

    [WebMethod]
    public CvModalidadContratacion[] BuscarModalidadContratacion(string criterio)
    {
        return RepositorioDeModalidadContratacion.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public CVInstitucionesEventos[] BuscarInstitucionesEventosAcademicos(string criterio)
    {
        return RepositorioDeInstitucionesEventosAcademicos.Nuevo(Conexion()).Find(criterio).ToArray();
    }


    [WebMethod]
    public Pais[] BuscarPaises(string criterio)
    {
        return RepositorioDePaises.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public UnidadDeTiempo[] BuscarUnidadesTiempo(string criterio)
    {
        return RepositorioDeUnidadesDeTiempo.Nuevo(Conexion()).Find(criterio).ToArray();
    }


    [WebMethod]
    public EstadoCivil[] BuscarEstadosCiviles(string criterio)
    {
        return RepositorioDeEstadosCiviles.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public TipoDeDocumento[] BuscarTiposDeDocumento(string criterio)
    {
        return RepositorioDeTiposdeDocumento.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public Sexo[] BuscarSexos(string criterio)
    {
        return RepositorioDeSexos.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public AmbitoLaboral[] BuscarAmbitosLaborales(string criterio)
    {
        return RepositorioDeAmbitoLaboral.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public NivelDeDocencia[] BuscarNivelesDeDocencia(string criterio)
    {
        return RepositorioDeNivelesDeDocencia.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public NivelDeEstudio[] BuscarNivelesDeEstudio(string criterio)
    {
        return RepositorioDeNivelesDeEstudio.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    [WebMethod]
    public Localidad[] BuscarLocalidades(string criterio)
    {
        return RepositorioDeLocalidades.Nuevo(Conexion()).Find(criterio).ToArray();
    }

    #endregion

    #region Formularios
        [WebMethod]
        public Formulario GetFormulario(string criterio, Usuario usuario)
        {
            return new RepositorioDeFormularios(Conexion()).GetFormulario(criterio, usuario);
        }

        [WebMethod]
        public void GuardarCambiosEnFormulario(Formulario form, Usuario usuario)
        {
            new RepositorioDeFormularios(Conexion()).GuardarDatos(form, usuario);
        }

        [WebMethod]
        public void GuardarCabeceraFormulario(Formulario form, Usuario usuario)
        {
            //el true es para poner en impreso
            new RepositorioDeFormularios(Conexion()).GuardarVersion(form, usuario,true);
        }

        [WebMethod]
        public int GetIdCabeceraFormulario(Formulario form, Usuario usuario)
        {
            //el true es para poner en impreso
            return new RepositorioDeFormularios(Conexion()).GetUltimaCabeceraFormulario(form, usuario);
        }
    #endregion

    #region Reportes
        [WebMethod]
        public string GetConsultaRapida(int documento , Usuario usuario)
        {

            return RepositorioDePersonas().GetConsultaRapida(documento);

        }

        [WebMethod]
        public string GetCarreraAdministrativa(int documento, Usuario usuario)
        {

            return RepositorioDePersonas().GetCarreraAdministrativa(documento);

        }

    #endregion


        private RepositorioLicencias RepoLicencias()
    {
        return new RepositorioLicencias(Conexion());
    }

    private RepositorioDeAlumnos RepoAlumnos()
    {
        return new RepositorioDeAlumnos(Conexion(), RepositorioDeCursos(), RepoModalidades());
    }

    private RepositorioDeModalidades RepoModalidades()
    {
        return new RepositorioDeModalidades(Conexion());
    }

    private RepositorioPersonas RepoPersonas()
    {
        return new RepositorioPersonas();
    }

    private RepositorioDeMaterias RepositorioDeMaterias()
    {
        return new RepositorioDeMaterias(Conexion(), RepositorioDeCursos(), RepoModalidades());
    }

    private RepositorioDeCursos RepositorioDeCursos()
    {
        return new RepositorioDeCursos(Conexion());
    }

    private RepositorioDePersonas RepositorioDePersonas()
    {
        return General.Repositorios.RepositorioDePersonas.NuevoRepositorioDePersonas(Conexion());
    }

    private RepositorioDeAreas RepositorioDeAreas()
    {
        return General.Repositorios.RepositorioDeAreas.NuevoRepositorioDeAreas(Conexion());
    }

    private IRepositorioDeUsuarios RepositorioDeUsuarios()
    {
        return new RepositorioDeUsuarios(Conexion(), RepositorioDePersonas());
    }

    private IRepositorioDeFuncionalidades RepositorioDeFuncionalidades()
    {
        return General.MAU.RepositorioDeFuncionalidades.NuevoRepositorioDeFuncionalidades(Conexion());
    }

    private IRepositorioDeFuncionalidadesDeUsuarios RepositorioDeFuncionalidadesDeUsuarios()
    {
        return General.MAU.RepositorioDeFuncionalidadesDeUsuarios.NuevoRepositorioDeFuncionalidadesDeUsuarios(Conexion(), RepositorioDeFuncionalidades());
    }


    private Autorizador Autorizador()
    {
        var repo_funcionalidades = RepositorioDeFuncionalidades();
        var repo_funcionalidades_de_usuarios = RepositorioDeFuncionalidadesDeUsuarios();
        var repo_accesos = RepositorioDeAccesosAURL.NuevoRepositorioDeAccesosAURL(Conexion(), repo_funcionalidades);

        return new Autorizador(repo_funcionalidades_de_usuarios,
            RepositorioDeMenues.NuevoRepositorioDeMenues(Conexion(), repo_accesos),
            RepositorioDeUsuarios(),
            RepoPermisosSobreAreas(),
            repo_accesos,
            Conexion());
    }

    private RepositorioDePermisosSobreAreas RepoPermisosSobreAreas()
    {
        return RepositorioDePermisosSobreAreas.NuevoRepositorioDePermisosSobreAreas(Conexion(), RepositorioDeAreas());
    }

    private RepositorioDeDocentes RepositorioDeDocentes()
    {
        return new RepositorioDeDocentes(Conexion(), RepositorioDeCursos());
    }

    private RepositorioDeAsistencias RepoAsistencias()
    {
        return new RepositorioDeAsistencias(Conexion());
    }

    private RepositorioDeEspaciosFisicos RepoEspaciosFisicos()
    {
        return new RepositorioDeEspaciosFisicos(Conexion(), RepositorioDeCursos());
    }

    private RepositorioDeEvaluacion RepoEvaluaciones()
    {
        return new RepositorioDeEvaluacion(Conexion(), RepositorioDeCursos(), RepoAlumnos());
    }

    private RepositorioDeCurriculum RepoCurriculum()
    {
        return new RepositorioDeCurriculum(Conexion());
    }

    private RepositorioDePerfiles RepoPerfiles()
    {
        return new RepositorioDePerfiles(Conexion());
    }

    private RepositorioDePostulaciones RepoPostulaciones()
    {
        return new RepositorioDePostulaciones(Conexion());
    }

    private RepositorioDeComites RepoComites()
    {
        return RepositorioDeComites.Nuevo(Conexion());
    }

}