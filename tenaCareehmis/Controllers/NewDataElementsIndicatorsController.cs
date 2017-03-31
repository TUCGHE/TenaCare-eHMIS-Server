/*
 * 
 * Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
 * dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
 * All rights reserved.

 * This file is part of TenaCareeHMIS
 * TenaCareeHMIS is free software: 
 * 
 * you can redistribute it and/or modify it under the terms of the 
 * GNU General Public License as published by the Free Software Foundation, 
 * version 3 of the License, or any later version.
 * TenaCareeHMIS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
 * If not, see http://www.gnu.org/licenses/.    
 * 
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SqlManagement.Database;
using System.Data.SqlClient;
using System.Data;
using eHMISWebApi.Models;

namespace eHMISWebApi.Controllers
{
    public class NewDataElementsIndicatorsController : ApiController
    {
        eHMISEntities db = new eHMISEntities();
        DataTable dtLastEntry = null, dtMAX = null, dtMAXID = null;
        DBConnHelper _helper = new DBConnHelper();
        [EnableCors("*", "*", "*")]
        public HttpStatusCode Post([FromBody]NewDataElementsValues values)
        {
            string cmdText = "select Top 1 ID, GroupID, SequenceNo from [dbo].[EthioHIMS_ServiceDataElementsNew] order by ID DESC";
            SqlCommand getLastEntry = new SqlCommand(cmdText);
            dtLastEntry = _helper.GetDataSet(getLastEntry).Tables[0];

            string cmdText2 = "select Max(LabelID) as LabelID from EthioHIMS_ServiceDataElementsNew";
            SqlCommand getMAX= new SqlCommand(cmdText2);
            dtMAX = _helper.GetDataSet(getMAX).Tables[0];


            EthioHIMS_ServiceDataElementsNew serviceDataElement = new EthioHIMS_ServiceDataElementsNew();
            serviceDataElement.ID = int.Parse(dtLastEntry.Rows[0]["ID"].ToString()) + 1;
            serviceDataElement.GroupID = int.Parse(dtLastEntry.Rows[0]["GroupID"].ToString()) + 1;
            serviceDataElement.LabelID = int.Parse(dtMAX.Rows[0]["LabelID"].ToString()) + 1;
            serviceDataElement.SNO = DBNull.Value.ToString();
            serviceDataElement.ActivityHP = values.Name;
            serviceDataElement.ActivityHC = values.Name;
            serviceDataElement.ActivityWorHO = values.Name;
            serviceDataElement.FullDescription = values.Name;
            serviceDataElement.PeriodType = values.Type;
            serviceDataElement.Readonly = false;
            serviceDataElement.SequenceNo = serviceDataElement.SequenceNo = int.Parse(dtLastEntry.Rows[0]["SequenceNo"].ToString()) + 1;
            serviceDataElement.AggregationType = 0;
            serviceDataElement.HP = true;
            serviceDataElement.HC = true;
            serviceDataElement.HCHP = true;
            serviceDataElement.Hospital = true;
            serviceDataElement.WHO = true;
            serviceDataElement.ZHD = true;
            serviceDataElement.RHB = true;
            serviceDataElement.FMOH = true;
            serviceDataElement.Private = true;

            cmdText = "insert into EthioHIMS_ServiceDataElementsNew(ID, GroupID, LabelID, ActivityHP, ActivityHC, ActivityWorHO, FullDescription, Category1, PeriodType, " +
                "ReadOnly, SequenceNo, HP, HC, HCHP, Hospital, WHO, ZHD, RHB, FMOH, Private) values(" + serviceDataElement.ID + ", " + serviceDataElement.GroupID + ", " + serviceDataElement.LabelID + ", N'" +
                serviceDataElement.ActivityHP + "', N'" + serviceDataElement.ActivityHC + "', N'" + serviceDataElement.ActivityWorHO + "', N'" + serviceDataElement.FullDescription + "', N'" + values.Category + "', "+
                serviceDataElement.PeriodType + ", '" + serviceDataElement.Readonly + "', " + serviceDataElement.SequenceNo + ", '" + serviceDataElement.HP + "', '" +
                serviceDataElement.HC + "', '" + serviceDataElement.HCHP + "', '" + serviceDataElement.Hospital + "', '" + serviceDataElement.WHO + "', '" + serviceDataElement.ZHD + "', '" +
                serviceDataElement.RHB + "', '" + serviceDataElement.FMOH + "', '" + serviceDataElement.Private + "')";
            getLastEntry = new SqlCommand(cmdText);
          int result = _helper.Execute(getLastEntry);
            //  db.Entry<EthioHIMS_ServiceDataElementsNew>(serviceDataElement).State = System.Data.Entity.EntityState.Added;
            // db.SaveChanges();
            if (result != -1)
            {
                return HttpStatusCode.OK;

            }
            else
                return HttpStatusCode.RequestTimeout;
           // return HttpStatusCode.OK;
        }
        [EnableCors("*", "*", "*")]
        public HttpStatusCode Post(int id, [FromBody]NewIndicator values)
        {
            string cmdText = "select Max(SequenceNo) as SequenceNo from[dbo].[EthEhmis_IndicatorsNewDisplay]";
            SqlCommand getMaxSeq = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(getMaxSeq).Tables[0];
            int sequenceNo = Convert.ToInt16( dt.Rows[0]["SequenceNo"]) + 1;
            string action = null;
            string service = "service";

            if (values.LabelIdN.Contains('-'))
            {
                values.LabelIdN = "+" + values.LabelIdN;
                action = "sumSubtract";
            }
            else if (values.LabelIdD == null)
            {
                action = "count";
                values.LabelIdN.Replace("+", "");
            }
            else if (values.Action == "*")
            {
                action = "sumdenomMultiply";
                string replacedNumerator = values.LabelIdN.Replace("+", "");
                string replacedDenominator = values.LabelIdD.Replace("+", "");
                values.LabelIdN = replacedNumerator;
                values.LabelIdD = replacedDenominator;
            }
            else if (values.Percentaile == false)
            {
                action = "sumnopercent";
                string replacedNumerator = values.LabelIdN.Replace("+", "");
                string replacedDenominator = values.LabelIdD.Replace("+", "");
                values.LabelIdN = replacedNumerator;
                values.LabelIdD = replacedDenominator;
            }
            else
            {
                action = "sum";
                string replacedNumerator = values.LabelIdN.Replace("+", "");
                string replacedDenominator = values.LabelIdD.Replace("+", "");
                values.LabelIdN = replacedNumerator;
                values.LabelIdD = replacedDenominator;

            }

            cmdText = "insert into EthEhmis_IndicatorsNewDisplay(SNO, IndicatorName, NumeratorName, NumeratorLabelId, Actions, NumeratorDataEleClass" + 
                " , DenominatorName, DenominatorLabelId, DenominatorDataEleClass, ReadOnly, SequenceNo, ReportType, HP, HC, Hospital, WorHo , annual, " + 
                " commonAnnual, PeriodType, commonQuarterly, targetDivide) values ('" + 1 + "', N'" + values.IndicatorName + "', N'" + values.NumeratorName +
                "', '" + values.LabelIdN + "', N'" + action + "', '" + values.DataEleClassN + "', N'" + values.DenominatorName + "', '" + values.LabelIdD +
                "', '" + values.DataEleClassD + "', " + 0+ ", " + sequenceNo +", '" + service + "', " + 1 + ", " + 1 + ", " + 1 + ", " + 1 + ", " + 0 + ", " + 0 +
                ", " + 0 + ", " + 0 + ", " + 0  + ")";
            SqlCommand insertNewIndicator = new SqlCommand(cmdText);
            _helper.Execute(insertNewIndicator);

            return HttpStatusCode.OK;
        }

        [EnableCors("*", "*", "*")]
        public HttpStatusCode Post(int id, string diseaseName)
        {
            if (id == 1)
            {
                // IPD Disease View and Opd Diseasse View
                string cmdText = "select Top 1 SNo from EthioHIMS_QuarterOPDDiseaseView4 order by Id  desc ";
                SqlCommand getLastEntry = new SqlCommand(cmdText);
                dtLastEntry = _helper.GetDataSet(getLastEntry).Tables[0];

                string cmdText2 = "select Max(MF15) as MF15 from EthioHIMS_QuarterIPDDiseaseView;";
                SqlCommand getMAX = new SqlCommand(cmdText2);
                dtMAX = _helper.GetDataSet(getMAX).Tables[0];

                string cmdText3 = "select Max(Id) as Id from DiseaseDictionary";
                SqlCommand getMaxId = new SqlCommand(cmdText3);
                dtMAXID = _helper.GetDataSet(getMaxId).Tables[0];

                EthioHIMS_QuarterOPDDiseaseView4 opdDiseaseView = new EthioHIMS_QuarterOPDDiseaseView4();
                opdDiseaseView.SNO = Convert.ToString(int.Parse(dtLastEntry.Rows[0]["SNO"].ToString()) + 1);
                opdDiseaseView.Disease = diseaseName;
                opdDiseaseView.M04 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 1;
                opdDiseaseView.M514 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 2;
                opdDiseaseView.M15 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 3;
                opdDiseaseView.F04 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 4;
                opdDiseaseView.F514 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 5;
                opdDiseaseView.F15 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 6;

                db.Entry(opdDiseaseView).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();

                EthioHIMS_QuarterIPDDiseaseView ipdDiseaseView = new EthioHIMS_QuarterIPDDiseaseView();
                ipdDiseaseView.SNO = Convert.ToString(int.Parse(dtLastEntry.Rows[0]["SNO"].ToString()) + 1);
                ipdDiseaseView.Disease = diseaseName;
                ipdDiseaseView.M04 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 7;
                ipdDiseaseView.M514 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 8;
                ipdDiseaseView.M15 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 9;
                ipdDiseaseView.F04 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 10;
                ipdDiseaseView.F514 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 11;
                ipdDiseaseView.F15 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 12;

                ipdDiseaseView.MM04 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 7;
                ipdDiseaseView.MM514 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 8;
                ipdDiseaseView.MM15 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 9;
                ipdDiseaseView.MF04 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 10;
                ipdDiseaseView.MF514 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 11;
                ipdDiseaseView.MF15 = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 12;

                db.Entry(ipdDiseaseView).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();

                // DiseaseDictionary
                int[] classAndLabel = { 20, 30, 80 };
                int[] dataEleClass = { 2, 3, 8 };
                string[] gender = { "M", "F" };
                string[] age = { "Under_5", "5_14", "Above_15" };

                int idDisease = Convert.ToInt32(dtMAXID.Rows[0]["Id"]);
                int labelId = Convert.ToInt32(dtMAX.Rows[0]["MF15"]) + 6;// to be the same with the ipd
                int sno = Convert.ToInt32(dtLastEntry.Rows[0]["SNO"]);

                int idDictinoary, labelIdDictionary, dataEleClassDict, classAndLabelDictionary, snoDictinoary;
                string genderDictinoary = null, ageDictionary = null, disease, discription;
                disease = diseaseName;
                discription = diseaseName;
                idDictinoary = idDisease + 1;
                labelIdDictionary = labelId;
                snoDictinoary = sno + 1;
                for (int i = 0; i < 3; i++)
                {
                    int count = 0;

                    for (int j = 0; j < 6; j++)
                    {
                        labelIdDictionary++;
                        dataEleClassDict = dataEleClass[i];
                        classAndLabelDictionary = Convert.ToInt32(classAndLabel[i].ToString() + labelIdDictionary.ToString());
                        if (count < 3)
                        {
                            genderDictinoary = gender[0];
                            ageDictionary = age[count];
                        }
                        else if (count >= 3)
                        {
                            genderDictinoary = gender[1];
                            ageDictionary = age[count - 3];
                        }
                        count++;
                        string cmdInsert = "insert into DiseaseDictionary(id, sno, labelid, dataEleClass, descrip, classAndLabel, gender, age, disease) values ( " +
                            idDictinoary + ", " + snoDictinoary + ", " + labelIdDictionary + ", " + dataEleClassDict + ", N'" + discription + "', " + classAndLabelDictionary + ", '" +
                            genderDictinoary + "', '" + ageDictionary + "', N'" + disease + "')";
                        SqlCommand insertDiseaseDictinoary = new SqlCommand(cmdInsert);
                        _helper.Execute(insertDiseaseDictinoary);
                    }
                }
            }
            else if (id == 2)
            {

            }

            // jdfka.AsEnumerable();
            return HttpStatusCode.OK;

        }
        [EnableCors("*", "*", "*")]
        public DataTable Get()
        {
            string cmdText = "select * from v_EthEhmis_HMIS_LabelDescriptions_ClassLabel_Gender_Age";
            SqlCommand getRows = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(getRows).Tables[0];
            return dt;
        }

    }
    public class NewDataElementsValues
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Type { get; set; }
    }
    public class NewIndicator
    {
        public string IndicatorName { get; set; }
        public string NumeratorName { get; set; }
        public string DenominatorName { get; set; }
        public string LabelIdN { get; set; }
        public string LabelIdD { get; set; }
        public string DataEleClassN { get; set; }
        public string DataEleClassD { get; set; }
        public string Action { get; set; }
        public bool Percentaile { get; set; }
    }
}
