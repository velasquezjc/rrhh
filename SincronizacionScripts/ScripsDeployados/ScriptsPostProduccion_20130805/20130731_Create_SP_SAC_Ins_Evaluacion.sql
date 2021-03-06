USE [DB_RRHH]
GO
/****** Object:  StoredProcedure [dbo].[SACC_Ins_Evaluaciones]    Script Date: 07/31/2013 21:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SACC_Ins_Evaluacion]
					@id_alumno int,
					@id_curso smallint,
					@id_instancia_evaluacion smallint,
					@calificacion varchar(30),
					@fecha_evaluacion datetime,
					@id_usuario int,
					@fecha datetime = null,
					@baja int = null
AS

BEGIN	
	INSERT INTO [dbo].[SAC_Evaluaciones]
			   ([idAlumno]
			   ,[idCurso]
			   ,[idInstanciaEvaluacion]
			   ,[Calificacion]
			   ,[fechaEvaluacion]
			   ,[idUsuario]
			   ,[fecha]
			   ,[idBaja]
			   )
		 VALUES
			   (@id_alumno,
				@id_curso,
				@id_instancia_evaluacion,
				@calificacion,
				@fecha_evaluacion,
				@id_usuario,
				@fecha,
				@baja)
	select @@ROWCOUNT
END		



