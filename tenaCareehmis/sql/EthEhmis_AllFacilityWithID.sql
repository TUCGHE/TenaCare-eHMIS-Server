--/*
-- * 
-- * Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
-- * dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
-- * All rights reserved.

-- * This file is part of TenaCareeHMIS
-- * TenaCareeHMIS is free software: 
-- * 
-- * you can redistribute it and/or modify it under the terms of the 
-- * GNU General Public License as published by the Free Software Foundation, 
-- * version 3 of the License, or any later version.
-- * TenaCareeHMIS is distributed in the hope that it will be useful,
-- * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
-- * FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

-- * You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
-- * If not, see http://www.gnu.org/licenses/.    
-- * 
-- * 
-- */

/****** Object:  Table [dbo].[EthEhmis_AllFacilityWithID]    Script Date: 10-Oct-16 10:34:03 AM ******/
GO
/****** Object:  Table [dbo].[EthEhmis_AllFacilityWithID]   Script Date: 04/22/2014 11:27:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EthEhmis_AllFacilityWithID]') AND type in (N'U'))
DROP TABLE [dbo].[EthEhmis_AllFacilityWithID]
GO

/****** Object:  Table [dbo].[EthEhmis_AllFacilityWithID]    Script Date: 10/18/2016 3:48:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EthEhmis_AllFacilityWithID](
	[DistrictId] [bigint] NOT NULL,
	[FacilityTypeId] [int] NOT NULL,
	[FacilityTypeName] [nvarchar](100) NOT NULL,
	[HMISCode] [nvarchar](50) NULL,
	[FacilityName] [nvarchar](1000) NULL,
	[HealthCenterID] [int] NULL,
	[HealthCenterName] [nvarchar](100) NULL,
	[WoredaId] [bigint] NULL,
	[WoredaName] [nvarchar](1000) NULL,
	[ZoneId] [bigint] NULL,
	[ZoneName] [nvarchar](1000) NULL,
	[RegionId] [int] NULL,
	[ReportingRegionName] [nvarchar](1000) NULL,
	[ReportingAdminSite] [nvarchar](1000) NULL,
	[ReportingDistrictId] [bigint] NULL,
	[reportingFacilityTypeId] [int] NULL
) ON [PRIMARY]

GO

insert into [EthEhmis_AllFacilityWithID]
select * from v_EthEhmis_AllFacilityWithID

GO


