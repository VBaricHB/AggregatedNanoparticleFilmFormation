CREATE PROCEDURE [dbo].[spParticleSimulations_GetById]
	@Id int
AS
begin

	set nocount on;
	
	select * 
	from dbo.[ParticleSimulation]
	where Id = @id;

end
