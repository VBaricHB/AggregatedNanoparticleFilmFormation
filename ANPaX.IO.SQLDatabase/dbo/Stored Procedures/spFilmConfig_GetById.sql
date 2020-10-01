CREATE PROCEDURE [dbo].[spFilmConfig_GetById]
	@Id int
AS
begin

	set nocount on;
	
	select * 
	from dbo.[FilmFormationConfiguration]
	where Id = @id;

end
