CREATE PROCEDURE [dbo].[spParticleSimulations_All]
AS
begin

	set nocount on;
	
	select * 
	from dbo.ParticleSimulation;

end
