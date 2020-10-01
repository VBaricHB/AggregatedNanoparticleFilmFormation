CREATE PROCEDURE [dbo].[spAggConfig_All]
AS
begin

	set nocount on;
	
	select * 
	from dbo.AggegrateConfiguration;

end
