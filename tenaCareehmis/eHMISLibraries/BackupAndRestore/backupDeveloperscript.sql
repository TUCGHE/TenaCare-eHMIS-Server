--alter database eHMIS set single_user with rollback immediate
DECLARE  @location As nvarchar(max)
DECLARE  @Day As nvarchar(max)
DECLARE  @Month As nvarchar(max)
DECLARE  @Year As nvarchar(max)
Declare  @Hour as nvarchar(max)
Declare  @Min as nvarchar(max)
DECLARE  @bak As nvarchar(max)
DECLARE  @Path As varchar(max)
set @location =('D:\NewEhmisPhem\NewEhmisPhemModules\ScheduledBackUpAndRestore\HMISBackUpDB\eHMIS_')
set @Day =day (convert(varchar,getdate(),100))
set @Month = DateName( Month, getDate() )
set @Year =year (convert(varchar,getdate(),100))
set @Hour=(DATEPART(hour,getdate()))
set @Min=(DATEPART(Minute,getdate()))
set @bak=('.bak')
set @Path=(@location +  @Month+'_'+ @Day +'_' + @Year +'_' + @Hour +'_' + @Min +@bak)
backup database ehmis to disk=@Path
--ALTER DATABASE eHMIS SET MULTI_USER