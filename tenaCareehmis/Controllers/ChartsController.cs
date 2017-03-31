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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using eHMISWebApi.Models;
using System.IO;
using System.Data.SqlClient;
using SqlManagement.Database;
using System.Data;

namespace eHMISWebApi.Controllers
{
    public static class MyExtensions
    {
        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("-yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
                );
        }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Charts")]
    public class ChartsController : ApiController
    {
        

        [Route("CreateDashboard")]
        [AcceptVerbs("Save")]
        public HttpResponseMessage Save()
        {

            int iNumberOfUploads = 0;

            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
            string sPath = "";
            Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath("~/ChartImageCache/"));
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ChartImageCache/");

            System.Web.HttpFileCollection requestFiles = System.Web.HttpContext.Current.Request.Files;

            // CHECK THE FILE COUNT.
            for (int i = 0; i <= requestFiles.Count - 1; i++)
            {
                System.Web.HttpPostedFile hostedFile = requestFiles[i];

                if (hostedFile.ContentLength > 0)
                {
                    // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                    if (!File.Exists(sPath + Path.GetFileName(hostedFile.FileName)))
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        hostedFile.SaveAs(sPath + Path.GetFileName("Chart.png".AppendTimeStamp()));
                        iNumberOfUploads = iNumberOfUploads + 1;
                    }
                }
            }

            // RETURN A MESSAGE (OPTIONAL).
            if (iNumberOfUploads > 0)
            {
                return this.Request.CreateResponse<String>(
               HttpStatusCode.OK, "Chart Sucessfully Saved");
            }
            else
            {
                return this.Request.CreateResponse<String>(
               HttpStatusCode.BadRequest, "Upload of Chart Failed");
            }

        }


        [Route("CreateDashboard")]
        [AcceptVerbs("Post")]
        public HttpResponseMessage CreateDashboard(UserDashboard userDashboard)
        {
            eHMISEntities db = new eHMISEntities();

            if (userDashboard != null)
            {
                if (userDashboard.UserId == null || userDashboard.DataSQL == null || userDashboard.DashboardSpec == null)
                {
                    return this.Request.CreateResponse<String>(
                    HttpStatusCode.Forbidden, "Please Specify the Parameters");
                }

                Models.UserDashboard dash = new Models.UserDashboard();
                dash.Id = String.Format("{0}-{1}",userDashboard.UserId , DateTime.Now.ToString());
                dash.dashboardSpec = userDashboard.DashboardSpec;
                dash.dataSQL = userDashboard.DataSQL;
                dash.UserId = userDashboard.UserId;
                dash.title = userDashboard.Title != null ? userDashboard.Title : String.Format("Dashboard-{0}", DateTime.Now.ToString());

                db.UserDashboards.Add(dash);                
                db.SaveChanges();

                return this.Request.CreateResponse<String>(
                HttpStatusCode.OK, "Dashboard has been saved");
            }
            else
                return this.Request.CreateResponse<String>(
                    HttpStatusCode.Forbidden, "Please Specify the Parameters");

        }

        [Route("GetDashboards")]
        [AcceptVerbs("Get")]
        public HttpResponseMessage GetDashboards([FromUriAttribute] String  UserId)
        {
            eHMISEntities db = new eHMISEntities();

            if (UserId != null)
            {

                var dashboards = db.UserDashboards.SqlQuery("SELECT * FROM dbo.UserDashboards WHERE UserId = @userId", new SqlParameter("userId", UserId));
                
            
                return this.Request.CreateResponse(
                HttpStatusCode.OK, dashboards);
            }
            else
                return this.Request.CreateResponse<String>(
                    HttpStatusCode.Forbidden, "Please Specify the UserId parameter!");

        }

        public class UserDashboard
        {
            public String UserId;
            public String DashboardSpec;
            public String DataSQL;
            public String Title;
        }

        public class DashboardSQL
        {
            public String DataSQL;
        }

        [Route("GetDashboardData")]
        [AcceptVerbs("Post")]
        public HttpResponseMessage GetDashboardData(DashboardSQL param)
        {

            if ((param != null) && (param.DataSQL != null))
            {
                DataTable reportDataTable = new DataTable();

                string parameterSql = param.DataSQL;

                DBConnHelper _helper = new DBConnHelper();
                SqlCommand parameterCmd = new SqlCommand(parameterSql);
                reportDataTable = _helper.GetDataSet(parameterCmd).Tables[0];

                return this.Request.CreateResponse(
               HttpStatusCode.OK, reportDataTable);
            }
            else
            {
                return this.Request.CreateResponse<String>(
                   HttpStatusCode.Forbidden, "Please Specify the required parameters!");
            }
        }
    }
}