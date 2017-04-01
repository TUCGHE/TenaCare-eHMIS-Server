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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_EthEhmis_HmisValue]') and OBJECTPROPERTY(id, N'Isview') = 1)
DROP VIEW [dbo].[v_EthEhmis_HmisValue]
GO

/****** Object:  View [dbo].[v_EthEhmis_HmisValue]    Script Date: 10/18/2016 4:17:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[v_EthEhmis_HmisValue]
WITH SCHEMABINDING 
AS
SELECT ValueID, LabelID, CAST(DataEleClass AS int) AS DataClass, 
DataEleClass * 100000 + LabelID AS ClassAndLabel, LocationID, [Year], [Month], [Week], CAST(Value AS bigint) AS Value, RegionID, ZoneID, WoredaID, 
                  FACILITTYPE AS FacilityTypeID,
				  FiscalYear = case when [Month] = 11 or [Month] = 12 then [Year]+1
				  else [Year] end,
				  FiscalMonth = case when [Month] = 11 then 1 when [Month] = 12 then 2 when [Month] = 0 then 0
				  else [Month]+2 end,
				  [Quarter] = case when [Month] = 11 or [Month] = 12 or [Month] = 1 then 1
				  when [Month] between 2 and 4 then 2
				  when [Month] between 5 and 7 then 3
				  when [Month] between 8 and 10 then 4
				  when [Month] = 0 then 0
				  end
				  
FROM     dbo.EthEhmis_HMISValue
WHERE  (Value <> 0)







GO


