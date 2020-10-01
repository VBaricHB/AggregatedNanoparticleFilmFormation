CREATE PROCEDURE [dbo].[spFilmConfig_Insert]
	@Description nvarchar(55),
	@XWidth decimal(18,2),
	@YWidth decimal(18,2),
	@ZWidth decimal(18,2),
	@MaxCPU int,
	@LargeNumber decimal(18,1),
	@Delta decimal(5,3),
	@DepositionMethod nvarchar(55),
	@WallCollisionMethod nvarchar(55),
	@NeighborslistMethod nvarchar(55),
	@Id int output
AS
begin

	set nocount on;

	insert into dbo.[FilmFormationConfiguration]([Description], XWidth, YWidth, ZWidth, MaxCPU, LargeNumber, Delta, DepositionMethod, WallCollisionMethod, NeighborslistMethod) 
	values (@Description, @XWidth, @YWidth, @ZWidth, @MaxCPU, @LargeNumber, @Delta, @DepositionMethod, @WallCollisionMethod, @NeighborslistMethod)

	set @Id = SCOPE_IDENTITY();

end
