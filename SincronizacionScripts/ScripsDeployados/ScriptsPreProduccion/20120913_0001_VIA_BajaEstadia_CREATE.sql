
CREATE PROCEDURE [dbo].[VIA_BajaEstadia]
@IdEstadia int
AS

BEGIN

UPDATE dbo.VIA_Estadias 
SET Baja = 1
WHERE Id = @IdEstadia

END