CREATE TABLE [dbo].[AggegrateConfiguration]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(55) NULL,
    [TotalPrimaryParticles] INT NOT NULL, 
    [ClusterSize] INT NOT NULL, 
    [Df] DECIMAL(5, 3) NOT NULL, 
    [Kf] DECIMAL(5,3) NOT NULL, 
    [Epsilon] DECIMAL(5,3) NOT NULL, 
    [Delta] DECIMAL(5,3) NOT NULL, 
    [MaxAttemptsPerCluster] INT NOT NULL, 
    [MaxAttemptsPerAggregate] INT NOT NULL, 
    [LargeNumber] DECIMAL(18,1) NOT NULL, 
    [RadiusMeanCalculationMethod] NVARCHAR(25) NOT NULL, 
    [AggregateSizeMeanCalculationMethod] NVARCHAR(25) NOT NULL, 
    [PrimaryParticleSizeDistribution] NVARCHAR(50) NOT NULL, 
    [AggregateSizeDistribution] NVARCHAR(50) NOT NULL, 
    [AggregateFormationFactory] NVARCHAR(50) NOT NULL, 
    [RandomGeneratorSeed] DECIMAL(18,1) NOT NULL
)
