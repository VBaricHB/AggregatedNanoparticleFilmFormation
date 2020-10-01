CREATE PROCEDURE [dbo].[spParticleSimulations_UpdatePercentage]
	@Id int,
	@Percentage decimal(3,0)
AS
begin

	set nocount on;
	update dbo.[ParticleSimulation]
	set [Percentage] = @Percentage
	where Id = @Id;


end
