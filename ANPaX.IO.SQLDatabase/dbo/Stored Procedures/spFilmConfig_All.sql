CREATE PROCEDURE [dbo].[spFilmConfig_All]
AS
begin

	set nocount on;
	
	select * 
	from dbo.FilmFormationConfiguration;

end
