CREATE PROCEDURE [dbo].[SACC_Get_MaxHorasCatedraCurso]

AS

BEGIN

SELECT [valor]
  FROM [dbo].[SAC_MaxHorasCatedra]
END

GO