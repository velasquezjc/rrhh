USE [DB_RRHH]
GO
/****** Object:  StoredProcedure [dbo].[LIC_GEN_GetDiasPermitidos]    Script Date: 02/04/2014 21:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LIC_GEN_GetDiasPermitidos]      
 @nro_documento int = null,
 @periodo smallint = null,
 @id_concepto_licencia smallint = null 
 
AS
   
BEGIN   

SELECT	dp.NroDocumento, dp.Apellido, dp.Nombre, 
		auth_lic.Id_Interna, auth_lic.Dias_Autorizados, auth_lic.Id_Concepto_Licencia, auth_lic.Periodo, auth_lic.Dias_Tomados 
  FROM dbo.Autorizaciones_Licencias auth_lic
  LEFT JOIN dbo.LegajosPersonas lp
  ON lp.legajo = auth_lic.Id_Interna
  LEFT JOIN dbo.DatosPersonales dp
  ON dp.Id = lp.idPersona 

  WHERE (@nro_documento IS NULL OR dp.NroDocumento = @nro_documento) AND
		(@periodo IS NULL OR auth_lic.Periodo = @periodo) AND
		(@id_concepto_licencia IS NULL OR auth_lic.Id_Concepto_Licencia = @id_concepto_licencia)
  
END