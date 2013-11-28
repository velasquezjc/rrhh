set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SACC_Get_Alumnos]

AS

BEGIN

select
pe.Id							Id,	
pe.Documento					Documento,
pe.Apellido						Apellido,
pe.Nombres						Nombre,
isnull(pr.Telefono,'')			Telefono,
isnull(pr.Email_personal,'')	Mail,
isnull(cd.Calle,'')	+  ' ' + isnull(cd.Numero,'') +  ' ' + isnull( 'Piso ' + cd.Piso,'') +  ' ' + isnull( 'Dto ' + cd.Depto,'')	 		Direccion,
--ta.descripcion					Area,
al.IdModalidad					IdModalidad,
mo.Descripcion					ModalidadDescripcion,
da.Id_area						IdArea,
ta.descripcion					NombreArea,
lt.Denominacion					LugarTrabajo,
pap.IdOrganismo					IdOrganismo,
org.Descripcion					DescripcionOrganismo,
dp.Fecha_Nacimiento				FechaNacimiento,
al.idBaja						IdBaja




From dbo.SAC_Alumnos al
inner join dbo.CRED_Personas pe on 
al.IdPersona = pe.Id
inner join dbo.CRED_PersonasRelevadas pr on
pe.Documento = pr.Documento
inner join  dbo.CRED_Domicilios cd on 
pe.Documento = cd.Documento
left join dbo.CRED_LugaresDeTrabajo lt on
lt.OficinaN = pr.lugar_de_trabajo
left join dbo.Datos_Personales dp on
dp.Nro_Documento = pe.Documento
left join dbo.CRED_PedidoAltaPersonas pap on
pap.IdPersona = pe.Id
left join dbo.CRED_Organismos  org on
org.Id = pap.IdOrganismo
left join dbo.SAC_Modalidad as mo
on al.IdModalidad = mo.IdModalidad
left join dbo.LEG_Desglose_Area as da
on pe.Documento = da.Documento
left join dbo.Tabla_Areas ta on
da.Id_Area = ta.id

WHERE al.idBaja is null

END


