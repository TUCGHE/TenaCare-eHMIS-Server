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
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using eHMISWebApi.Models;
using System.Web.Http.Cors;
using System.Collections;
using System.Data.SqlClient;
using SqlManagement.Database;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class EthioHIMS_QuarterOPDDiseaseView4Controller : ApiController
    {
        private eHMISEntities db = new eHMISEntities();
        string languageSet = LanguageController.languageSet;
        string opdTable = string.Empty;

        //[EnableCors("*", "*", "*")]
        //// GET: api/EthioHIMS_QuarterOPDDiseaseView4
        //public IQueryable GetEthioHIMS_QuarterOPDDiseaseView4([FromUri] int facilityType)
        //{
        //    if(facilityType == 3)
        //    {
        //        return db.EthioHIMS_QuarterHPDiseaseView;
        //    }
        //    else
        //    {
        //        return db.EthioHIMS_QuarterOPDDiseaseView4;
        //    }

        //}

        private void setCorrectLanguageTable()
        {
            //if (facilityType == 3)
            //{
            //    opdTable  = "EthioHIMS_QuarterHPDiseaseView";
            //}
            //else
            //{
            //    opdTable = "EthioHIMS_QuarterOPDDiseaseView4"; 
            //}

            opdTable = "EthioHIMS_QuarterOPDDiseaseView4";
            languageSet = LanguageController.languageSet;

            //DBConnHelper _helper = new DBConnHelper();
            //string cmdText = " select language from languageSetting where languageSet = 1";
            //SqlCommand languageCmd = new SqlCommand(cmdText);

            //DataTable dt = _helper.GetDataSet(languageCmd).Tables[0];

            //foreach (DataRow row in dt.Rows)
            //{
            //    languageSet = row["language"].ToString();
            //}

            if (languageSet != "english")
            {
                opdTable = opdTable + languageSet;
            }
        }

        [EnableCors("*", "*", "*")]
        // GET: api/EthioHIMS_QuarterOPDDiseaseView4
        public DataTable GetEthioHIMS_QuarterOPDDiseaseView4([FromUri] int facilityType)
        {
            setCorrectLanguageTable();

            DBConnHelper _helper = new DBConnHelper();
            DataTable dt = new DataTable();
            string diseaseListText = string.Empty;

            diseaseListText = " select * from  " + opdTable;

            SqlCommand diseaseListCommand = new SqlCommand(diseaseListText);
            dt = _helper.GetDataSet(diseaseListCommand).Tables[0];

            return dt;
        }
        //[EnableCors("*", "*", "*")]
        //// GET: api/EthioHIMS_QuarterOPDDiseaseView4/5
        //[ResponseType(typeof(EthioHIMS_QuarterOPDDiseaseView4))]
        //public IHttpActionResult GetEthioHIMS_QuarterOPDDiseaseView4(int id)
        //{
        //    EthioHIMS_QuarterOPDDiseaseView4 ethioHIMS_QuarterOPDDiseaseView4 = db.EthioHIMS_QuarterOPDDiseaseView4.Find(id);
        //    if (ethioHIMS_QuarterOPDDiseaseView4 == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(ethioHIMS_QuarterOPDDiseaseView4);
        //}
        [EnableCors("*", "*", "*")]
        // PUT: api/EthioHIMS_QuarterOPDDiseaseView4/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEthioHIMS_QuarterOPDDiseaseView4(int id, EthioHIMS_QuarterOPDDiseaseView4 ethioHIMS_QuarterOPDDiseaseView4)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ethioHIMS_QuarterOPDDiseaseView4.ID)
            {
                return BadRequest();
            }

            db.Entry(ethioHIMS_QuarterOPDDiseaseView4).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EthioHIMS_QuarterOPDDiseaseView4Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EthioHIMS_QuarterOPDDiseaseView4
        //[ResponseType(typeof(EthioHIMS_QuarterOPDDiseaseView4))]
        //public IHttpActionResult PostEthioHIMS_QuarterOPDDiseaseView4(EthioHIMS_QuarterOPDDiseaseView4 ethioHIMS_QuarterOPDDiseaseView4)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.EthioHIMS_QuarterOPDDiseaseView4.Add(ethioHIMS_QuarterOPDDiseaseView4);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = ethioHIMS_QuarterOPDDiseaseView4.ID }, ethioHIMS_QuarterOPDDiseaseView4);
        //}
        
        [EnableCors("*", "*", "*")]
        // DELETE: api/EthioHIMS_QuarterOPDDiseaseView4/5
        [ResponseType(typeof(EthioHIMS_QuarterOPDDiseaseView4))]
        public IHttpActionResult DeleteEthioHIMS_QuarterOPDDiseaseView4(int id)
        {
            EthioHIMS_QuarterOPDDiseaseView4 ethioHIMS_QuarterOPDDiseaseView4 = db.EthioHIMS_QuarterOPDDiseaseView4.Find(id);
            if (ethioHIMS_QuarterOPDDiseaseView4 == null)
            {
                return NotFound();
            }

            db.EthioHIMS_QuarterOPDDiseaseView4.Remove(ethioHIMS_QuarterOPDDiseaseView4);
            db.SaveChanges();

            return Ok(ethioHIMS_QuarterOPDDiseaseView4);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EthioHIMS_QuarterOPDDiseaseView4Exists(int id)
        {
            return db.EthioHIMS_QuarterOPDDiseaseView4.Count(e => e.ID == id) > 0;
        }

        private void combineDictionaryValues()
        {

        }


    }
}