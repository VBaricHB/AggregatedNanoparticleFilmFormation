CREATE PROCEDURE [dbo].[spParticleSimulations_Insert]
	@Description nvarchar(255),
	@User nvarchar(25),
	@EMail nvarchar(125),
	@PrimaryParticles int,
	@XWidth decimal(18,2), 
	@YWidth decimal(18,2), 
	@ZWidth decimal(18,2), 
	@Date datetime2(7),
	@SimulationType nvarchar(55),
	@SimulationPath nvarchar(255),
	@AggregateFormationConfigId int,
	@FilmFormationConfigId int,
	@Status nvarchar(55),
	@Percentage decimal(5,2),
	@Id int output
AS
begin

	set nocount on;

	insert into dbo.[ParticleSimulation]([Description], [User], EMail, PrimaryParticles, XWidth, YWidth, ZWidth, [Date], SimulationType, SimulationPath, AggregateFormationConfigId, FilmFormationConfigId, [Status], [Percentage])
	values (@Description, @User, @EMail, @PrimaryParticles, @XWidth, @YWidth, @ZWidth, @Date, @SimulationType, @SimulationPath, @AggregateFormationConfigId, @FilmFormationConfigId, @Status, @Percentage)

	set @Id = SCOPE_IDENTITY();

end
