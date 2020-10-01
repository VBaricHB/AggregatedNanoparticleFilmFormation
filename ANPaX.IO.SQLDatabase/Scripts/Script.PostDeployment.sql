/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

if not exists (select * from dbo.AggegrateConfiguration)
begin
    insert into dbo.AggegrateConfiguration([Description], TotalPrimaryParticles, ClusterSize, Df, Kf, Epsilon, Delta, MaxAttemptsPerCluster, MaxAttemptsPerAggregate, LargeNumber, RadiusMeanCalculationMethod, AggregateSizeMeanCalculationMethod, PrimaryParticleSizeDistribution, AggregateSizeDistribution, AggregateFormationFactory, RandomGeneratorSeed)
    values('Default', 600000, 6, 1.8, 1.30, 1.001, 1.01, 50000, 200000, 10000000,'Geometric', 'Geometric', 'DissDefault', 'DissDefault', 'CCA', -1)
end

if not exists (select * from dbo.FilmFormationConfiguration)
begin
    insert into dbo.FilmFormationConfiguration([Description], XWidth, YWidth, ZWidth, MaxCPU, LargeNumber, Delta, DepositionMethod, WallCollisionMethod, NeighborslistMethod)
    values('Default', 2000.0,2000.0,2000.0,4,10000000,1.01,'Ballistic','Periodic','Accord')

end