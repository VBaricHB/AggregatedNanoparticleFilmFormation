CREATE TABLE [dbo].[ParticleSimulation]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(255),
    [User] NVARCHAR(25) NOT NULL,
    [EMail] NVARCHAR(125),
    [PrimaryParticles] INT NOT NULL,
    [XWidth] decimal(18,2) NOT NULL,
    [YWidth] decimal(18,2) NOT NULL,
    [ZWidth] decimal(18,2) NOT NULL,
    [Date] DATETIME2 NOT NULL, 
    [SimulationType] NVARCHAR(55) NOT NULL,
    [SimulationPath] NVARCHAR(255) NOT NULL, 
    [AggregateFormationConfigId] INT NOT NULL, 
    [FilmFormationConfigId] INT NULL, 
    [Status] NVARCHAR(55) NOT NULL, 
    [Percentage] DECIMAL(5,2) NOT NULL
)
