CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT

AS
BEGIN
	SET XACT_ABORT ON;

	BEGIN TRAN
		
	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name = @Studies);
	DECLARE @newSemesterestr INT = @Semester + 1;
	DECLARE @IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudies AND Semester = @Semester);
	DECLARE @NewEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester=@newSemesterestr AND IdStudy=@IdStudies);
	DECLARE @MakeNewEnrollment INT = (SELECT MAX(IdEnrollment) FROM Enrollment) + 1;

	IF @IdStudies IS NULL
		BEGIN
			RAISERROR ('Studies ID is missing', 10, 1)
			RETURN
		END

	IF @IdEnrollment IS NULL
		BEGIN
			RAISERROR ('Enrollment is missing', 10, 1)
			RETURN
		END

	IF @Semester IS NULL
		BEGIN
			RAISERROR ('Semester is missing', 10, 1)
			RETURN
		END	


	IF @NewEnrollment IS NULL
		BEGIN
			INSERT INTO Enrollment VALUES (@MakeNewEnrollment, @newSemesterestr, @IdStudies, GETDATE())
			
			UPDATE Student SET IdEnrollment = @MakeNewEnrollment;
			
			COMMIT
			
			RETURN @MakeNewEnrollment
		END

	UPDATE Student SET IdEnrollment = @NewEnrollment;
		
	COMMIT
	RETURN @NewEnrollment;

END