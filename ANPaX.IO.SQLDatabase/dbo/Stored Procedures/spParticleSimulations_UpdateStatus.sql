CREATE PROCEDURE [dbo].[spParticleSimulations_UpdateStatus]
	@Id int,
	@Status nvarchar(55)
AS
begin

	set nocount on;
	update dbo.[ParticleSimulation]
	set [Status] = @Status
	where Id = @Id;


end
