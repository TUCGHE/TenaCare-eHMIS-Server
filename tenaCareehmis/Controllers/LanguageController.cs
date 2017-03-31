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
using System.Data.SqlClient;
using System.Data;
using SqlManagement.Database;
using System.Web.Http.Cors;

namespace eHMISWebApi.Controllers
{
    public class LanguageController : ApiController
    {
        public static string ipdViewTable = string.Empty;
        public static string opdViewTable = string.Empty;
        public static string serviceViewTable = string.Empty;
        public static string facilityTable = string.Empty;
        public static string diseaseDictionaryTable = string.Empty;
        public static string languageSet = string.Empty;
        enum reportTypeIndex
        {
            Service = 0,
            OPD_Disease = 1,
            IPD_Disease = 2,
            Indicators = 3,
            DiseaseAnalysis = 4,
        }

        public static void setCorrectLanguageTable()
        {
            ipdViewTable = "EthioHIMS_QuarterIPDDiseaseView";
            opdViewTable = "EthioHIMS_QuarterOPDDiseaseView4";
            serviceViewTable = "EthioHIMS_ServiceDataElementsNew";
            facilityTable = "EthEhmis_AllFacilityWithId";
            diseaseDictionaryTable = "DiseaseDictionary";

        //DBConnHelper _helper = new DBConnHelper();
        //    string cmdText = " select language from languageSetting where languageSet = 1";
        //    SqlCommand languageCmd = new SqlCommand(cmdText);

        //    DataTable dt = _helper.GetDataSet(languageCmd).Tables[0];

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        languageSet = row["language"].ToString();
        //    }

            if (languageSet != "english")
            {
                ipdViewTable = ipdViewTable + languageSet;
                opdViewTable = opdViewTable + languageSet;
                serviceViewTable = serviceViewTable + languageSet;
                facilityTable = facilityTable + languageSet;
                diseaseDictionaryTable = diseaseDictionaryTable + languageSet;
            }
        }

        [EnableCors("*", "*", "*")]
        // GET: api/Language
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Language/5
        public string Get(int id)
        {
            return "value";
        }

        [EnableCors("*", "*", "*")]
        public DataTable Get(string langName, string className)
        {
            if ((langName == null) || (langName == "") || (langName == "null"))
            {
                langName = "english";
            }

            languageSet = langName;
            string[] classes = className.Split(',');

            string classNames = string.Empty;
            int count = 1;
            classNames = "(";
            foreach (string cls in classes)
            {
                if (count != classes.Length)
                {
                    classNames += "'" + cls + "',";
                    count++;
                }
                else
                {
                    classNames += "'" + cls + "'";
                }
            }

            classNames += ")";

            string cmdLanguage = " select indexName, LanguageName from " +
                                 " languageLabelSetting where classname in " + classNames +
                                 "  and language = @languageSet ";

            DBConnHelper _helper = new DBConnHelper();
            SqlCommand categoryCmd = new SqlCommand(cmdLanguage);
            //languageSet = Get("", 1);
            //categoryCmd.Parameters.AddWithValue("className", className);
            categoryCmd.Parameters.AddWithValue("languageSet", languageSet);

            DataTable dt = _helper.GetDataSet(categoryCmd).Tables[0];
                     
            return dt;
        }

        [EnableCors("*", "*", "*")]
        public string Get(string languageSet, int id)
        {
            DBConnHelper _helper = new DBConnHelper();

            if (id == 0) // set the language
            {
                string cmdLanguage1 = "update languageSetting set LanguageSet = 0";

                SqlCommand categoryCmd = new SqlCommand(cmdLanguage1);

                _helper.Execute(categoryCmd);

                string cmdLanguage2 = " update languageSetting set LanguageSet = 1 where Language = @languageSet ";
                categoryCmd = new SqlCommand(cmdLanguage2);

                categoryCmd.Parameters.AddWithValue("languageSet", languageSet);
                _helper.Execute(categoryCmd);
            }
            else if (id == 1) // get the set language
            {
                string cmdText = " select language from languageSetting where languageSet = 1";
                SqlCommand languageCmd = new SqlCommand(cmdText);

                DataTable dt = _helper.GetDataSet(languageCmd).Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    languageSet = row["language"].ToString();
                }
            }
                        
            return languageSet;
        }

        // POST: api/Language
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Language/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Language/5
        public void Delete(int id)
        {
        }
    }
}
