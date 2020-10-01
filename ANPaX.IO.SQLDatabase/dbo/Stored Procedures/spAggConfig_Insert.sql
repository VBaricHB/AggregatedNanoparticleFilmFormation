CREATE PROCEDURE [dbo].[spAggConfig_Insert]
	@Description nvarchar(55),
	@TotalPrimaryParticles int,
	@ClusterSize int,
	@Df decimal(5,3),
	@Kf decimal (5,3),
	@Epsilon decimal (5,3),
	@Delta decimal (5,3),
	@MaxAttemptsPerCluster int,
	@MaxAttemptsPerAggregate int,
	@LargeNumber decimal(18,1),
	@RadiusMeanCalculationMethod nvarchar(25),
	@AggregateSizeMeanCalculationMethod nvarchar(25),
	@PrimaryParticleSizeDistribution nvarchar(50),
	@AggregateSizeDistribution nvarchar(50),
	@AggregateFormationFactory nvarchar(50),
	@RandomGeneratorSeed decimal(18,1),
	@Id int output
AS
begin 
	set nocount on;

	insert into dbo.[AggegrateConfiguration](
		[Description],
		TotalPrimaryParticles, 
		ClusterSize, 
		Df, 
		Kf, 
		Epsilon, 
		Delta, 
		MaxAttemptsPerCluster, 
		MaxAttemptsPerAggregate, 
		LargeNumber, 
		RadiusMeanCalculationMethod, 
		AggregateSizeMeanCalculationMethod, 
		PrimaryParticleSizeDistribution, 
		AggregateSizeDistribution, 
		AggregateFormationFactory, 
		RandomGeneratorSeed)
	values (
		@Description,
		@TotalPrimaryParticles, 
		@ClusterSize, 
		@Df, 
		@Kf, 
		@Epsilon, 
		@Delta, 
		@MaxAttemptsPerCluster, 
		@MaxAttemptsPerAggregate, 
		@LargeNumber, 
		@RadiusMeanCalculationMethod, 
		@AggregateSizeMeanCalculationMethod, 
		@PrimaryParticleSizeDistribution, 
		@AggregateSizeDistribution, 
		@AggregateFormationFactory, 
		@RandomGeneratorSeed)

	set @Id = SCOPE_IDENTITY();
end
