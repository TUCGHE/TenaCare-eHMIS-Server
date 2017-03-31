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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using eHMISWebApi.Models;
using System.Web.Http.Cors;
using System.Data.SqlClient;
using SqlManagement.Database;
using System.Collections;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ValuesController : ApiController
    {
        eHMISEntities db = new eHMISEntities();
        
        // GET api/values
        public IEnumerable<EthEhmis_HMISValue> Get([FromUri] string limit = "100")
        {
            int l = 1000;
            try
            {
                l = int.Parse(limit);
            }
            catch (Exception) { }

            return db.EthEhmis_HMISValue.Take(l);
        }

        public DataTable getHmisData([FromUri] int year, int quarter, int week, int month, int dataEleClass,int regionid )
        {

            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "SELECT        dbo.EthEhmis_HMISValue.LabelID, dbo.EthEhmis_HMISValue.RegionID, SUM(dbo.EthEhmis_HMISValue.Value) AS Value, dbo.EthEhmis_HMISValue.Year, dbo.EthEhmis_HMISValue.Month" +
                            " FROM            dbo.EthEhmis_HMISValue INNER JOIN" +
                            " dbo.labelidForValidation ON dbo.EthEhmis_HMISValue.LabelID = dbo.labelidForValidation.labelId" +
                            " GROUP BY dbo.EthEhmis_HMISValue.LabelID, dbo.EthEhmis_HMISValue.RegionID, dbo.EthEhmis_HMISValue.Year, dbo.EthEhmis_HMISValue.Month, dbo.EthEhmis_HMISValue.DataEleClass" +
                            " having Year = " + year + " and Month = " + month + " and RegionID = " + regionid + " and dbo.EthEhmis_HMISValue.DataEleClass = " + dataEleClass;

            SqlCommand getFacilityCmd = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(getFacilityCmd).Tables[0];

            return dt;
        }

        public DataTable getHmisDataByLevel([FromUri] int year, int quarter, int week, int month, int dataEleClass, int id,string level)
        {

            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "SELECT        dbo.EthEhmis_HMISValue.LabelID, dbo.EthEhmis_HMISValue." + level + ", SUM(dbo.EthEhmis_HMISValue.Value) AS Value, dbo.EthEhmis_HMISValue.Year, dbo.EthEhmis_HMISValue.Month" +
                            " FROM            dbo.EthEhmis_HMISValue INNER JOIN" +
                            " dbo.labelidForValidation ON dbo.EthEhmis_HMISValue.LabelID = dbo.labelidForValidation.labelId" +
                            " GROUP BY dbo.EthEhmis_HMISValue.LabelID, dbo.EthEhmis_HMISValue."+ level + ", dbo.EthEhmis_HMISValue.Year, dbo.EthEhmis_HMISValue.Month, dbo.EthEhmis_HMISValue.DataEleClass" +
                            " having Year = " + year + " and Month = " + month + " and "+ level + " = '" + id + "' and dbo.EthEhmis_HMISValue.DataEleClass = " + dataEleClass;

            SqlCommand getFacilityCmd = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(getFacilityCmd).Tables[0];

            return dt;
        }

        //year, quarter, value, lbid
        // GET api/values                                                      
        public IEnumerable<EthEhmis_HMISValue> getQuarterOpdDisease([FromUri] int year, int quarter, string LocationID)
        {
            return db.EthEhmis_HMISValue.Where(v => v.Year == year &&
                                               v.Quarter == quarter &&
                                               v.DataEleClass == 8 &&
                                               v.LocationID == LocationID);
        }

        //year, quarter, value, lbid
        // GET api/values                                           
        //[Route("api/values/getHmisData")]
        public IEnumerable<EthEhmis_HMISValue> getHmisData([FromUri] int year, int quarter,int week,int month, int dataEleClass , string locationID)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_HMISValue.Where(v => v.Year == year &&
                                               v.Quarter == quarter &&
                                               v.DataEleClass == dataEleClass &&
                                               v.Month == month &&
                                               v.Week == week &&
                                               v.LocationID == locationID);
        }

        //year, quarter, value, lbid
        // GET api/values                                           
        //[Route("api/values/getHmisData")]
        public IEnumerable<EthEhmis_HMISValue> getHmisData([FromUri] int year, int quarter, int week, int month, int dataEleClass)
        {
            var intList = new List<int>() { 3068,4423, 3067, 3068, 3069, 3070 };
            db.Database.CommandTimeout = Int32.MaxValue;
            IEnumerable<EthEhmis_HMISValue> temp = db.EthEhmis_HMISValue.Where(v => v.Year == year &&
                                              v.Quarter == quarter &&
                                              v.DataEleClass == dataEleClass &&
                                              v.Month == month &&
                                              v.Week == week &&  
                                              v.RegionID == 1 
                                              //v.LabelID == 3014 && v.LabelID == 3014 && v.LabelID == 3015 && v.LabelID == 3016 && v.LabelID == 3017 && v.LabelID == 3023 && v.LabelID == 3024 && v.LabelID == 3025 && v.LabelID == 3027 
                                              //intList.Contains(v.LabelID)
                                              ) ;

            return temp;
        }

        

        //year, quarter, value, lbid
        // GET api/values                                           
        //[Route("api/values/getHmisData")]
        public IEnumerable<EthEhmis_HMISValue> getHmisData([FromUri] int year, int quarter, int week, int month, int dataEleClassMorbidity, int dataEleClassMortality, string locationID)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_HMISValue.Where(v => v.Year == year &&
                                               v.Quarter == quarter &&
                                               (v.DataEleClass == dataEleClassMorbidity || v.DataEleClass == dataEleClassMortality) &&
                                               v.Month == month &&
                                               v.Week == week &&
                                               v.LocationID == locationID);
        }

        // GET api/values/5
        /*
        public JArray Get(string id, [FromUri] string limit = "100")
        {
            return DataEngine.GetValues(id, null, null, null, null, limit == null ? "100" : limit, false);
        }*/

        // POST api/values

        public EthEhmis_HMISValue Post([FromBody]EthEhmis_HMISValue value)
        {
            try
            {
                db.Entry<EthEhmis_HMISValue>(value).State = EntityState.Modified;
                db.SaveChanges();
                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            EthEhmis_HMISValue val = db.EthEhmis_HMISValue.Find(new int[] { id });
            db.EthEhmis_HMISValue.Remove(val);
            db.SaveChanges();
        }
        [Route("api/values/save")]    
        public string Post([FromBody]EthEhmis_HMISValue[] values)
        {
            try
            {

               string result = "Data updated successfully.";
                foreach (EthEhmis_HMISValue v in values)
                {
                    EthEhmis_HMISValue valInDb = (EthEhmis_HMISValue)db.EthEhmis_HMISValue.Where(va => va.LabelID == v.LabelID &&
                                                                              va.DataEleClass == v.DataEleClass &&
                                                                              va.LocationID == v.LocationID &&
                                                                              va.Quarter == v.Quarter &&
                                                                              va.Month == v.Month &&
                                                                              va.Week == v.Week &&
                                                                              va.Year == v.Year).SingleOrDefault();


                    if (valInDb != null)
                    {
                        valInDb.Value = v.Value;
                    }
                    else
                    {
                        db.Entry(v).State = EntityState.Added;
                    }

                    db.SaveChanges();
                }
                
                return result;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        //[Route("api/values/validate")]
        //public ValidateMessage validate([FromBody]EthEhmis_HMISValue value)
        //{
        //    ValidateMessage validationResult = new ValidateMessage();
        //    validationResult = newValidateCell(value);
        //    return validationResult;
        //}
        Hashtable labelIdRowHash = new Hashtable();
        Hashtable rowLabelIdHash = new Hashtable();
        Hashtable rowSequenceHash = new Hashtable();

        Hashtable verticalSumIdRowHash = new Hashtable();
        Hashtable sequenceToRowHash = new Hashtable();
        Hashtable dtValidation2 = new Hashtable();
        Hashtable rowPrevValue = new Hashtable();

        private int _expectedDataElements = 0;
        private int _filledDataElements = 0;

        private DataTable dtServiceDataElements;
        private DataTable dtConstraints;
        private DataTable dtVerticalSum;
        private DataTable dtValidation;

        private string _serviceDataElementsTable;
        private string _verticalSumTable;
        private string _constraintsTable;

        private void prepareTables(EthEhmis_HMISValue valueObj)
        {
            _serviceDataElementsTable = "EthioHIMS_ServiceDataElementsNew";
            _constraintsTable = "EthioHIMS_ConstraintsNew";
            _verticalSumTable = "EthioHIMS_VerticalSumNew";
            string deletestatment = " DELETE FROM EthioHIMS_QuarterServiceView WHERE 1=1";

            using (DBConnHelper dbConnHelper = new DBConnHelper())
            {

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = deletestatment;
                cmd.CommandType = CommandType.Text;

                try
                {
                    Int32 rowsAffected = dbConnHelper.Execute(cmd);
                }
                catch (System.Data.SqlClient.SqlException ep)
                {

                }

                string strsql = "SELECT * FROM " + _serviceDataElementsTable + " WHERE (periodtype=0 OR periodtype=1) ";


                string copystatment;

                if (valueObj.Level > 0)
                {

                    copystatment = "INSERT INTO  [dbo].[EthioHIMS_QuarterServiceView]" +
                                   "([SNO],[Activity],[Value],[Format]) " +
                                   "SELECT [SNO],[ActivityWorHO],[LabelID],[SequenceNo]" +
                                   " FROM  [dbo]." + _serviceDataElementsTable + " WHERE (periodtype=0 OR periodtype=1) AND WHO=1 order by sequenceno";
                    strsql += " AND WHO=1";

                }
                else
                {
                    if ((valueObj.FACILITTYPE == 11) || (valueObj.FACILITTYPE == 1) || (valueObj.FACILITTYPE == 2))
                    {
                        copystatment = "INSERT INTO  [dbo].[EthioHIMS_QuarterServiceView]" +
                                    "([SNO],[Activity],[Value],[Format]) " +
                                     "SELECT [SNO],[ActivityHC],[LabelID],[SequenceNo]" +
                                    " FROM  [dbo]." + _serviceDataElementsTable + " WHERE (periodtype=0 OR periodtype=1) AND (Hospital=1 OR HC=1) order by sequenceno ";
                        strsql += " AND HC=1";
                    }
                    else
                    {
                        copystatment = "INSERT INTO  [dbo].[EthioHIMS_QuarterServiceView]" +
                                                      "([SNO],[Activity],[Value],[Format])" +
                                                       "SELECT [SNO],[ActivityHP],[LabelID],[SequenceNo]" +
                                                       " FROM  [dbo]." + _serviceDataElementsTable + " WHERE (periodtype=0 OR periodtype=1) AND HP=1 order by sequenceno";

                        strsql += " AND HP=1";
                    }
                }

                dtServiceDataElements = dbConnHelper.GetDataSet(strsql + " ORDER BY sequenceno").Tables[0];
                dtConstraints = dbConnHelper.GetDataSet("SELECT * FROM " + _constraintsTable).Tables[0];
                dtVerticalSum = dbConnHelper.GetDataSet("SELECT * FROM " + _verticalSumTable).Tables[0];

                string wherestm = "";
                if (valueObj.FACILITTYPE == 1)
                    wherestm = "where Hosp=1";
                else if (valueObj.FACILITTYPE == 2)
                    wherestm = "where HC=1";
                else if (valueObj.FACILITTYPE == 3)
                    wherestm = "where HP=1";
                dtValidation = dbConnHelper.GetDataSet("SELECT * FROM ethEhmis_NewValidation  " + wherestm).Tables[0];
                cmd = new SqlCommand();
                cmd.CommandText = copystatment;
                cmd.CommandType = CommandType.Text;
                try
                {
                    Int32 rowsAffected = dbConnHelper.Execute(cmd);
                }
                catch (System.Data.SqlClient.SqlException ep)
                {

                }

            }

        }

        private decimal sumLabelIds(string[] labelIds, EthEhmis_HMISValue valueObj, out bool fullyFilled, bool fullFilledRequired)
        {
            decimal sum = 0;
            fullyFilled = true;

            foreach (string labels in labelIds)
            {
                string trimLabels = labels.Trim();
                int rowNum = Convert.ToInt32(labelIdRowHash[trimLabels]);

                decimal rowValue = 0;

                if (valueObj.Value > 0)
                {
                    if (valueObj.Value > 0)
                    {
                        rowValue = valueObj.Value;
                        sum += rowValue;
                    }
                    else
                    {
                        fullyFilled = false;
                        if (fullFilledRequired == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    fullyFilled = false;
                    if (fullFilledRequired == true)
                    {
                        break;
                    }
                }
            }

            return sum;
        }

        //private ValidateMessage newValidateCell(EthEhmis_HMISValue valueObj)
        //{
        //    ValidateMessage validateMessage = new ValidateMessage();
        //    using (DBConnHelper dbConnHelper = new DBConnHelper())
        //    {
        //        string facilityType = "";
        //        if (valueObj.FACILITTYPE == 3) // Health Post
        //        {
        //            facilityType = "  and hp = 1 ";
        //        }
        //        else
        //        {
        //            facilityType = "  and hc = 1 ";
        //        }

        //        string dataEleClass = " and dataEleClass = 6 ";

        //        string myLabelId = "";

        //        myLabelId = valueObj.LabelID.ToString();


        //        string cmdText = " select * from EthValidationdb " +
        //                         " where labelId = " + myLabelId + facilityType + dataEleClass;

        //        SqlCommand toExecute = new SqlCommand(cmdText);

        //        toExecute.CommandTimeout = 4000; //300 // = 1000000;
        //        DataTable dt = dbConnHelper.GetDataSet(toExecute).Tables[0];

        //        string sumRule = "";
        //        string message = "";
        //        string ruleType = "";
        //        bool fullyFilledRequired = true;
        //        bool leftValidate = true;
        //        bool rightValidate = true;
        //        bool oneOrZero = false;
        //        bool incrementTen = false;
        //        bool valid = true;

        //        bool validationExists = false;

        //        int count = 1;
        //        int countValidation = 0;
        //        foreach (DataRow row1 in dt.Rows)
        //        {
        //            countValidation++;

        //            sumRule = row1["sumRule"].ToString();
        //            message = row1["message"].ToString();
        //            ruleType = row1["ruletype"].ToString();
        //            string leftValidateString = row1["leftValidate"].ToString();
        //            if (leftValidateString != "")
        //            {
        //                leftValidate = Convert.ToBoolean(row1["leftValidate"].ToString());
        //            }
        //            string rightValidatestring = row1["rightValidate"].ToString();
        //            if (rightValidatestring != "")
        //            {
        //                rightValidate = Convert.ToBoolean(row1["rightValidate"].ToString());
        //            }
        //            string fullyFilledString = row1["fullyfilled"].ToString();
        //            if (fullyFilledString != "")
        //            {
        //                fullyFilledRequired = Convert.ToBoolean(row1["fullyfilled"].ToString());
        //            }
        //            string onezeroString = row1["oneOrZero"].ToString();
        //            if (onezeroString != "")
        //            {
        //                oneOrZero = Convert.ToBoolean(row1["oneOrZero"].ToString());
        //            }

        //            string incrementTenString = row1["incrementTen"].ToString();
        //            if (incrementTenString != "")
        //            {
        //                incrementTen = Convert.ToBoolean(row1["incrementTen"].ToString());
        //            }

        //            if (oneOrZero == true)
        //            {
        //                decimal oneZeroTempVal = valueObj.Value;

        //                if ((oneZeroTempVal == 1) || (oneZeroTempVal == 0))
        //                {
        //                    valid = true;
        //                }
        //                else
        //                {
        //                    valid = false;
        //                }

        //            }
        //            else if (sumRule.ToUpper().Contains("RANGE"))
        //            {
        //                decimal valToCheck = valueObj.Value;
        //                string rule = sumRule.ToUpper().Replace("RANGE", "").Trim();
        //                //string
        //                string[] ruleSplit = rule.Split('-');
        //                decimal leftCompare = Convert.ToDecimal(ruleSplit[0]);
        //                decimal rightCompare = Convert.ToDecimal(ruleSplit[1]);

        //                if ((valToCheck < leftCompare) || (valToCheck > rightCompare))
        //                {
        //                    valid = false;
        //                }
        //                else
        //                {
        //                    valid = true;
        //                }
        //            }
        //            else if (incrementTen == true)
        //            {
        //                decimal incrementTenTempVal = valueObj.Value;
        //                decimal remainder = incrementTenTempVal % 10;
        //                if (remainder != 0)
        //                {
        //                    valid = false;
        //                }
        //                else
        //                {
        //                    valid = true;
        //                }
        //            }
        //            else if (sumRule.ToUpper().Contains("INCREMENT5"))
        //            {
        //                decimal incrementTenTempVal = valueObj.Value;
        //                decimal remainder = incrementTenTempVal % 5;
        //                if (remainder != 0)
        //                {
        //                    valid = false;
        //                }
        //                else
        //                {
        //                    valid = true;
        //                }
        //            }
        //            else if (sumRule.ToUpper().Contains("NONZERO"))
        //            {
        //                valid = true;


        //                decimal valToCheck = valueObj.Value;

        //                string rule = sumRule.ToUpper().Replace("NONZERO", "").Trim();

        //                //string
        //                string[] ruleSplit = rule.Split(',');

        //                //split label ids
        //                string firstLabelId = ruleSplit[0].Trim();
        //                string secondLabelId = ruleSplit[1].Trim();
        //                string thirdLabelId = ruleSplit[2].Trim();

        //                int rowNum1 = Convert.ToInt32(labelIdRowHash[firstLabelId]);
        //                int rowNum2 = Convert.ToInt32(labelIdRowHash[secondLabelId]);
        //                int rowNum3 = Convert.ToInt32(labelIdRowHash[thirdLabelId]);

        //                decimal row1Value = 0;
        //                decimal row2Value = 0;
        //                decimal row3Value = 0;
                        
        //                row1Value = valueObj.Value;
        //                row2Value = valueObj.Value;

        //                if (row2Value != 0)
        //                {
        //                    valid = false;
        //                }
        //                 row3Value = valueObj.Value;

        //                if (row3Value != 0)
        //                {
        //                    valid = false;
        //                }
        //            }
        //            else
        //            {
        //                validationExists = true;

        //                char splitChar = ' ';
        //                string operatorToUse = "";

        //                if (sumRule.Contains("="))
        //                {
        //                    if (sumRule.Contains(">="))
        //                    {
        //                        splitChar = '>';
        //                        operatorToUse = ">=";
        //                    }
        //                    else if (sumRule.Contains("<="))
        //                    {
        //                        splitChar = '<';
        //                        operatorToUse = "<=";
        //                    }
        //                    else
        //                    {
        //                        splitChar = '=';
        //                        operatorToUse = "=";
        //                    }
        //                }
        //                else if (sumRule.Contains(">"))
        //                {
        //                    splitChar = '>';
        //                    operatorToUse = ">";
        //                }
        //                else if (sumRule.Contains("<"))
        //                {
        //                    splitChar = '<';
        //                    operatorToUse = "<";
        //                }


        //                string[] sumRuleSplit = sumRule.Split(splitChar);

        //                string leftGroup = sumRuleSplit[0];
        //                string rightGroup = sumRuleSplit[1];
        //                bool fullyFilledLeft = true;
        //                bool fullyFilledRight = true;

        //                leftGroup = leftGroup.Trim();
        //                leftGroup = leftGroup.Replace(" ", "");

        //                rightGroup = rightGroup.Trim();
        //                rightGroup = rightGroup.Replace(" ", "");

        //                rightGroup = rightGroup.Replace("=", "");

        //                decimal leftGroupSummation, rightGroupSummation;

        //                string[] leftGroupLabelIDs = leftGroup.Split('+');

        //                //leftGroupSummation = sumLabelIds(leftGroupLabelIDs, col, out fullyFilled, fullyFilledRequired);

        //                leftGroupSummation = sumLabelIds(leftGroupLabelIDs, valueObj, out fullyFilledLeft, fullyFilledRequired);

        //                string[] rightGroupLabelIDs = rightGroup.Split('+');

        //                //rightGroupSummation = sumLabelIds(rightGroupLabelIDs, col, out fullyFilled, fullyFilledRequired);

        //                rightGroupSummation = sumLabelIds(rightGroupLabelIDs, valueObj, out fullyFilledRight, fullyFilledRequired);


        //                valid = true;

        //                if ((fullyFilledRequired == true) && ((fullyFilledLeft == false) || (fullyFilledRight == false)))
        //                {
        //                    valid = true;
        //                }
        //                else if ((fullyFilledLeft == false) && (leftValidate == true))
        //                {
        //                    valid = true;
        //                }
        //                else if ((fullyFilledRight == false) && (rightValidate == true))
        //                {
        //                    valid = true;
        //                }
        //                else
        //                {


        //                    if (operatorToUse == "=")
        //                    {
        //                        if (leftGroupSummation == rightGroupSummation)
        //                        {
        //                            valid = true;
        //                        }
        //                        else
        //                        {
        //                            valid = false;
        //                        }
        //                    }
        //                    else if (operatorToUse == ">=")
        //                    {
        //                        if (leftGroupSummation >= rightGroupSummation)
        //                        {
        //                            valid = true;
        //                        }
        //                        else
        //                        {
        //                            valid = false;
        //                        }
        //                    }
        //                    else if (operatorToUse == ">")
        //                    {
        //                        if (leftGroupSummation > rightGroupSummation)
        //                        {
        //                            valid = true;
        //                        }
        //                        else
        //                        {
        //                            valid = false;
        //                        }
        //                    }
        //                    else if (operatorToUse == "<=")
        //                    {
        //                        if (leftGroupSummation <= rightGroupSummation)
        //                        {
        //                            valid = true;
        //                        }
        //                        else
        //                        {
        //                            valid = false;
        //                        }
        //                    }
        //                    else if (operatorToUse == "<")
        //                    {
        //                        if (leftGroupSummation < rightGroupSummation)
        //                        {
        //                            valid = true;
        //                        }
        //                        else
        //                        {
        //                            valid = false;
        //                        }
        //                    }
        //                }
        //            }

        //            if (valid == false)
        //            {
        //                if (ruleType.ToUpper() != "WARNING")
        //                {
        //                    validateMessage.LabelId = valueObj.LabelID;
        //                    validateMessage.Message = message;
        //                    validateMessage.Type = "ERROR";
        //                    validateMessage.Value = valueObj.Value;
        //                    return validateMessage;
        //                }
        //                else // it is a warning and still need to save the value
        //                {
        //                    validateMessage.LabelId = valueObj.LabelID;
        //                    validateMessage.Message = message;
        //                    validateMessage.Type = "WARNING";
        //                    validateMessage.Value = valueObj.Value;
        //                    return validateMessage;
        //                }                        
        //            }
        //            else if (valid == true)
        //            {
        //                validateMessage.LabelId = valueObj.LabelID;
        //                validateMessage.Message = message;
        //                validateMessage.Type = "PASS";
        //                validateMessage.Value = valueObj.Value;
        //                return validateMessage;
        //            }
                    
        //        }

        //    }
        //    validateMessage.LabelId = valueObj.LabelID;
        //    validateMessage.Message = "";
        //    validateMessage.Type = "PASS";
        //    validateMessage.Value = valueObj.Value;

        //    return validateMessage;
        //}
    }

    

}