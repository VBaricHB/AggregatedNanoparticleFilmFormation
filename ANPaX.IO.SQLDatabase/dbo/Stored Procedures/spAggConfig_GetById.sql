CREATE PROCEDURE [dbo].[spAggConfig_GetById]
	@Id int
AS
begin

	set nocount on;
	
	select * 
	from dbo.[AggegrateConfiguration]
	where Id = @id;

end
