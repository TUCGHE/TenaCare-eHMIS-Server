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

--Table dbo.DiseaseDictionary
/****** Object:  Table [dbo].[DiseaseDictionary]    Script Date: 10-Oct-16 10:34:03 AM ******/
GO
/****** Object:  Table [dbo].[DiseaseDictionary]  Script Date: 04/22/2014 11:27:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiseaseDictionaryarabic]') AND type in (N'U'))
DROP TABLE [dbo].[DiseaseDictionaryarabic]
GO


--Create table and its columns
CREATE TABLE [dbo].[DiseaseDictionaryarabic] (
	[id] [int] NULL,
	[sno] [varchar](50) NULL,
	[labelId] [int] NULL,
	[dataEleClass] [int] NULL,
	[descrip] [varchar](5000) NULL,
	[classAndLabel] [int] NULL,
	[gender] [nvarchar](100) NULL,
	[age] [nvarchar](1000) NULL,
	[disease] [nvarchar](max) NULL);
GO

insert into DiseaseDictionaryarabic
select * from v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Agearabic where dataEleClass in (8,2,3)

