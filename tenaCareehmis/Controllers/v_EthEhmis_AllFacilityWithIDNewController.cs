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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class v_EthEhmis_AllFacilityWithIDNewController : ApiController
    {
        private eHMISEntities db = new eHMISEntities();

        // GET: api/v_EthEhmis_AllFacilityWithIDNew
        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> Getv_EthEhmis_AllFacilityWithIDNew()
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.v_EthEhmis_AllFacilityWithIDNew;
        }

        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> GetByRegion(int regionid)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.v_EthEhmis_AllFacilityWithIDNew.Distinct().Where(x => x.RegionId == regionid);
        }

        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> GetByZone(int zoneid)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.v_EthEhmis_AllFacilityWithIDNew.Where(x => x.ZoneId == zoneid);
        }

        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> GetWoredas(int woredaid)
        {
            return db.v_EthEhmis_AllFacilityWithIDNew.Distinct().Where(x => x.WoredaId == woredaid);
        }


        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> GetByRegionByType(int regionid,int ftype)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.v_EthEhmis_AllFacilityWithIDNew.Distinct().Where(x => x.RegionId == regionid && x.FacilityTypeId == ftype);
        }

        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> GetByZoneByType(int zoneid, int ftype)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.v_EthEhmis_AllFacilityWithIDNew.Where(x => x.ZoneId == zoneid && x.FacilityTypeId == ftype);
        }

        public IQueryable<v_EthEhmis_AllFacilityWithIDNew> GetWoredasByType(int woredaid, int ftype)
        {
            return db.v_EthEhmis_AllFacilityWithIDNew.Distinct().Where(x => x.WoredaId == woredaid && x.FacilityTypeId == ftype);
        }


        // GET: api/v_EthEhmis_AllFacilityWithIDNew/5
        //[ResponseType(typeof(v_EthEhmis_AllFacilityWithIDNew))]
        //public IHttpActionResult Getv_EthEhmis_AllFacilityWithIDNew(int id)
        //{
        //    v_EthEhmis_AllFacilityWithIDNew v_EthEhmis_AllFacilityWithIDNew = db.v_EthEhmis_AllFacilityWithIDNew.Find(id);
        //    if (v_EthEhmis_AllFacilityWithIDNew == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(v_EthEhmis_AllFacilityWithIDNew);
        //}

        // PUT: api/v_EthEhmis_AllFacilityWithIDNew/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putv_EthEhmis_AllFacilityWithIDNew(int id, v_EthEhmis_AllFacilityWithIDNew v_EthEhmis_AllFacilityWithIDNew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != v_EthEhmis_AllFacilityWithIDNew.DistrictId)
            {
                return BadRequest();
            }

            db.Entry(v_EthEhmis_AllFacilityWithIDNew).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!v_EthEhmis_AllFacilityWithIDNewExists(id))
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

        // POST: api/v_EthEhmis_AllFacilityWithIDNew
        [ResponseType(typeof(v_EthEhmis_AllFacilityWithIDNew))]
        public IHttpActionResult Postv_EthEhmis_AllFacilityWithIDNew(v_EthEhmis_AllFacilityWithIDNew v_EthEhmis_AllFacilityWithIDNew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.v_EthEhmis_AllFacilityWithIDNew.Add(v_EthEhmis_AllFacilityWithIDNew);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (v_EthEhmis_AllFacilityWithIDNewExists(v_EthEhmis_AllFacilityWithIDNew.DistrictId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = v_EthEhmis_AllFacilityWithIDNew.DistrictId }, v_EthEhmis_AllFacilityWithIDNew);
        }

        // DELETE: api/v_EthEhmis_AllFacilityWithIDNew/5
        [ResponseType(typeof(v_EthEhmis_AllFacilityWithIDNew))]
        public IHttpActionResult Deletev_EthEhmis_AllFacilityWithIDNew(int id)
        {
            v_EthEhmis_AllFacilityWithIDNew v_EthEhmis_AllFacilityWithIDNew = db.v_EthEhmis_AllFacilityWithIDNew.Find(id);
            if (v_EthEhmis_AllFacilityWithIDNew == null)
            {
                return NotFound();
            }

            db.v_EthEhmis_AllFacilityWithIDNew.Remove(v_EthEhmis_AllFacilityWithIDNew);
            db.SaveChanges();

            return Ok(v_EthEhmis_AllFacilityWithIDNew);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool v_EthEhmis_AllFacilityWithIDNewExists(int id)
        {
            return db.v_EthEhmis_AllFacilityWithIDNew.Count(e => e.DistrictId == id) > 0;
        }
    }
}