USE [DB_RRHH]
GO
/****** Object:  StoredProcedure [dbo].[LIC_GEN_GetDiasFeraidos]    Script Date: 10/24/2014 19:47:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LIC_GEN_GetDiasFeraidos]      

AS
   
BEGIN   

SELECT	id,
		fecha,
		año,
		periodico
		
  FROM dbo.Tabla_Feriados
  
END



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON

