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

/****** Object:  Table [dbo].[EthEhmis_IndicatorsNewDisplay]    Script Date: 10-Oct-16 10:34:03 AM ******/
GO
/****** Object:  Table [dbo].[EthEhmis_IndicatorsNewDisplay]   Script Date: 04/22/2014 11:27:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EthEhmis_IndicatorsNewDisplay]') AND type in (N'U'))
DROP TABLE [dbo].[EthEhmis_IndicatorsNewDisplay]
GO

--Create table and its columns
CREATE TABLE [dbo].[EthEhmis_IndicatorsNewDisplay] (
	[SNO] [varchar](50) NULL,
	[IndicatorName] [varchar](MAX) NULL,
	[Category1] [varchar](MAX) NULL,
	[NumeratorName] [varchar](MAX) NULL,
	[NumeratorLabelId] [varchar](MAX) NULL,
	[Actions] [varchar](MAX) NULL,
	[NumeratorDataEleClass] [varchar](50) NULL,
	[DenominatorName] [varchar](MAX) NULL,
	[DenominatorLabelId] [varchar](MAX) NULL,
	[DenominatorDataEleClass] [varchar](50) NULL,
	[ReadOnly] [bit] NULL,
	[SequenceNo] [int] NULL,
	[ReportType] [varchar](100) NULL,
	[HP] [bit] NULL,
	[HC] [bit] NULL,
	[Hospital] [bit] NULL,
	[WorHo] [bit] NULL,
	[annual] [bit] NULL,
	[commonAnnual] [bit] NULL,
	[PeriodType] [int] NULL,
	[commonQuarterly] [bit] NULL,
	[targetDivide] [bit] NULL);
GO

INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1', N'Accesstohealthservice:Totalindicators:97', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 1, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('True' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.1', N'MaternalandChildhealth:Totalindicators:35', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 2, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('True' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.1.1', N'MaternalHealth;Totalindicators:13', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 3, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('True' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'ContraceptiveAcceptanceRate', N'Family Planning', N'Family Planning New and Repeat Acceptors age between 15 - 49', N'3011, 3012, 3013, 3015, 3016, 3017', N'sum', N'6', N'Total number of women of reproductive age (15-49) who are not pregnant', N'8', N'4', CAST ('False' AS bit), 4, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'AntenatalCareCoverageFirstVisit', N'Antenatal Care (ANC)', N'Number of pregnant women that received antenatal care at least once', N'5', N'sum', N'6', N'Total number of expected pregnancies', N'3', N'4', CAST ('False' AS bit), 5, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'AntenatalCareCoverageFourVisit', N'Antenatal Care (ANC)', N'Number of pregnant women that received antenatal care: at least four visits', N'3028', N'sum', N'6', N'Total number of expected pregnancies', N'3', N'4', CAST ('False' AS bit), 6, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'PercentageofpregnantwomenattendingantenatalcareclinicstestedforSyphilis', N'Antenatal Care (ANC)', N'Number of pregnant women tested for syphilis', N'3029', N'sum', N'6', N'Total number of -pregnant mothers attended at least one ANC visit', N'5', N'6', CAST ('False' AS bit), 7, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'ProportionofbirthsAttendedbySkilledHealthPersonnel', N'Delivery', N'Births Attended by skilled attendant', N'7', N'sum', N'6', N'Total number of expected deliveries', N'4', N'4', CAST ('False' AS bit), 8, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'ProportionofbirthsAttendedbyhealthextensionworkersathealthposts', N'Delivery', N'Number of births attended by health extension workers at Health Post', N'3030', N'sum', N'6', N'Total number of expected deliveries', N'4', N'4', CAST ('False' AS bit), 9, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'EarlyPostnatalCareCoverage', N'Postnatal', N'Early-first postnatal care attendances 0-48 hrs ( 0-2 days), -49-72 hrs (2-3 days), 73 hrs - 6 days (4-6 days)', N'3031, 3032, 3033', N'sum', N'6', N'Total number of expected deliveries', N'4', N'4', CAST ('False' AS bit), 10, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'8', N'CaesareanSectionRate', N'Delivery', N'Number of women that gave birth by caesarean section', N'14', N'sum', N'6', N'Total number of expected deliveries', N'4', N'4', CAST ('False' AS bit), 11, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'9', N'Numberofwomenreceivingcomprehensiveabortioncareservices', N'Abortion Care', N'Number of safe abortions performed, and number of post abortion/emergency care', N'6', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 12, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'10', N'InstitutionalMaternalDeathRate', N'Maternal Death', N'Number of maternal deaths in health facility', N'15', N'sum', N'6', N'Births Attended by skilled attendant', N'7', N'6', CAST ('False' AS bit), 13, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'11', N'Numberofmaternaldeathinthecommunity', N'Maternal Death', N'Total number of maternal deaths in the community', N'4475', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 14, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'12', N'StillbirthRate', N'Delivery', N'Number of still births', N'9', N'sum', N'6', N'Total number of deliveries in health facility (Live and Still Birth)', N'8, 9', N'6', CAST ('False' AS bit), 15, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'13', N'Proportionofkebelesdeclared_homedeliveryfree_', N'Delivery', N'Number of kebeles that have been declared home delivery free', N'3043', N'sum', N'6', N'Total number of kebeles', N'89', N'4', CAST ('False' AS bit), 16, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.1.2', N'PMTCT;Totalindicators:7', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 17, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'PercentageofpregnantandlactatingwomenwhoweretestedforHIVandwhoknowtheirresults', N'PMTCT', N'Number of pregnant women tested and know their result during pregnancy, labour -& delivery, and postpartum', N'3044, 3045, 3501', N'sum', N'6', N'Estimated number of pregnant women', N'3', N'4', CAST ('False' AS bit), 18, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'NumberofHIVPositivepregnantandlactatingwomenwhoreceivedARTatANC+L&D+PNCforthefirsttimebasedonoptionB+.', N'PMTCT', N'Number of HIV positive -Pregnant women who received ART to reduce the risk of -mother to child transmission for the first time during ANC, Labor & Delivery, and PNC ---', N'4535', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 19, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'NumberofHIV-positivepregnantwomenwhowereonARTandlinkedtoANC', N'PMTCT', N'Number of HIV-positive women who get pregnant while on ART and linked to ANC', N'3049', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 20, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'PercentageofinfantsborntoHIVinfectedwomenreceivingavirologicaltestforHIVwithin12monthsofbirth', N'PMTCT', N'Number of HIV exposed -infants who received an HIV test within 2 months of birth, and who receive an HIV test between 2 and 12 months, during the reporting period', N'3050, 3051', N'sum', N'6', N'Total number of expected Live births from HIV positive mothers', N'98', N'4', CAST ('False' AS bit), 21, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'PercentageofInfantsborntoHIV-infectedwomenstartedonco-trimoxazoleprophylaxiswithintwomonthsofbirth', N'PMTCT', N'Number of infants born to HIV positive women started on co-trimoxazole prophylaxis within two months of birth', N'3052', N'sum', N'6', N'Estimated number of HIV- infected pregnant women who gave birth', N'54', N'4', CAST ('False' AS bit), 22, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'PercentageofinfantsborntoHIV-infectedwomenreceivingantiretroviral(ARV)prophylaxisforpreventionofmother-to-childtransmission(PMTCT', N'PMTCT', N'Number of HIV exposed infants who received antiretroviral prophylaxis at L&D and PNC', N'3053', N'sum', N'6', N'Total number of expected live births from HIV positive mothers', N'98', N'4', CAST ('False' AS bit), 23, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'PercentageofHIVexposedinfantsreceivingHIVconfirmatory(antibodytest)testby18months', N'PMTCT', N'Number of HIV exposed infants -receiving HIV confirmatory (antibody test) by 18 months- whose test result is HIV positive, and whose test result is HIV -negative', N'3054, 3055', N'sum', N'6', N'Estimated number of exposed infants', N'99', N'4', CAST ('False' AS bit), 24, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.1.3', N'ChildHealth:-TotalIndicators:15', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 25, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'DPT1-HepB1-Hib1(pentavalentFirstdose)immunizationcoverage(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received first -dose of pentavalent vaccine', N'23', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 26, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'DPT3-HepB3-Hib3(Pentavalentthirddose)immunizationcoverage(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received third dose of pentavalent vaccine', N'24', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 27, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Pneumococcalconjugatedvaccinefirstdose(PCV1)immunizationcoverage-(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received first dose of pneumococcal -vaccine', N'3001', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 28, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Pneumococcalconjugatedvaccinethirddose(PCV3)immunizationcoverage-(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received third dose of pneumococcal -vaccine', N'3003', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 29, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Rotavirusvaccinefirstdose(Rota1)immunizationcoverage-(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received first dose of Rotavirus -vaccine', N'3004', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 30, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'Rotavirusvaccineseconddose(Rota2)immunizationcoverage-(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received 2nd -dose of Rotavirus -vaccine', N'3005', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 31, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'Measlesimmunizationcoverage(<1year)', N'Child Health_Immunization', N'Number of children under one year of age who have received measles vaccine', N'25', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 32, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'8', N'FullImmunizationCoverage-(<1year)', N'Child Health_Immunization', N'Number of children received all vaccine doses before 1st birthday', N'26', N'sum', N'6', N'Estimated number of surviving infants', N'6', N'4', CAST ('False' AS bit), 33, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'MerraTestIndicator', NULL, N'MerraTestNumerator', N'191,4,192', N'sumnopercent', N'7,6,7', N'MerraTestDenominator', N'8', N'4', CAST ('False' AS bit), 239, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'9', N'Proportionofinfantsprotectedatbirthagainstneonataltetanus', N'Child Health_Immunization', N'Number of Infants whose mothers had protective doses of TT against NNT (PAB)', N'27', N'sum', N'6', N'Estimated number of live births', N'5', N'4', CAST ('False' AS bit), 34, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.1.3.10', N'VaccineWastageRate', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 35, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'BCGvaccinewastagerate', N'Vaccine Wastage', N'BCG vaccine doses given', N'28', N'sumHundredMinus', N'6', N'BCG vaccine doses opened', N'1645', N'6', CAST ('False' AS bit), 36, N'', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Pentavalentvaccinewastagerate', N'Vaccine Wastage', N'Pentavalent vaccine doses given', N'29', N'sumHundredMinus', N'6', N'Pentavalent vaccine doses opened', N'1646', N'6', CAST ('False' AS bit), 37, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Pneumococcal-conjugatedvaccinewastagerate', N'Vaccine Wastage', N'Pneumococcal conjugated vaccine doses given', N'3006', N'sumHundredMinus', N'6', N'Pneumococcal conjugated vaccine doses opened', N'3007', N'6', CAST ('False' AS bit), 38, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Rotavaccinewastagerate', N'Vaccine Wastage', N'Rota vaccine doses given', N'3008', N'sumHundredMinus', N'6', N'Rota vaccine doses opened', N'3009', N'6', CAST ('False' AS bit), 39, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Poliovaccinewastagerate', N'Vaccine Wastage', N'Polio vaccine doses given', N'30', N'sumHundredMinus', N'6', N'Polio vaccine doses opened', N'1647', N'6', CAST ('False' AS bit), 40, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'Measlesvaccinewastagerate', N'Vaccine Wastage', N'Measles vaccine doses given', N'31', N'sumHundredMinus', N'6', N'Measles vaccine doses opened', N'1648', N'6', CAST ('False' AS bit), 41, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'TTvaccinewastagerate', N'Vaccine Wastage', N'TT vaccine doses given', N'32', N'sumHundredMinus', N'6', N'TT vaccine doses opened', N'1649', N'6', CAST ('False' AS bit), 42, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 43, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Earlyinstitutionalneonataldeathrate', N'Neonatal Death', N'Number of deaths in the first 24 hrs of life, and number of -neonatal deaths -between 1 and 7 days of life/institutional/', N'16, 3968', N'sum', N'6', N'Live births', N'8', N'6', CAST ('False' AS bit), 44, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Neonataldeathrateatcommunity', N'Neonatal Death', N'Number of deaths in the first -seven days -of life in the community', N'3,057', N'sum', N'6', N'Total number of live births in the same kebele', N'104', N'4', CAST ('False' AS bit), 45, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Proportionofchildrentreatedforpneumonia', N'Pneumonia', N'Number of Children under 5 treated for pneumonia in OPD and IPD', N'43,143,410,011,004', N'sum', N'8,2', N'Estimated number of under 5 children with pneumonia', N'95', N'4', CAST ('False' AS bit), 46, N'opd', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'ProportionofnewbornstreatedforSepsis', N'Sepsis', N'Number of newborns treated for sepsis', N'155,315,541,555,155,000,000,000', N'sum', N'2', N'Estimated number of neonates with sepsis', N'96', N'4', CAST ('False' AS bit), 47, N'ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Proportionofnewbornstreatedforasphyxiaathealthfacility', N'Asphyxia', N'Number of newborns treated for asphyxia', N'154,115,421,543,154,000,000,000', N'sum', N'2', N'Estimated number of neonates with birth asphyxia', N'97', N'4', CAST ('False' AS bit), 48, N'ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.2', N'Nutrition:TotalIndicators:6', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 49, N'', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'PercentageofLowbirthweightnewborns', N'Nutrition', N'Number of live-born babies with birth weight less than 2,500 g', N'19', N'sum', N'6', N'Total number of live births weighed', N'18', N'6', CAST ('False' AS bit), 50, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'PercentageofunderweightChildrenaged<5years-', N'Nutrition', N'Number of weights recorded with moderate malnutrition, by age (Z-score below -2 and -3: Under weight), and -(Z-score below -3: Severely under weight)', N'44,844,485', N'sum', N'6', N'Number of weights measured for children under 5yrs, by age', N'4483', N'6', CAST ('False' AS bit), 51, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Proportionofchildren6-59monthswithsevereacutemalnutrition', N'Nutrition', N'Number of children screened and have severe acute malnutrition', N'3065', N'sum', N'6', N'Total Number of children screened for malnutrition', N'3064', N'6', CAST ('False' AS bit), 52, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.2.4', N'Treatmentoutcomeformanagementofsevereacutemalnutritioninchildren6-59months', N'Nutrition', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 53, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Treatmentoutcomeforchildrenrecovered', N'Nutrition', N'Number of children --recovered', N'3067', N'sum', N'6', N'Total number of children who exit from severe acute malnutrition treatment', N'4423', N'6', CAST ('False' AS bit), 54, N'', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Treatmentoutcomeforchildrendefaulted', N'Nutrition', N'Number of children --defaulted', N'3068', N'sum', N'6', N'Total number of children who exit from severe acute malnutrition treatment', N'4423', N'6', CAST ('False' AS bit), 55, N'', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Treatmentoutcomeforchildrentransferred', N'Nutrition', N'Number of children --transferred', N'3069', N'sum', N'6', N'Total number of children who exit from severe acute malnutrition treatment', N'4423', N'6', CAST ('False' AS bit), 56, N'', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Treatmentoutcomeforchildrendied', N'Nutrition', N'Number of children --died', N'3070', N'sum', N'6', N'Total number of children who exit from severe acute malnutrition treatment', N'4423', N'6', CAST ('False' AS bit), 57, N'', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 58, N'', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Proportionofchildrenaged6-59monthswhoreceivedvitaminAsupplementation', N'Vitamin A', N'Number of children aged 6-59 months supplemented with vitamin-A', N'1836', N'sum', N'6', N'Estimated number of children aged 6-59 months', N'52', N'4', CAST ('False' AS bit), 59, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Proportionofchildrenaged2-5yearsde-wormed', N'De-worm', N'Number of children aged 2-5 years de-wormed -', N'1837', N'sum', N'7', N'Estimated number of children aged 2-5 years', N'53', N'4', CAST ('False' AS bit), 60, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.3', N'HygieneandEnvironmentalHealthTotalindicators:3', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 61, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Proportionofhouseholdsaccesstolatrinefacilities', N'Hygiene and Environmental Health', N'Number of households with any type of latrine facilities (both unimproved and improved)', N'4486', N'sum', N'6', N'Total number of households', N'2', N'4', CAST ('False' AS bit), 62, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'ProportionofHHsusinglatrine', N'Hygiene and Environmental Health', N'Number of HHS utilizing latrine', N'3973', N'sum', N'6', N'Number of households with any type of latrine facilities (both unimproved and improved)', N'4486', N'6', CAST ('False' AS bit), 63, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Kebeledeclared_OpenDefecationFree''', N'Hygiene and Environmental Health', N'Number of Kebeles that have been declared open defecation free', N'4001', N'sum', N'6', N'Total number of kebeles', N'89', N'4', CAST ('False' AS bit), 64, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4', N'PreventionandControlofDiseases:Totalindicators:53', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 65, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.1', N'AllDiseases:TotalIndicators:3', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 66, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Top10Causesof-Morbidity', N'', N'All diseases labelID', N'', N'count', N'8, 2', N'', N'', N'', CAST ('False' AS bit), 67, N'opd', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Top10CausesofInstitutionalMortality', N'', N'ALL IPD', N'', N'count', N'3', N'', N'', N'', CAST ('False' AS bit), 68, N'ipd, service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Inpatientmortalityrate', N'In patient mortality', N'Number of inpatient deaths', N'4569', N'sum', N'6', N'Total number of discharges', N'180', N'6', CAST ('False' AS bit), 69, N'ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.2', N'CommunicableDiseases:Totalindicators:45', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 70, N'', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.2.1', N'HIV/AIDS:Totalindicators:-14', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 71, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'NumberofindividualsTestedandcounseledforHIVandwhoreceivedtheirtestresults', N'VCT and PICT', N'VCT and PICT Client receiving their test results', N'4536', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 72, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'NumberofPLHIVnewlyenrolledinPre-ARTcare', N'Pre-ART', N'Number of adults and children with HIV infection newly enrolled in Pre ART care', N'4491', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 73, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'HIVpositivepersonsreceivingco-trimoxazoleprophylaxis', N'Co-trimoxazole Prophylaxis', N'Number of HIV positive persons receiving CTX prophylaxis', N'3692', N'sum', N'6', N'Estimated number of -HIV positive patients -enrolled -to care and eligible to CTX according to national guideline', N'100', N'4', CAST ('False' AS bit), 74, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'NumberofeverstartedonART', N'ART', N'Number of adults and children with advanced HIV infection ever started on ART', N'4492', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 75, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Number-of-adultsandchildrenreceivingantiretroviraltherapy-(CurrentlyonART)', N'ART', N'Number of adults and children who are currently on ART', N'4493', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 76, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'ChildrenAdultRatio', NULL, N'ChildCurrentlyOnArt', N'4496', N'sum', N'6', N'AdultCurrentlyOnArt', N'4494', N'6', CAST ('False' AS bit), 240, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'NumberofadultsandchildrenwithHIVinfectionnewlystartedonART', N'ART', N'Number of adults and children with HIV infection newly started on ART', N'4503', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 77, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'SurvivalonART', N'ART', N'Number of persons on original 1st line regimen, including those on alternate 1st line regimen and those on 2nd line regimen', N'3889', N'sum', N'6', N'Number of persons on ART in the original cohort including those transferred in, minus those transferred out (net current cohort).', N'3890', N'6', CAST ('False' AS bit), 78, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'8', N'PercentageofARTpatientswithanundetectableviralloadat12monthafterinitiationofART', N'ART', N'Number of adult and pediatric patients with an undetectable viral load <1,000 copies/ml at 12 months', N'3892', N'sum', N'6', N'Number of adults and children who initiated ART in the 12 months prior to the beginning of the reporting period with a viral load count at 12 month visit', N'3893', N'6', CAST ('False' AS bit), 79, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'9', N'ProportionofclinicallyundernourishedPeopleLivingwithHIV(PLHIV)whoreceivedtherapeuticorsupplementaryfood', N'ART', N'Clinically undernourished PLHIV who are on ART plus those that are not on ART, that received therapeutic or supplementary food', N'4504', N'sum', N'6', N'Number of PLHIV that were nutritionally assessed and found to be clinically undernourished', N'3922', N'6', CAST ('False' AS bit), 80, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'10', N'NumberofHIV-positiveadultsandchildrenCurrentlyreceivingclinicalcare', N'ART', N'Number of HIV positive adults and children who Currently receive clinical Service  (clinical WHO staging or CD4 count or viral load) during the reporting period', N'4508', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 81, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'11', N'NumberofHIV-positiveadultsandchildrennewlyenrolledinclinicalcare', N'ART', N'Number of newly enrolled HIV positive adults and children who received clinical Service -(clinical WHO staging or CD4 count or viral load) during the reporting period, by age and sex', N'4509', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 82, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'12', N'NumberofpersonsprovidedwithPost-exposureprophylaxis(PEP)', N'Post-exposure prophylaxis', N'Number of persons provided with post-exposure prophylaxis (PEP) for risk of HIV infection For occupational risk and For Non occupational risk', N'4511', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 83, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'13', N'HealthFacilitiesProvidingARTthatExperiencedStock-outofatleastonerequiredARV', N'ART', N'Does the facility experienced stock-out of at least one required ARV in the reporting period', N'4003', N'count', N'', N'N/A', N'', N'7', CAST ('False' AS bit), 84, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'14', N'NumberofHIVinfectedwomenusingamodernfamilyplanningmethod', N'HIV', N'Number of HIV infected women aged 15-49 reporting the use of any method of modern family planning', N'3965, 3966, 3967', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 85, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.2.2', N'Tuberculosis:-Totalindicators:16', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 86, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Tuberculosiscasedetectionrate(Allforms)', N'Tuberculosis (TB)', N'Number of bacteriologically confirmed New PTB cases, clinically diagnosed New P/Negative TB cases, clinically diagnosed New -EPTB cases, and number of bacteriologically confirmed RELAPSE -PTB cases detected in the quarter', N'4513, 4514, 4515, 4516', N'sum', N'6', N'Estimated number of all forms of TB cases in the population during the same -period*', N'106', N'4', CAST ('False' AS bit), 87, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Tuberculosisre-treatment-rate', N'Tuberculosis (TB)', N'Treatment after relapse, Treatment after failure(F), Treatment after loss to follow-up (L), and Other previously treated (unknown & undocumented treatment outcome) (O)', N'4083, 4084, 4085, 4086, 4087, 4088, 4089, 4090', N'sum', N'6', N'Total number of all forms of TB cases registered during the reporting period', N'4513, 4514, 4515, 4516', N'6', CAST ('False' AS bit), 88, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'CurerateforbacteriologicallyconfirmednewPTBcases(CR)', N'Tuberculosis (TB)', N'Cured PTB+ -', N'4095', N'sum', N'6', N'Total number of bacteriologically confirmed TB cases enrolled in cohort', N'4464', N'6', CAST ('False' AS bit), 89, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'TreatmentSuccessRate(TSR)amongbacteriologicallyconfirmedPTBcases', N'Tuberculosis (TB)', N'Treatment completed PTB+ and Cured PTB+', N'4094, 4095', N'sum', N'6', N'Total number of bacteriologically confirmed TB cases enrolled in cohort', N'4464', N'6', CAST ('False' AS bit), 90, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'TreatmentsuccessamongofclinicallydiagnosednewTBcases-', N'Tuberculosis (TB)', N'Treatment completed P/Neg TB -and Treatment completed -EPTB cases', N'4103, 4111', N'sum', N'6', N'Total number of clinically diagnosed New -P/Neg TB and EPTB cases enrolled in the cohort (P/Neg):', N'4465, 4466', N'6', CAST ('False' AS bit), 91, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'DeathrateamongallformsofTBcases', N'Tuberculosis (TB)', N'Deaths P/Neg TB -and Deaths PTB+ and Deaths EPTB cases', N'4097, 4105, 4113', N'sum', N'6', N'Total number of bacteriologically confirmed, New P/Neg and clinically diagnosed New EPTB cases enrolled in the cohort:', N'4464, 4465, 4466', N'6', CAST ('False' AS bit), 92, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'LosttofollowuprateamongallformsofTB-', N'Tuberculosis (TB)', N'Lost to follow up -PTB+ and Lost to follow up -P/Neg TB -and Lost to follow up -EPTB cases', N'4096, 4104, 4112', N'sum', N'6', N'Total number of bacteriologically confirmed, New P/Neg and clinically diagnosed New EPTB cases enrolled in the cohort:', N'4464, 4465, 4466', N'6', CAST ('False' AS bit), 93, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'8', N'TBcaseDetectionthroughcommunityTBcare', N'Tuberculosis (TB)', N'Number of notified TB cases (all forms) referred by the community to a health facility for TB diagnosis in the quarter.', N'4517', N'sum', N'6', N'Number of bacteriologically confirmed New PTB cases, clinically diagnosed New P/Negative TB cases, clinically diagnosed New -EPTB cases, and number of bacteriologically confirmed RELAPSE -PTB cases detected in the quarter', N'4513, 4514, 4515, 4516', N'6', CAST ('False' AS bit), 94, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'9', N'ProportionofTBcases(allforms)providedtreatmentobservation(DOT)bythecommunityamongallTBcases', N'Tuberculosis (TB)', N'Number of new TB patients (all forms) registered in the same quarter of previous EFY successfully treated and provided with treatment adherence support by community health workers (HEWs)', N'4123', N'sum', N'6', N'Total number of bacteriologically confirmed, New P/Neg and clinically diagnosed New EPTB cases enrolled in the cohort:', N'4464, 4465, 4466', N'6', CAST ('False' AS bit), 95, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'10', N'ProportionofAFBMicroscopycenters(HF)withadequateEQAperformance', N'Tuberculosis (TB)', N'Count of the number of facilities where the AFB Microscopy result show a -95% concordance result on EQA blind rechecking', N'4127', N'sum', N'6', N'Count of the number of facilities that participate on EQA during the quarter?', N'4126', N'6', CAST ('False' AS bit), 96, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'12', N'ProportionofpresumptiveMDRTBcaseswithresultfordrugsusceptibilitytesting(DST)-', N'MDR Tuberculosis (TB)', N'Number of presumptive MDR TB for whom DST is performed for at least rifampicin during previous reporting period', N'4519', N'sum', N'6', N'Total number presumptive TB cases eligible for DST during previous reporting period:', N'4518', N'6', CAST ('False' AS bit), 98, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'13', N'NumberofMDRTBcasesdetected', N'MDR Tuberculosis (TB)', N'Total Number of confirmed -RR -TB cases and Total number -of confirmed -MDR TB cases', N'4520', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 99, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'14', N'MDR-TBcasesenrolledonSecondLineDrugs(SLDs)', N'MDR Tuberculosis (TB)', N'Confirmed MDR- RR/ TB cases -put on second line treatment', N'4149, 4150', N'count', N'6', N'N/A', N'', N'N/A', CAST ('False' AS bit), 100, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4.2.2.7.6', N'FinaloutcomeMDR-TBcases(reportedafter24and36monthsofenrolment)', N'MDR Tuberculosis (TB)', N'Total number of -confirmed cohort MDR-TB cases started on second line treatment for whom final outcome has been determined at 24 and 36 months', N'', N'', N'6', N'', N'', N'6', CAST ('True' AS bit), 107, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'MerraTest', NULL, N'Merranumerator', N'+439,+440,+441,-441', N'sumSubtract', N'8', N'merradenominator', N'1', N'4', CAST ('False' AS bit), 241, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'11', N'ProportionofTBcases(allforms)contributedbyprivatesector', N'Tuberculosis (TB)', N'Number of TB cases (all forms) diagnosed through private health facilities during the reporting period', N'4129', N'sum', N'6', N'Number of bacteriologically confirmed New PTB cases detected in the quarter and Number of clinically diagnosed New P/Negative TB cases detected in the quarter and Number of clinically diagnosed New -EPTB cases detected in the quarter and Number of bacteriologically confirmed RELAPSE -PTB cases detected in the quarter', N'4513, 4514, 4515, 4516', N'6', CAST ('False' AS bit), 97, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'CuredPTB+-percentageat24months', N'MDR Tuberculosis (TB)', N'Cured PTB+ -at 24 months', N'4168', N'sum', N'6', N'Total_Number_of_confirmed_MDR_TB_cases_started_on_2nd_line_treatment_for_whom_24_months_outcome_has_been_determined', N'4469', N'6', CAST ('False' AS bit), 108, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Completedtreatmentpercentage-at24months', N'MDR Tuberculosis (TB)', N'Completed at 24 months', N'4169', N'sum', N'6', N'Total_Number_of_confirmed_MDR_TB_cases_started_on_2nd_line_treatment_for_whom_24_months_outcome_has_been_determined', N'4469', N'6', CAST ('False' AS bit), 109, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Diedpercentageat24months', N'MDR Tuberculosis (TB)', N'Died at 24 months', N'4170', N'sum', N'6', N'Total_Number_of_confirmed_MDR_TB_cases_started_on_2nd_line_treatment_for_whom_24_months_outcome_has_been_determined', N'4469', N'6', CAST ('False' AS bit), 110, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Failedpercentageat24months', N'MDR Tuberculosis (TB)', N'Failed at 24 months', N'4171', N'sum', N'6', N'Total_Number_of_confirmed_MDR_TB_cases_started_on_2nd_line_treatment_for_whom_24_months_outcome_has_been_determined', N'4469', N'6', CAST ('False' AS bit), 111, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Losttofollowuppercentageat24months', N'MDR Tuberculosis (TB)', N'Lost to follow up -at 24 months', N'4172', N'sum', N'6', N'Total_Number_of_confirmed_MDR_TB_cases_started_on_2nd_line_treatment_for_whom_24_months_outcome_has_been_determined', N'4469', N'6', CAST ('False' AS bit), 112, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'Notevaluatedpercentage-at24months', N'MDR Tuberculosis (TB)', N'Not evaluated at 24 months', N'4173', N'sum', N'6', N'Total_Number_of_confirmed_MDR_TB_cases_started_on_2nd_line_treatment_for_whom_24_months_outcome_has_been_determined', N'4469', N'6', CAST ('False' AS bit), 113, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'CuredPTB+-percentageat36months', N'MDR Tuberculosis (TB)', N'Cured PTB+ -at 36 months', N'4445', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4470', N'6', CAST ('False' AS bit), 114, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'8', N'Completedtreatmentpercentage-at36months', N'MDR Tuberculosis (TB)', N'Completed at 36 months', N'4446', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4470', N'6', CAST ('False' AS bit), 115, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'9', N'Diedpercentageat36months', N'MDR Tuberculosis (TB)', N'Died at 36 months', N'4447', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4470', N'6', CAST ('False' AS bit), 116, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'10', N'Failedpercentageat36months', N'MDR Tuberculosis (TB)', N'Failed at 36 months', N'4448', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4470', N'6', CAST ('False' AS bit), 117, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'11', N'Losttofollowuppercentageat36months', N'MDR Tuberculosis (TB)', N'Lost to follow up -at 36 months', N'4449', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4470', N'6', CAST ('False' AS bit), 118, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'12', N'Notevaluatedpercentage-at36months', N'MDR Tuberculosis (TB)', N'Not evaluated at 36 months', N'4450', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4470', N'6', CAST ('False' AS bit), 119, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 120, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.2.3', N'Leprosy:Totalindicators:3', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 121, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Leprosycasenotification', N'Leprosy', N'Total number of new leprosy cases detected', N'4524', N'sum', N'6', N'Estimated number of -population in the catchment area', N'1', N'4', CAST ('False' AS bit), 122, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'GradeIIdisabilityrateamongnewcasesofleprosy', N'Leprosy', N'New leprosy cases with -Grade II disability (MB+PB)', N'4525', N'sum', N'6', N'Total number of new leprosy cases detected', N'4524', N'6', CAST ('False' AS bit), 123, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Leprosytreatmentcompletionrate', N'Leprosy', N'Treatment completed -MB cases and Treatment completed -PB cases', N'4196, 4198', N'sum', N'6', N'Registered cohort of MB/PB leprosy cases:', N'4195, 4197', N'6', CAST ('False' AS bit), 124, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.2.4', N'TB/HIVCo-infection:Totalindicators:5', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 125, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'HIVscreeningforTBpatients', N'TB HIV', N'The number of TB patients enrolled in DOTS who are tested for HIV in the quarter', N'4202, 4203', N'sum', N'6', N'The total number of TB patients enrolled in DOTS during the same period', N'4513, 4514, 4515, 4516', N'6', CAST ('False' AS bit), 126, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'TBScreeningforHIVpositiveClients', N'TB HIV', N'Number of clients enrolled in HIV care who were screened -for TB during -their last visit in the-reporting period', N'4211, 4212', N'sum', N'6', N'Total number of adults and children enrolled in HIV care and seen for care in the reporting period.', N'4215, 4216', N'6', CAST ('False' AS bit), 127, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Ant-RetroviralTherapy(ART)forHIVpositiveTBpatient', N'TB HIV', N'Total number of previously known HIV positive TB patients who are on ART and Total number of newly tested -HIV positive TB patients who are on ART', N'4526, 4527', N'sum', N'6', N'Total number of HIV positive TB patients registred during the reporting period', N'4467', N'6', CAST ('False' AS bit), 128, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'INHPreventivetherapy(IPT)forHIVpositiveclients', N'TB HIV', N'Total number of PLWHA newly enrolled in HIV care who started IPT during the quarter :', N'4528', N'sum', N'6', N'Total number of IPT eligible HIV positive clients newly enrolled in to HIV care during the reporting period.', N'4529', N'6', CAST ('False' AS bit), 129, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Co-trimoxazolepreventivetherapyduringTBtreatmentforPLHIV', N'TB HIV', N'Number of HIV-positive TB patients, registered in the reporting period, starting or continuing CPT treatment during reporting period:', N'4530', N'sum', N'6', N'Total number of HIV-positive TB patients registered during the reporting period.', N'4467', N'6', CAST ('False' AS bit), 130, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.2.5', N'Malaria:Totalindicators:5', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 131, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Morbidityattributedtomalaria', N'Malaria', N'Number of new malaria OPD + IPD cases (All malaria cases, of any species, should be included whether clinical or laboratory diagnosis.)', N'851, 852, 853, 854, 855, 856,857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298', N'sum', N'8, 2', N'Total population in the catchment area', N'1', N'4', CAST ('False' AS bit), 132, N'opd, ipd', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'FacilitybasedMalariadeaths', N'Malaria', N'The total number of all inpatient deaths with laboratory-confirmed (RDT/Microscopy) malaria', N'857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868', N'sum', N'3', N'Total number of deaths reported at health facilities during the reporting period', N'4569', N'6', CAST ('False' AS bit), 133, N'ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Malariapositivityrate', N'Malaria', N'Number of slides or RDT positive for malaria', N'4531', N'sum', N'6', N'Total number of slides or RDT performed for malaria diagnosis', N'3205', N'6', CAST ('False' AS bit), 134, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'ProportionoftargetedHHcoveredwithLLINinthelast12months', N'Malaria', N'Number of targeted HHs received at least one LLINs in the last 12 months', N'4414', N'sum', N'7', N'Number of HHs that need LLINs in the last 12 months', N'4415', N'7', CAST ('False' AS bit), 135, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'ProportionofunitstructurecoveredbyIndoorresidualspraying', N'Malaria', N'Number of unit structures covered with IRS', N'4547', N'sum', N'7', N'Total number of unit structures in target area', N'4548', N'7', CAST ('False' AS bit), 136, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.2.4.6', N'NeglectedTropicalDiseases(NTDS):2', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 137, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'TherapeuticCoverageforpreventivechemotherapydiseases(PCT)(MDAdrugfortrachoma)', N'Neglected Tropical Diseases (NTDS)', N'Number of individuals who swallowed MDA drug for trachoma', N'3974', N'sum', N'6', N'Total eligible population for Trachoma MDA treatment', N'109', N'4', CAST ('False' AS bit), 138, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'TherapeuticCoverageforpreventivechemotherapydiseases(PCT)(MDAdrugforOnchocerciasis)', N'Neglected Tropical Diseases (NTDS)', N'Number of individuals who swallowed MDA drug for Onchocerciasis', N'3975', N'sum', N'6', N'Total eligible population for Onchocerciasis MDA treatment', N'110', N'4', CAST ('False' AS bit), 139, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'TherapeuticCoverageforpreventivechemotherapydiseases(PCT)(MDAdrugforLymphaticFilariasis)', N'Neglected Tropical Diseases (NTDS)', N'Number of individuals who swallowed MDA drug for lymphatic filariasis', N'3976', N'sum', N'6', N'Total eligible population for Lymphatic Filariasis MDA treatment', N'111', N'4', CAST ('False' AS bit), 140, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'TherapeuticCoverageforpreventivechemotherapydiseases(PCT)(MDAdrugfor-Schistosomiasis)', N'Neglected Tropical Diseases (NTDS)', N'Number of individuals who swallowed MDA drug for Schistosomiasis', N'3977', N'sum', N'6', N'Total eligible population for Schistosomiasis MDA treatment', N'112', N'4', CAST ('False' AS bit), 141, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'TherapeuticCoverageforpreventivechemotherapydiseases(PCT)(MDAdrugforSoiltransmittinghelminthes(STH))', N'Neglected Tropical Diseases (NTDS)', N'Number of individuals who swallowed MDA drug for Soil transmitting helminthes (STH)', N'3978', N'sum', N'6', N'Total eligible poTotal eligible population for STH (Soil transmitting helminthes)  MDA treatmentpulation for Trachoma MDA treatment', N'113', N'4', CAST ('False' AS bit), 142, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'Numberoflymphedemacasestreated', N'Neglected Tropical Diseases (NTDS)', N'Number of Lymphatic Filariasis cases treated and Number of Podoconiosis cases treated', N'4532', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 143, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C1.4.3', N'Non-communicablediseases:5', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 144, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Morbidityattributedtohypertension', N'Hypertension', N'Number of patients diagnosed/seen with hypertension in the given period', N'667, 670, 1309, 1312', N'sum', N'8, 2', N'Estimated number of adults -in the catchment area', N'101', N'4', CAST ('False' AS bit), 145, N'opd,ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Morbidityattributedtodiabetesmellitus', N'Diabetes', N'Number of patients diagnosed/seen with diabetes mellitus in the given period', N'587, 588, 589, 590, 591, 592, 1205, 1206, 1207, 1208, 1209, 1210', N'sum', N'8, 2', N'Total population of the catchment area', N'1', N'4', CAST ('False' AS bit), 146, N'opd,ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Morbidityattributedtoasthma', N'Asthma', N'Number of patients newly diagnosed with asthma', N'689, 690, 691, 692, 693, 694, 1349, 1350, 1351, 1352, 1353, 1354', N'sum', N'8, 2', N'Total population of the catchment area', N'1', N'4', CAST ('False' AS bit), 147, N'opd,ipd', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Cervicalcancerscreeninginwomenage30-49usingVIA/PAPsmear', N'Cervical Cancer', N'Number of women age 30 49 screened once with VIA for cervical cancer', N'3385', N'sum', N'6', N'Total number of women age 30-49 within the catchment area', N'103', N'4', CAST ('False' AS bit), 148, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Cataractsurgicalrate', N'Cataract Surgery', N'Number of cataract surgeries performed', N'4327', N'sum', N'7', N'Total population in the catchment area', N'1', N'4', CAST ('False' AS bit), 149, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'C2', N'CommunityOwnership:-2', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 150, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('True' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'ProportionofModelhouseholdsgraduated/HouseholdsCurrentlyModel/', N'Community Ownership', N'((Previously graduated households plus Newly graduated) minus Drop out)', N'+3981,+4420,-4419', N'sumSubtract', N'6', N'Total number of households in the catchment area', N'2', N'4', CAST ('False' AS bit), 151, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Proportionoffunctional1to5networks', N'Community Ownership', N'Functional 1 to 5 networks', N'3206', N'sum', N'6', N'total expected number of 1 to 5 networks', N'3207', N'6', CAST ('False' AS bit), 152, N'service', CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'F1', N'ResourceMobilizationandUtilization:-4', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 153, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Generalgovernmentexpenditureonhealth', N'Finance', N'Total budget allocated to health', N'4551', N'sum', N'7', N'Total government budget', N'4550', N'7', CAST ('False' AS bit), 154, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Healthbudgetutilization', N'Finance', N'Health budget utilized', N'217', N'sum', N'7', N'Health budget allocated', N'216', N'7', CAST ('False' AS bit), 155, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Shareofinternalrevenuegenerated', N'Finance', N'Total revenue generated in year', N'221', N'sum', N'7', N'Health budget allocated', N'216', N'7', CAST ('False' AS bit), 156, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Proportionofreimbursedamountoutoftotalpatientfeeswaived', N'Finance', N'Amount of waived fee reimbursed', N'4332', N'sum', N'7', N'Amount of fees waived', N'4333', N'7', CAST ('False' AS bit), 157, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'P1', N'QualityofhealthServices:-6', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 158, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Outpatientattendancepercapita', N'Out patient', N'Number of outpatient visits', N'4533', N'sumnopercent', N'6', N'Population of the catchment area', N'1', N'4', CAST ('False' AS bit), 159, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Admissionrate', N'In patient', N'Number of admissions', N'177', N'sum', N'6', N'Population of the catchment area', N'1', N'4', CAST ('False' AS bit), 160, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Bedoccupancyrate', N'In patient', N'Total length of stay (in days)', N'178', N'sumdenomMultiply', N'6', N'(Number of beds available) x (Number of days in period)', N'179,888,888', N'6', CAST ('False' AS bit), 161, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Averagelengthofstay', N'In patient', N'Total length of stay (in days)', N'178', N'sumnopercent', N'6', N'Number of discharges', N'180', N'6', CAST ('False' AS bit), 162, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Proportionofbloodunitsutilizedfrombloodbankservice', N'Blood Bank', N'Total number of units of blood transfused', N'4534', N'sum', N'6', N'Total units of blood received from NBTS & regional blood banks', N'3208', N'6', CAST ('False' AS bit), 163, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'Seriousadversetransfusionincidentsandreactions', N'Blood Bank', N'Number of serious adverse transfusion incidents and -reactions occurred', N'3209', N'sum', N'6', N'Total number of units of blood transfused', N'3208', N'6', CAST ('False' AS bit), 164, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'P3', N'PharmaceuticalSupplyandServices:-1', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 165, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Essentialdrugsavailability', N'Essential drugs', N'Sum (tracer drugs x months available)', N'140, 141,142,143,144,145,146,147,148,149,150,3210,3211', N'count', N'6', N'N/A', N'', N'', CAST ('False' AS bit), 166, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'P5', N'EvidenceBasedDecisionMaking:-4', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 167, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'IntegratedSupportiveSupervision', N'Supportive Supervision', N'Number of supervisory visits with written feedback received', N'3982', N'sum', N'6', N'Number of supervisory visits expected per specified time period', N'3983', N'6', CAST ('False' AS bit), 168, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'ReportCompleteness', N'Report Completeness', N'Number of reports received --', N'185', N'sum', N'6', N'Total number of reports expected', N'187', N'6', CAST ('False' AS bit), 169, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'ReportTimeliness', N'Report Timeliness', N'Number of reports received timely (acording to schedule)', N'186', N'sum', N'6', N'Total number of reports expected', N'187', N'6', CAST ('False' AS bit), 170, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'DataQuality', N'Data Quality', N'Data quality', N'', N'', N'6', N'N/a', N'', N'6', CAST ('True' AS bit), 171, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4.1', N'LQAS', N'Data Quality', N'Data quality', N'188', N'count', N'6', N'N/a', N'', N'6', CAST ('False' AS bit), 172, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 0, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4.2', N'RDQA', N'Data Quality', N'Data quality', N'4545', N'count', N'6', N'N/a', N'', N'6', CAST ('False' AS bit), 173, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'CB1', N'HealthInfrastructure:-4', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 174, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'FunctionalFacilitytopopulationratio', N'Health Infrastructure', N'Number of functional health facilities', N'4556', N'sum', N'7', N'Total Population', N'1', N'4', CAST ('False' AS bit), 175, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Healthinstitutionsnewlyconstructedandupgraded', N'Health Infrastructure', N'Number of health institutions newly constructed and upgraded', N'4555', N'count', N'7', N'N/A', N'', N'', CAST ('False' AS bit), 176, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Healthinstitutionswithfunctionalinfrastructure', N'Health Infrastructure', N'', N'', N'', N'7', N'', N'', N'', CAST ('True' AS bit), 177, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3.1', N'Healthinstitutionswithfunctionalelectricityratio', N'Health Infrastructure', N'Number of health institutions with electricity', N'211', N'sum', N'7', N'Total number of health institutions', N'81,82,83,84,85,86,87', N'4', CAST ('False' AS bit), 178, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3.2', N'Healthinstitutionswithwatersupplyratio', N'Health Infrastructure', N'Number of health institutions with water supply', N'212', N'sum', N'7', N'Total number of health institutions', N'81,82,83,84,85,86,87', N'4', CAST ('False' AS bit), 179, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3.3', N'Healthinstitutionswithsanitationfacilitiesratio', N'Health Infrastructure', N'Number of health institutions with sanitation facilities', N'4341', N'sum', N'7', N'Total number of health institutions', N'81,82,83,84,85,86,87', N'4', CAST ('False' AS bit), 180, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Primaryhealthcarecoverage', N'Health Infrastructure', N'', N'', N'sum', N'7', N'', N'', N'', CAST ('False' AS bit), 181, N'service', CAST ('False' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'CB2', N'HumanCapitalandleadership:-4', N'', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 182, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'HealthStaff-topopulationratiobycategory', N'Human Resources (HR)', N'Number of health stuff at the end of the year', N'4391', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 183, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.1', N'Maledoctorstopopulationratio', N'Human Resources (HR)', N'Number of male doctors at the end of the year', N'4392', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 184, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.2', N'Femaledoctorstopopulationratio', N'Human Resources (HR)', N'Number of female doctors at the end of the year', N'4393', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 185, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.3', N'MaleESO(EmergencySurgicalOfficers)topopulationratio', N'Human Resources (HR)', N'Number of male ESO (Emergency Surgical Officers) at the end of the year', N'4394', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 186, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.4', N'FemaleESO(EmergencySurgicalOfficers)topopulationratio', N'Human Resources (HR)', N'Number of females ESO (Emergency Surgical Officers) at the end of the year', N'4395', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 187, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.5', N'Malehealthofficerstopopulationratio', N'Human Resources (HR)', N'Number of male health officers at the end of the year', N'4396', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 188, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.6', N'Femalehealthofficerstopopulationratio', N'Human Resources (HR)', N'Number of female health officers at the end of the year', N'4397', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 189, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.7', N'Malenursestopopulationratio', N'Human Resources (HR)', N'Number of male nurses at the end of the year', N'4398', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 190, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.8', N'Femalenursestopopulationratio', N'Human Resources (HR)', N'Number of female nurses at the end of the year', N'4399', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 191, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.9', N'Malemidwivestopopulationratio', N'Human Resources (HR)', N'Number of male midwives at the end of the year', N'4400', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 192, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.1', N'Femalemidwivestopopulationratio', N'Human Resources (HR)', N'Number of female midwives at the end of the year', N'4401', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 193, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.11', N'Malelabtechnicians/technologiststopopulationratio', N'Human Resources (HR)', N'Number of male lab technicians/technologists at the end of the year', N'4402', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 194, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.12', N'Femalelabtechnicians/technologisttopopulationratio', N'Human Resources (HR)', N'Number of female lab technicians/technologist at the end of the year', N'4403', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 195, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.13', N'Malephramacytechnicians/Pharmaciststopopulationratio', N'Human Resources (HR)', N'Number of male phramacy technicians/Pharmacists at the end of the year', N'4404', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 196, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.14', N'Femalephramacytechnicians/Pharmaciststopopulationratio', N'Human Resources (HR)', N'Number of female phramacy technicians/Pharmacists at the end of the year', N'4405', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 197, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.15', N'Maleenvironmentaltechnicians/environmentaliststopopulationratio', N'Human Resources (HR)', N'Number of male environmental technicians/environmentalists at the end of the year', N'4406', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 198, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.16', N'Femaleenvironmentaltechnicain/environmentaliststopopulationratio', N'Human Resources (HR)', N'Number of female environmental technicians/environmentalists at the end of the year', N'4407', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 199, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.17', N'Maleanesthetiststopopulationratio', N'Human Resources (HR)', N'Number of male anesthetists at the end of the year', N'4408', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 200, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.18', N'Femleanesthetiststopopulationratio', N'Human Resources (HR)', N'Number of female anesthetists at the end of the year', N'4409', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 201, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.19', N'Maleotherhealthprofessionalstopoplationratio', N'Human Resources (HR)', N'Number of male other health professionals at the end of the year', N'4410', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 202, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.2', N'Femaleotherhealthprofessionalstopoplationratio', N'Human Resources (HR)', N'Number of female other health professionals at the end of the year', N'4411', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 203, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.21', N'Maleadministrativestufftopopulationration', N'Human Resources (HR)', N'Number of male administrative stuff at the end of the year', N'4412', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 204, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1.22', N'Femaleadministrativestufftopopulationration', N'Human Resources (HR)', N'Number of female administrative stuff at the end of the year', N'4413', N'sum', N'7', N'Total Population', N'1', N'7', CAST ('False' AS bit), 205, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'', N'', N'Human Resources (HR)', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 206, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Healthstaffskillmix', N'Human Resources (HR)', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 207, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Maledoctorstototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male doctors at the end of the year', N'4392', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 208, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Femaledoctorstototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female doctors at the end of the year', N'4393', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 209, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'MaleESO(EmergencySurgicalOfficers)tototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male ESO (Emergency Surgical Officers) at the end of the year', N'4394', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 210, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'FemaleESO(EmergencySurgicalOfficers)tototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of females ESO (Emergency Surgical Officers) at the end of the year', N'4395', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 211, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'Malehealthofficerstototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male health officers at the end of the year', N'4396', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 212, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'6', N'Femalehealthofficerstototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female health officers at the end of the year', N'4397', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 213, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'7', N'Malenursestototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male nurses at the end of the year', N'4398', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 214, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'8', N'Femalenursestototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female nurses at the end of the year', N'4399', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 215, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'9', N'Malemidwivestototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male midwives at the end of the year', N'4400', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 216, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'10', N'Femalemidwivestototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female midwives at the end of the year', N'4401', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 217, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'11', N'Malelabtechnicians/technologiststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male lab technicians/technologists at the end of the year', N'4402', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 218, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'12', N'Femalelabtechnicians/technologisttototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female lab technicians/technologist at the end of the year', N'4403', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 219, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'13', N'Malephramacytechnicians/Pharmaciststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male phramacy technicians/Pharmacists at the end of the year', N'4404', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 220, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'14', N'Femalephramacytechnicians/Pharmaciststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female phramacy technicians/Pharmacists at the end of the year', N'4405', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 221, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15', N'Maleenvironmentaltechnicians/environmentaliststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male environmental technicians/environmentalists at the end of the year', N'4406', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 222, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'16', N'Femaleenvironmentaltechnicain/environmentaliststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female environmental technicians/environmentalists at the end of the year', N'4407', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 223, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'17', N'Maleanesthetiststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of male anesthetists at the end of the year', N'4408', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 224, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'18', N'Femleanesthetiststototalnumberofhealthworkersratio', N'Human Resources (HR)', N'Number of female anesthetists at the end of the year', N'4409', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 225, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'19', N'Maleotherhealthprofessionalstopoplationratio', N'Human Resources (HR)', N'Number of male other health professionals at the end of the year', N'4410', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 226, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'20', N'Femaleotherhealthprofessionalstopoplationratio', N'Human Resources (HR)', N'Number of female other health professionals at the end of the year', N'4411', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 227, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'21', N'Maleadministrativestufftototalnumberofhealthworkersration', N'Human Resources (HR)', N'Number of male administrative stuff at the end of the year', N'4412', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 228, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'22', N'Femaleadministrativestufftototalnumberofhealthworkersration', N'Human Resources (HR)', N'Number of female administrative stuff at the end of the year', N'4413', N'sum', N'7', N'Total number of health stuff at the end of the year', N'4391', N'7', CAST ('False' AS bit), 229, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'', N'', N'Human Resources (HR)', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 230, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'healthprofessionalattritionrate', N'Human Resources (HR)', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 231, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'1', N'Doctorsattritionrate', N'Human Resources (HR)', N'Number of doctros leaving', N'242', N'sum', N'7', N'Number of doctors at the beginning of the year', N'4346, 4347', N'7', CAST ('False' AS bit), 232, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'2', N'Healthofficersattritionrate', N'Human Resources (HR)', N'Number of health officers leaving', N'243', N'sum', N'7', N'Number of health officers at the beginning of the year', N'4350, 4351', N'7', CAST ('False' AS bit), 233, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'3', N'Nursesattritionrate', N'Human Resources (HR)', N'Number of nurses leaving', N'244', N'sum', N'7', N'Number of nurses at the beginning of the year', N'4352, 4353', N'7', CAST ('False' AS bit), 234, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Midwivesattritionrate', N'Human Resources (HR)', N'Number of midwives leaving', N'245', N'sum', N'7', N'Number of midwives at the beginning of the year', N'4354, 4355', N'7', CAST ('False' AS bit), 235, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'5', N'HealthExtensionWorkers(HEWs)attritionrate', N'Human Resources (HR)', N'Number of health extension workers (HEWs) leaving', N'4422', N'sum', N'7', N'Number of health extension workers (HEWs) at the beginning of the year', N'4456', N'7', CAST ('False' AS bit), 236, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'', N'', N'Human Resources (HR)', N'', N'', N'', N'', N'', N'', N'', CAST ('True' AS bit), 237, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'4', N'Facilitiesstaffedasperthestandard', N'Human Resources (HR)', N'', N'', N'sum', N'', N'', N'', N'', CAST ('False' AS bit), 238, N'service', CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), NULL, 2, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15', N'TotalMDRTBTreatmentSixmonthInterimresult', N'MDR Tuberculosis (TB)', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB with outcome Positive culture at 6 months, Negative culture, Died before 6 months of treatment, Lost to follow up -during the first 6 months of treatment and Not evaluated at 6 months of treatment', N'4160, 4161, 4162, 4163, 4164', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4159', N'6', CAST ('False' AS bit), 101, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15.2', N'MDRTBTreatmentSixmonthInterimresultNegative', N'MDR Tuberculosis (TB)', N'MDRTBTreatmentSixmonthInterimresultNegative', N'4161', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4159', N'6', CAST ('False' AS bit), 103, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15.3', N'MDRTBTreatmentSixmonthInterimresultDied', N'MDR Tuberculosis (TB)', N'MDRTBTreatmentSixmonthInterimresultDied', N'4162', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4159', N'6', CAST ('False' AS bit), 104, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15.4', N'MDRTBTreatmentSixmonthInterimresultLostToFollowUp', N'MDR Tuberculosis (TB)', N'MDRTBTreatmentSixmonthInterimresultLostToFollowUp', N'4163', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4159', N'6', CAST ('False' AS bit), 105, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15.5', N'MDRTBTreatmentSixmonthInterimresultNotEvaluated', N'MDR Tuberculosis (TB)', N'MDRTBTreatmentSixmonthInterimresultNotEvaluated', N'4164', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4159', N'6', CAST ('False' AS bit), 106, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
INSERT INTO [dbo].[EthEhmis_IndicatorsNewDisplay] ([SNO], [IndicatorName], [Category1], [NumeratorName], [NumeratorLabelId], [Actions], [NumeratorDataEleClass], [DenominatorName], [DenominatorLabelId], [DenominatorDataEleClass], [ReadOnly], [SequenceNo], [ReportType], [HP], [HC], [Hospital], [WorHo], [annual], [commonAnnual], [PeriodType], [commonQuarterly], [targetDivide])
	VALUES (N'15.1', N'MDRTBTreatmentSixmonthInterimresultPositive', N'MDR Tuberculosis (TB)', N'MDRTBTreatmentSixmonthInterimresultPositive', N'4160', N'sum', N'6', N'Total Number of -cohort RR/ MDR-TB cases initiated on second-line anti-TB', N'4159', N'6', CAST ('False' AS bit), 102, N'service', CAST ('False' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('True' AS bit), CAST ('False' AS bit), CAST ('False' AS bit), 1, CAST ('False' AS bit), CAST ('False' AS bit))

GO
