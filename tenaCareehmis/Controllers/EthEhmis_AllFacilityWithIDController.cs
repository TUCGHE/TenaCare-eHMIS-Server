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
using SqlManagement.Database;
using System.Data.SqlClient;

namespace eHMISWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EthEhmis_AllFacilityWithIDController : ApiController
    {
        private eHMISEntities db = new eHMISEntities();

        // GET: api/EthEhmis_AllFacilityWithID
        public IQueryable<EthEhmis_AllFacilityWithID> GetEthEhmis_AllFacilityWithID()
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_AllFacilityWithID;
        }

        public IQueryable<EthEhmis_AllFacilityWithID> GetByRegion(int regionid)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_AllFacilityWithID.Distinct().Where(x => x.RegionId == regionid);
        }

        public IQueryable<EthEhmis_AllFacilityWithID> GetByZone(int zoneid)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_AllFacilityWithID.Where(x => x.ZoneId == zoneid);
        }

        //public IQueryable<EthEhmis_AllFacilityWithID> GetWoredas(int woredaid)
        //{
        //    return db.EthEhmis_AllFacilityWithID.Distinct().Where(x => x.WoredaId == woredaid);
        //}


        public IQueryable<EthEhmis_AllFacilityWithID> GetByRegionByType(int regionid, int ftype)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_AllFacilityWithID.Distinct().Where(x => x.RegionId == regionid && x.FacilityTypeId == ftype);
        }

        public IQueryable<EthEhmis_AllFacilityWithID> GetByZoneByType(int zoneid, int ftype)
        {
            db.Database.CommandTimeout = Int32.MaxValue;
            return db.EthEhmis_AllFacilityWithID.Where(x => x.ZoneId == zoneid && x.FacilityTypeId == ftype);
        }

        public IQueryable<EthEhmis_AllFacilityWithID> GetWoredasByType(int woredaid, int ftype)
        {
            return db.EthEhmis_AllFacilityWithID.Distinct().Where(x => x.WoredaId == woredaid && x.FacilityTypeId == ftype);
        }

        public DataTable GetFacilitiesByWoreda(int woredaid)
        {
            //return db.EthEhmis_AllFacilityWithID.Distinct().Where(x => x.WoredaId == woredaid && x.FacilityTypeId!=8);
            DBConnHelper _helper = new DBConnHelper();
            string cmdText = "SELECT *  FROM [EthEhmis_AllFacilityWithID] where FacilityTypeId <> 8 and  woredaid =" + woredaid;

            SqlCommand getFacilityCmd = new SqlCommand(cmdText);
            DataTable dt = _helper.GetDataSet(getFacilityCmd).Tables[0];

            return dt;
        }

        // GET: api/EthEhmis_AllFacilityWithID/5
        //[ResponseType(typeof(EthEhmis_AllFacilityWithID))]
        //public IHttpActionResult GetEthEhmis_AllFacilityWithID(int id)
        //{
        //    EthEhmis_AllFacilityWithID EthEhmis_AllFacilityWithID = db.EthEhmis_AllFacilityWithID.Find(id);
        //    if (EthEhmis_AllFacilityWithID == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(EthEhmis_AllFacilityWithID);
        //}

        // PUT: api/EthEhmis_AllFacilityWithID/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEthEhmis_AllFacilityWithID(int id, EthEhmis_AllFacilityWithID EthEhmis_AllFacilityWithID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != EthEhmis_AllFacilityWithID.DistrictId)
            {
                return BadRequest();
            }

            db.Entry(EthEhmis_AllFacilityWithID).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EthEhmis_AllFacilityWithIDExists(id))
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

        //// POST: api/EthEhmis_AllFacilityWithID
        //[ResponseType(typeof(EthEhmis_AllFacilityWithID))]
        //public IHttpActionResult PostEthEhmis_AllFacilityWithID(EthEhmis_AllFacilityWithID EthEhmis_AllFacilityWithID)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.EthEhmis_AllFacilityWithID.Add(EthEhmis_AllFacilityWithID);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (EthEhmis_AllFacilityWithIDExists(EthEhmis_AllFacilityWithID.DistrictId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = EthEhmis_AllFacilityWithID.DistrictId }, EthEhmis_AllFacilityWithID);
        //}

        // DELETE: api/EthEhmis_AllFacilityWithID/5
        [ResponseType(typeof(EthEhmis_AllFacilityWithID))]
        public IHttpActionResult DeleteEthEhmis_AllFacilityWithID(int id)
        {
            EthEhmis_AllFacilityWithID EthEhmis_AllFacilityWithID = db.EthEhmis_AllFacilityWithID.Find(id);
            if (EthEhmis_AllFacilityWithID == null)
            {
                return NotFound();
            }

            db.EthEhmis_AllFacilityWithID.Remove(EthEhmis_AllFacilityWithID);
            db.SaveChanges();

            return Ok(EthEhmis_AllFacilityWithID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EthEhmis_AllFacilityWithIDExists(int id)
        {
            return db.EthEhmis_AllFacilityWithID.Count(e => e.DistrictId == id) > 0;
        }
    }
}