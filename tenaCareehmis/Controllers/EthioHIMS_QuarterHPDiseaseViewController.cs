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

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class EthioHIMS_QuarterHPDiseaseViewController : ApiController
    {
        private eHMISEntities db = new eHMISEntities();

        // GET: api/EthioHIMS_QuarterHPDiseaseView
        public IQueryable<EthioHIMS_QuarterHPDiseaseView> GetEthioHIMS_QuarterHPDiseaseView()
        {
            return db.EthioHIMS_QuarterHPDiseaseView;
        }

        // GET: api/EthioHIMS_QuarterHPDiseaseView/5
        [ResponseType(typeof(EthioHIMS_QuarterHPDiseaseView))]
        public IHttpActionResult GetEthioHIMS_QuarterHPDiseaseView(int id)
        {
            EthioHIMS_QuarterHPDiseaseView ethioHIMS_QuarterHPDiseaseView = db.EthioHIMS_QuarterHPDiseaseView.Find(id);
            if (ethioHIMS_QuarterHPDiseaseView == null)
            {
                return NotFound();
            }

            return Ok(ethioHIMS_QuarterHPDiseaseView);
        }

        // PUT: api/EthioHIMS_QuarterHPDiseaseView/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEthioHIMS_QuarterHPDiseaseView(int id, EthioHIMS_QuarterHPDiseaseView ethioHIMS_QuarterHPDiseaseView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ethioHIMS_QuarterHPDiseaseView.ID)
            {
                return BadRequest();
            }

            db.Entry(ethioHIMS_QuarterHPDiseaseView).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EthioHIMS_QuarterHPDiseaseViewExists(id))
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

        // POST: api/EthioHIMS_QuarterHPDiseaseView
        [ResponseType(typeof(EthioHIMS_QuarterHPDiseaseView))]
        public IHttpActionResult PostEthioHIMS_QuarterHPDiseaseView(EthioHIMS_QuarterHPDiseaseView ethioHIMS_QuarterHPDiseaseView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EthioHIMS_QuarterHPDiseaseView.Add(ethioHIMS_QuarterHPDiseaseView);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ethioHIMS_QuarterHPDiseaseView.ID }, ethioHIMS_QuarterHPDiseaseView);
        }

        // DELETE: api/EthioHIMS_QuarterHPDiseaseView/5
        [ResponseType(typeof(EthioHIMS_QuarterHPDiseaseView))]
        public IHttpActionResult DeleteEthioHIMS_QuarterHPDiseaseView(int id)
        {
            EthioHIMS_QuarterHPDiseaseView ethioHIMS_QuarterHPDiseaseView = db.EthioHIMS_QuarterHPDiseaseView.Find(id);
            if (ethioHIMS_QuarterHPDiseaseView == null)
            {
                return NotFound();
            }

            db.EthioHIMS_QuarterHPDiseaseView.Remove(ethioHIMS_QuarterHPDiseaseView);
            db.SaveChanges();

            return Ok(ethioHIMS_QuarterHPDiseaseView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EthioHIMS_QuarterHPDiseaseViewExists(int id)
        {
            return db.EthioHIMS_QuarterHPDiseaseView.Count(e => e.ID == id) > 0;
        }
    }
}