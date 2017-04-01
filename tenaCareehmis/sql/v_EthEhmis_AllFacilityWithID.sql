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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_EthEhmis_AllFacilityWithIDNew]') and OBJECTPROPERTY(id, N'Isview') = 1)
DROP VIEW [dbo].[v_EthEhmis_AllFacilityWithIDNew]

/****** Object:  View [dbo].[v_EthEhmis_AllFacilityWithIDNew]    Script Date: 10/12/2016 10:25:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_EthEhmis_AllFacilityWithIDNew]
AS
-- Under Woreda
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,           --7175
	facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, district.districtseq as WoredaId, 
	district.Name as WoredaName, 
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then 0 else EthEhmis_HmisZoneDistrict.ZoneId end as ZoneId, 
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then 'NoZone_' + 'District_' + district.Name else EthEhmis_HmisZone.ZONENAME end as ZONENAME,
	provincecode as RegionId, Province.Name as ReportingRegionName , district.Name as ReportingAdminSite, 
	facility.DistrictId as ReportingDistrictId, 8 as reportingFacilityTypeId
	from facility 
	inner join district on
	district.DistrictSeq = facility.DistrictId
	left outer join EthEhmis_HmisZoneDistrict on
	EthEhmis_HmisZoneDistrict.DistrictSeq = District.DistrictSeq
	left outer join EthEhmis_HMISZone on
	EthEhmis_HMISZone.ZoneId =  EthEhmis_HmisZoneDistrict.ZoneId
	inner join province on
	district.ProvinceCode = province.Code
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (AggregationLevelId = 4 and notWoreda = 0)
union
-- Woreda health office under zone or Region
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,           --7175
	facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, district.districtseq as WoredaId, 
	district.Name as WoredaName, 
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then 0 else EthEhmis_HmisZoneDistrict.ZoneId end as ZoneId, 
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then 'NoZone_' + 'District_' + district.Name else EthEhmis_HmisZone.ZONENAME end as ZONENAME,
	provincecode as RegionId, Province.Name as ReportingRegionName , 
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then province.name else EthEhmis_HmisZone.ZONENAME end as ReportingAdminSite,
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then province.code else EthEhmis_HmisZone.ZONEID end as ReportingDistrictId,
	case when EthEhmis_HmisZoneDistrict.ZoneId is null 
	then 10 else 9 end as reportingFacilityTypeId	
	from facility 
	inner join district on
	district.DistrictSeq = facility.DistrictId
	left outer join EthEhmis_HmisZoneDistrict on
	EthEhmis_HmisZoneDistrict.DistrictSeq = District.DistrictSeq
	left outer join EthEhmis_HMISZone on
	EthEhmis_HMISZone.ZoneId =  EthEhmis_HmisZoneDistrict.ZoneId
	inner join province on
	district.ProvinceCode = province.Code
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (facility.FacilityTypeId = 8)
union
-- Under a Zone
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,
	facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, 
	0 as WoredaId, 'NoDistrict' + '_Zone_' + EthEhmis_HMISZone.ZONENAME as WoredaName, 
	ZoneId, EthEhmis_HMISZone.ZONENAME, REGIONID, Province.Name as ReportingRegionName , 
	EthEhmis_HMISZone.ZONENAME as ReportingAdminSite, 
	facility.DistrictId as ReportingDistrictId, 9 as reportingFacilityTypeId
	from facility
	inner join EthEhmis_HMISZone on
	EthEhmis_HMISZone.ZoneId = facility.DistrictId
	inner join province on
	EthEhmis_HmisZone.REGIONID = province.code
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (AggregationLevelId = 4) 
union
-- Zonal Health Department
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,
	facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, 
	0 as WoredaId, 'NoDistrict' + '_Zone_' + EthEhmis_HMISZone.ZONENAME as WoredaName, 
	ZoneId, EthEhmis_HMISZone.ZONENAME, REGIONID, Province.Name as ReportingRegionName , 
	Province.Name as ReportingAdminSite, 
	province.Code as ReportingDistrictId, 10 as reportingFacilityTypeId
	from facility
	inner join EthEhmis_HMISZone on
	EthEhmis_HMISZone.ZoneId = facility.DistrictId
	inner join province on
	EthEhmis_HmisZone.REGIONID = province.code
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (facilityType.FacilityTypeId = 9) 
union
-- Under a Region
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,
	facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, 
	0 as WoredaId, 'NoDistrict' + '_Province_' + Province.Name as WoredaName, 
	0 as ZoneId, 
	'NoZone' + '_Province_' + Province.Name as ZONENAME,
	province.code as REGIONID, Province.Name as ReportingRegionName , Province.Name as ReportingAdminSite, 
	facility.DistrictId as ReportingDistrictId, 10 as reportingFacilityTypeId
	from facility
	inner join province on
	facility.districtId = province.code
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (AggregationLevelId = 4)
union
-- RHBs
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,
	facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, 
	0 as WoredaId, 'NoDistrict' + '_Province_' + Province.Name as WoredaName, 
	0 as ZoneId, 
	'NoZone' + '_Province_' + Province.Name as ZONENAME,
	province.code as REGIONID, Province.Name as ReportingRegionName , 'National' as ReportingAdminSite, 
	88 as ReportingDistrictId, 11 as reportingFacilityTypeId
	from facility
	inner join province on
	facility.districtId = province.code
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (facility.FacilityTypeId = 10)
union
-- Under Federal
	select facility.DistrictId, Facility.FacilityTypeId, FacilityType.FacilityTypeName, Facility.HMISCode,
    facility.FacilityName, 0 as HealthCenterID, 'NoHC' as HealthCenterName, 
	0 as WoredaId, 'NoDistrict' + '_National_' as WoredaName, 
	0 as ZoneId, 
	'NoZone' + '_National_' as ZONENAME,
	DistrictId as REGIONID, facility.FacilityName as ReportingRegionName , 'National' as ReportingAdminSite, 
	facility.DistrictId as ReportingDistrictId, 11 as reportingFacilityTypeId
	from facility	
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where (facility.FacilityTypeId = 11)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_EthEhmis_AllFacilityHC]') and OBJECTPROPERTY(id, N'Isview') = 1)
DROP VIEW [dbo].[v_EthEhmis_AllFacilityHC]

GO
/****** Object:  View [dbo].[v_EthEhmis_AllFacilityHC]    Script Date: 10/12/2016 10:27:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_EthEhmis_AllFacilityHC]
AS
SELECT *
FROM     dbo.v_EthEhmis_AllFacilityWithIDNew
WHERE  (FacilityTypeId = 2)

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_EthEhmis_AllFacilityHP]') and OBJECTPROPERTY(id, N'Isview') = 1)
DROP VIEW [dbo].[v_EthEhmis_AllFacilityHP]

/****** Object:  View [dbo].[v_EthEhmis_AllFacilityHP]    Script Date: 10/12/2016 10:27:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_EthEhmis_AllFacilityHP]
AS
	select facility.DistrictId, facility.FacilityTypeId,  facilityType.FacilityTypeName, facility.HMISCode,
	facility.FacilityName, facility.DistrictId as HealthCenterID, v_EthEhmis_AllFacilityHC.FacilityName as HealthCenterName,
	v_EthEhmis_AllFacilityHC.WoredaId,
	v_EthEhmis_AllFacilityHC.WoredaName, v_EthEhmis_AllFacilityHC.ZoneId,
	v_EthEhmis_AllFacilityHC.ZONENAME, v_EthEhmis_AllFacilityHC.RegionId,
	v_EthEhmis_AllFacilityHC.ReportingRegionName as ReportingRegionName , 
	v_EthEhmis_AllFacilityHC.FacilityName as ReportingAdminSite, facility.DistrictId as ReportingDistrictId,
	2 as reportingFacilityTypeId
	from facility 
	inner join v_EthEhmis_AllFacilityHC
	on
	facility.DistrictId = v_EthEhmis_AllFacilityHC.hmisCode
	inner join FacilityType on
	FacilityType.FacilityTypeId = facility.FacilityTypeId
	where facility.FacilityTypeId = 3
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_EthEhmis_AllFacilityWithID]') and OBJECTPROPERTY(id, N'Isview') = 1)
DROP VIEW [dbo].[v_EthEhmis_AllFacilityWithID]

/****** Object:  View [dbo].[v_EthEhmis_AllFacilityWithID]    Script Date: 10/12/2016 10:24:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_EthEhmis_AllFacilityWithID]
AS
select * from v_EthEhmis_AllFacilityHP
union
select * from v_EthEhmis_AllFacilityWithIDNew

GO