USE [DB_RRHH]
GO
/****** Object:  StoredProcedure [dbo].[MODI_Des_Asignar_Imagen]    Script Date: 05/21/2014 19:35:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[MODI_Des_Asignar_Imagen]
	@id_imagen int
AS

declare @NombreSp varchar(60) 
set @NombreSp = (select OBJECT_NAME(@@PROCID))
exec dbo.Audit @NombreSp

BEGIN
	UPDATE dbo.MODI_Imagenes 
	SET nro_folio=NULL
	WHERE id_imagen=@id_imagen
END

