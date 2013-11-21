USE [DB_RRHH]
GO
/****** Object:  StoredProcedure [dbo].[SACC_Get_Asistencias]    Script Date: 10/22/2013 20:33:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SACC_Get_Asistencias]

AS

BEGIN

SELECT [id]
      ,[idAlumno]
      ,[idCurso]
      ,[valor]
      ,[fechaAsistencia]
      ,[idUsuario]
      ,[idBaja]
  FROM [dbo].[SAC_Asistencias]
  WHERE idBaja is null

END

