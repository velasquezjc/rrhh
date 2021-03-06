USE [DB_RRHH]
GO
/****** Objeto:  StoredProcedure [dbo].[LIC_GEN_GetAusenciasPendientesEntreFechas]    Fecha de la secuencia de comandos: 04/28/2014 21:23:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LIC_GEN_GetAusenciasPendientesEntreFechas]  
@fecha_desde datetime, 
@fecha_hasta datetime
 
AS

BEGIN   

SELECT	dp.NroDocumento, 
		lic_pen.Id_Interna, 
		lic_pen.Concepto, 
		con_lic.Descripcion,  
		lic_pen.Desde, 
		lic_pen.Hasta
  FROM dbo.ASIS_Solicitudes_Licencia_Web lic_pen
  LEFT JOIN dbo.LegajosPersonas lp
  ON lp.legajo = lic_pen.Id_Interna
  LEFT JOIN dbo.DatosPersonales dp
  ON dp.Id = lp.idPersona 
  LEFT JOIN dbo.Tabla_Conceptos_Licencias con_lic	
  ON con_lic.Id_Concepto = lic_pen.Concepto

  WHERE (@fecha_desde IS NULL OR lic_pen.Desde > @fecha_desde OR (lic_pen.Desde < @fecha_desde AND lic_pen.Hasta > @fecha_desde))
  
END






set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON





