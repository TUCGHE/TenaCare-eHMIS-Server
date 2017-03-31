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
using System.Web.Http;
using System.Web.Http.Description;
using eHMISWebApi.Models;
using System.Web.Http.Cors;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class EthEhmis_HMISDenominatorController : ApiController
    {
        private eHMISEntities db = new eHMISEntities();

        // GET: api/EthEhmis_HMISDenominator
        public IQueryable<EthEhmis_HMISDenominator> GetEthEhmis_HMISDenominator()
        {
            return db.EthEhmis_HMISDenominator;
        }
        public IQueryable<EthEhmis_HMISDenominator> GetEthEhmis_HMISDenominator(int facilityType)
        {
            if (facilityType==1)
            {
                return db.EthEhmis_HMISDenominator.Where(t=>t.Healthpost==true);
            }
            else if (facilityType == 2)
            {
                return db.EthEhmis_HMISDenominator.Where(t => t.HealthCenter == true);
            }
            else if (facilityType == 3)
            {
                return db.EthEhmis_HMISDenominator.Where(t => t.Hospital == true);
            }
            else if (facilityType == 4)
            {
                return db.EthEhmis_HMISDenominator.Where(t => t.WoredaHO == true);
            }
            else if (facilityType == 5)
            {
                return db.EthEhmis_HMISDenominator.Where(t => t.ZonalHD == true);
            }
            else if (facilityType == 6)
            {
                return db.EthEhmis_HMISDenominator.Where(t => t.RegionalHB == true);
            }
            else if (facilityType == 7)
            {
                return db.EthEhmis_HMISDenominator.Where(t => t.FMOH == true);
            }
            return db.EthEhmis_HMISDenominator;
        }
        // GET: api/EthEhmis_HMISDenominator/5
        //[ResponseType(typeof(EthEhmis_HMISDenominator))]
        //public IHttpActionResult GetEthEhmis_HMISDenominator(int id)
        //{
        //    EthEhmis_HMISDenominator ethEhmis_HMISDenominator = db.EthEhmis_HMISDenominator.Find(id);
        //    if (ethEhmis_HMISDenominator == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(ethEhmis_HMISDenominator);
        //}

        // PUT: api/EthEhmis_HMISDenominator/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEthEhmis_HMISDenominator(int id, EthEhmis_HMISDenominator ethEhmis_HMISDenominator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ethEhmis_HMISDenominator.DenominatorID)
            {
                return BadRequest();
            }

            db.Entry(ethEhmis_HMISDenominator).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EthEhmis_HMISDenominatorExists(id))
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

        // POST: api/EthEhmis_HMISDenominator
        [ResponseType(typeof(EthEhmis_HMISDenominator))]
        public IHttpActionResult PostEthEhmis_HMISDenominator(EthEhmis_HMISDenominator ethEhmis_HMISDenominator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EthEhmis_HMISDenominator.Add(ethEhmis_HMISDenominator);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EthEhmis_HMISDenominatorExists(ethEhmis_HMISDenominator.DenominatorID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = ethEhmis_HMISDenominator.DenominatorID }, ethEhmis_HMISDenominator);
        }

        // DELETE: api/EthEhmis_HMISDenominator/5
        [ResponseType(typeof(EthEhmis_HMISDenominator))]
        public IHttpActionResult DeleteEthEhmis_HMISDenominator(int id)
        {
            EthEhmis_HMISDenominator ethEhmis_HMISDenominator = db.EthEhmis_HMISDenominator.Find(id);
            if (ethEhmis_HMISDenominator == null)
            {
                return NotFound();
            }

            db.EthEhmis_HMISDenominator.Remove(ethEhmis_HMISDenominator);
            db.SaveChanges();

            return Ok(ethEhmis_HMISDenominator);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EthEhmis_HMISDenominatorExists(int id)
        {
            return db.EthEhmis_HMISDenominator.Count(e => e.DenominatorID == id) > 0;
        }
    }
}