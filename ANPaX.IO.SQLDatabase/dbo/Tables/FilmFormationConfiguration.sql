CREATE TABLE [dbo].[FilmFormationConfiguration]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(55) NULL,
    [XWidth] DECIMAL(18,2) NOT NULL, 
    [YWidth] DECIMAL(18,2) NOT NULL, 
    [ZWidth] DECIMAL(18,2) NOT NULL, 
    [MaxCPU] INT NOT NULL, 
    [LargeNumber] DECIMAL(18,1) NOT NULL, 
    [Delta] DECIMAL(5,3) NOT NULL, 
    [DepositionMethod] NVARCHAR(55) NOT NULL, 
    [WallCollisionMethod] NVARCHAR(55) NOT NULL, 
    [NeighborslistMethod] NVARCHAR(55) NOT NULL
)
