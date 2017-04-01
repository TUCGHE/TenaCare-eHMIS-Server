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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[vw_FacilityGIS]') and OBJECTPROPERTY(id, N'Isview') = 1)
DROP VIEW [dbo].[vw_FacilityGIS]
GO


GO

/****** Object:  View [dbo].[vw_FacilityGIS]    Script Date: 9/14/2016 3:40:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_FacilityGIS]
AS
SELECT        dbo.Facility.FacilityName, dbo.FacilityType.FacilityTypeName, dbo.GIS_Table.POINT_X, dbo.GIS_Table.POINT_Y, dbo.Facility.HMISCode
FROM            dbo.Facility INNER JOIN
                         dbo.FacilityType ON dbo.Facility.FacilityTypeId = dbo.FacilityType.FacilityTypeId INNER JOIN
                         dbo.GIS_Table ON dbo.Facility.HMISCode = dbo.GIS_Table.HMISCode
WHERE        (dbo.GIS_Table.POINT_X <> 0) AND (dbo.GIS_Table.POINT_Y <> 0)

GO



