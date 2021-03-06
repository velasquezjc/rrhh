USE [DB_RRHH]
GO
/****** Object:  StoredProcedure [dbo].[CV_Upd_Del_Publicaciones]    Script Date: 07/23/2014 20:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Procedure [dbo].[CV_Upd_Del_Publicaciones]
@CantidadHojas varchar(100) =null,
@DatosEditorial varchar(100)=null,
@DisponeCopia int=null,
@Titulo varchar(100)=null,
@FechaPublicacion datetime=null,
@Usuario int=null,
@Baja int=null,
@IdPublicacion int

AS

begin

declare @NombreSp varchar(60) 
	set @NombreSp = (select OBJECT_NAME(@@PROCID))
	exec dbo.Audit @NombreSp  

UPDATE [dbo].[CV_Publicaciones]
   SET CantidadHojas = isnull(@CantidadHojas, CantidadHojas),
      DatosEditorial = isnull(@DatosEditorial,DatosEditorial),
      DisponeCopia = isnull(@DisponeCopia,DisponeCopia),
      Titulo = isnull(@Titulo, Titulo),
      FechaPublicacion = isnull(@FechaPublicacion, FechaPublicacion),
      Usuario = isnull(@Usuario, Usuario),
      FechaOperacion =GETDATE(),
      Baja = isnull(@Baja, Baja)
     
 WHERE id = @IdPublicacion


END



