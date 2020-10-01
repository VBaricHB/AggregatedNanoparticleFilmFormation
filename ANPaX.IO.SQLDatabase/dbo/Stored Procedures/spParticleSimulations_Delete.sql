CREATE PROCEDURE [dbo].[spParticleSimulations_Delete]
	@Id int
AS
begin
	
	set nocount on;

	delete 
	from dbo.[ParticleSimulation]
	where Id = @Id;

end
