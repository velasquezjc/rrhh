set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SACC_Get_Espacios_Fisicos]

AS

BEGIN

select
ef.id							id,	
ef.Aula							Aula,
ed.id							idEdificio,
ed.Nombre						NombreEdificio,
ed.Calle						DireccionEdificio,
ed.Numero						NumeroEdificio,
ef.Capacidad					Capacidad


From dbo.SAC_Espacios_Fisicos ef
left join dbo.SAC_Edificios ed on
ef.idEdificio = ed.id

where ef.idBaja is null

END

