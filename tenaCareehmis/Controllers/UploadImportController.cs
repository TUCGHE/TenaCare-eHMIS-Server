using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Text;
using System.IO;
using System.Data;
using Newtonsoft.Json.Linq;
using eHMISWebApi.Models;
using SqlManagement.Database;
using System.Data.SqlClient;
using System.Collections;
using OfficeOpenXml;
using System.Web;


namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UploadImportController : ApiController
    {
        eHMISEntities db = new eHMISEntities();
        StringBuilder sb = new StringBuilder();
        string Filename = null;
        int filesize = 0;
        // string root = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
        string root = "C:/temp";
     //   string root = HttpContext.Current.Server.MapPath("~/") + "App_Data\\Import";
        StreamWriter sql;
        DBConnHelper _helper = new DBConnHelper();
        public async Task<HttpResponseMessage> PostFile()
        {
           
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }


           
            var provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                // Holds the response body
               // Request.Content.LoadIntoBufferAsync().Wait();
                // Read the form data and return an async task.
                // var bytes = Request.Content.ReadAsMultipartAsync(provider);
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                //  System.IO.File.WriteAllBytes(@"d:\import\" + bytes.CreationOptions., bytes.Result);

                //  var task = Request.Content.ReadAsMultipartAsync(provider).
                //  ContinueWith<HttpResponseMessage>(o =>
                // {

                //  string file1 = provider.FileData.First().Headers.ContentDisposition.FileName;
                // this is the file name on the server where the file was saved 

                //  return new HttpResponseMessage()
                //   {
                //     Content = new StringContent("File uploaded.")
                //   };
                //  }
                //  );
                // This illustrates how to get the form data.
                //extract username from the request .
                var model = result.FormData["model"];
              //  var year = result.FormData["year"];
                dynamic data = JObject.Parse(model);
                string userName = data.FullName;


                //foreach (var key in provider.FormData.AllKeys)
                //{
                //    foreach (var val in provider.FormData.GetValues(key))
                //    {
                //        //sb.Append(string.Format("{0}: {1}\n", key, val));
                //    }
                //}

                // This illustrates how to get the file names for uploaded files.

                foreach (var file in provider.FileData)
                {
                    Filename = file.Headers.ContentDisposition.FileName;
                    Filename = Filename.Replace("\"", string.Empty).Trim();

                    //  filesize = file.LocalFileName.Length;

                }

                if (Filename.Substring(Filename.LastIndexOf('.') + 1).ToLower() != "zip" &&  Filename.Substring(Filename.LastIndexOf('.') + 1).ToLower() != "xlsx" && Filename.Substring(Filename.LastIndexOf('.') + 1).ToLower() != "xls")
                {
                    sb.Append(string.Format("Wrong file uploaded: " + Filename + ", Please choose correct eHMIS report file."));
                    if (Filename != null)
                        System.IO.File.Delete(root + "\\" + Filename);
                }
                else
                {
                    string shortfilename = Filename;
                    string fullfilename = root + "\\" + Filename;
                    //get file size in bytes.
                    FileInfo fileLengthInfo = new FileInfo(fullfilename);
                    eHMIS.HMIS.ReportViewing.AutomatedImport import = new eHMIS.HMIS.ReportViewing.AutomatedImport();
                    filesize = Convert.ToInt32(fileLengthInfo.Length);
                    sb.Append(string.Format("Imported file: {0} ({1} bytes)\n", Filename, filesize));

                    EthEHMIS_ImportedFiles importedFiles = new EthEHMIS_ImportedFiles();
                    importedFiles.User = userName;
                    importedFiles.FileName = Filename;
                    importedFiles.Date = DateTime.Now;
                    importedFiles.FileSize = filesize + " bytes";
                    if (Filename.Substring(Filename.LastIndexOf('.') + 1).ToLower() == "zip")
                    {
                      //  eHMIS.HMIS.ReportViewing.AutomatedImport import = new eHMIS.HMIS.ReportViewing.AutomatedImport();
                        import.importDirectory = root;
                        //start importing the zip file.
                        import.startImport(shortfilename, fullfilename);
                        //insert to EthEhmis_ImportedFiles information table
                        

                        db.Entry(importedFiles).State = System.Data.Entity.EntityState.Added;
                        db.SaveChanges();

                    }
                    else
                    {
                        //var year = result.FormData["year"];
                       // string cmdText = "select * from EthEHMIS_ImportedFiles where FileName = '" + shortfilename + "'";
                       // SqlCommand cmd = new SqlCommand(cmdText);
                       //System.Data.DataTable dt = _helper.GetDataSet(cmd).Tables[0];
                       // if (dt.Rows.Count > 0)
                       // {
                       //     sb.Clear();
                       //     sb.Append("File Already Imported.");
                       // }
                       // else
                       // {
                            
                            ImportExcelFile(fullfilename);
                            //insert to EthEhmis_ImportedFiles information table

                            db.Entry(importedFiles).State = System.Data.Entity.EntityState.Added;
                            db.SaveChanges();
                        //}
                       
                    }
                }

               

                return new HttpResponseMessage()
                {
                    Content = new StringContent(sb.ToString())
                };
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                sql.Close();
                foreach (var eve in e.EntityValidationErrors)
                {
                    string validationerror = eve.Entry.Entity.GetType().Name +  eve.Entry.State;
                }
                    if (Filename != null)
                    System.IO.File.Delete(root + "\\" + Filename);

                return new HttpResponseMessage()
                {

                    Content = new StringContent(HttpStatusCode.InternalServerError.ToString() + ". " + e.Message)
                };
                // return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        [EnableCors("*", "*", "*")]
        public IEnumerable<object> Get()
        {
            return db.EthEHMIS_ImportedFiles.OrderByDescending(s => s.Date);

        }
        public void ImportExcelFile(string fullFileName)
        {
            if (fullFileName.Contains("OPD"))
            {
                ImportOPDReportFromExcel(fullFileName);
            }
            else if (fullFileName.Contains("Service"))
            {
                ImportServiceReportFromExcel(fullFileName);
            }
            else if (fullFileName.Contains("IPD"))
            {
                ImportIPDReportFromExcel(fullFileName);
            }
        }
        private static IEnumerable<ReaderResult<v_EthEhmis_AllFacilityWithID>> GetHealthPosts(eHMISEntities dc)
        {
            return GetFacilities(dc, 7, 3);
        }

        private static IEnumerable<ReaderResult<v_EthEhmis_AllFacilityWithID>> GetHealthCenters(eHMISEntities dc)
        {
            return GetFacilities(dc, 7, 2);
        }

        private static IEnumerable<ReaderResult<v_EthEhmis_AllFacilityWithID>> GetHospitals(eHMISEntities dc)
        {
            return GetFacilities(dc, 7, 1);
        }

        private static IEnumerable<ReaderResult<v_EthEhmis_AllFacilityWithID>> GetFacilities(eHMISEntities dc, int regionId, int facilityType)
        {
            
            using (var command = dc.Database.Connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM v_EthEhmis_AllFacilityWithID WHERE RegionId = {regionId} AND FacilityTypeId = {facilityType}";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var loop = true;
                        while (loop)
                        {
                            v_EthEhmis_AllFacilityWithID f = reader.Map<v_EthEhmis_AllFacilityWithID>();
                            var res = new ReaderResult<v_EthEhmis_AllFacilityWithID>(f);
                            loop = reader.Read();
                            if (!loop)
                            {
                                //last record
                                res.IsLast = true;
                            }
                            yield return res;
                        }
                    }
                }

            }
        }

        private static string GenerateInsert(int LabelID, int DataEleClass, int FederalID, int RegionID, int ZoneID, int WoredaID, string LocationID, int WEEK, int MONTH, int QUARTER, int YEAR, decimal Value, int LEVEL, int FACILITTYPE, DateTime DateEntered, int Editable)
        {
            //            string insertSql = @"
            //INSERT INTO dbo.EthEhmis_HMISValue 
            //(LabelID,DataEleClass,FederalID,RegionID,ZoneID,WoredaID,LocationID,[Week],
            //[Month],[Quarter],[Year],[Value],[Level],FACILITTYPE,DateEntered,Editable)
            //VALUES ("
            //+ $"{LabelID},{DataEleClass},{FederalID},{RegionID},{ZoneID},{WoredaID},'{LocationID}',{WEEK},{MONTH},{QUARTER},{YEAR},{Value},{LEVEL},{FACILITTYPE},{DateEntered},{Editable})";

            string insertSql = $"{1},{LabelID},{DataEleClass},{FederalID},{RegionID},{ZoneID},{WoredaID},{LocationID},{WEEK},{MONTH},{QUARTER},{YEAR},{Value},{LEVEL},{FACILITTYPE},{DateEntered},{Editable}";

            return insertSql.Replace(Environment.NewLine, "");
        }

        private void ImportOPDReportFromExcel(string fullFileName)
        {
            EthEhmis_HMISValue hmisValue = new EthEhmis_HMISValue();
            string month = null, yearString = null;
            int year;

                string[] splitfullFileName = fullFileName.Split(' ');
            int length = splitfullFileName.Length - 1;
            yearString = splitfullFileName[splitfullFileName.Length - 1];
                year = int.Parse(yearString.Remove(yearString.IndexOf('.')));
               
                month = splitfullFileName[splitfullFileName.Length - 2];
          
            Hashtable labelID = new Hashtable();
            string cmdText = "Select * from EthEHMIS_SNNP_OPD_MatchLabelID";
            SqlCommand toExecute = new SqlCommand(cmdText);
            System.Data.DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            string sno, name, key;
            int M04, M514, M15, F04, F514, F15;
            string age04 = "0-4 years", age514 = "5-14 years", age15 = ">=15 years";
            string female = "Female", male = "Male";
            int labelid;
            foreach (DataRow row in dt.Rows)
            {
                if (row["M04"] != DBNull.Value)
                {
                    sno = row["SNO"].ToString();
                    name = row["Disease"].ToString();
                    M04 = int.Parse(row["M04"].ToString());
                    M514 = int.Parse(row["M514"].ToString());
                    M15 = int.Parse(row["M15"].ToString());
                    F04 = int.Parse(row["F04"].ToString());
                    F514 = int.Parse(row["F514"].ToString());
                    F15 = int.Parse(row["F15"].ToString());

                    string Label = row["M04"].ToString();
                    if (Label != "")
                    {
                        key = sno + "_" + name + "_"+ male + "_"+ age04;
                        labelID.Add(key, M04);
                        key = null;
                        key = sno + "_" + name + "_"+ male +"_"+ age514;
                        labelID.Add(key, M514);
                        key = null;
                        key = sno + "_" + name + "_"+ male +"_"+ age15;
                        labelID.Add(key, M15);
                        key = null;
                        key = sno + "_" + name + "_"+ female +"_"+ age04;
                        labelID.Add(key, F04);
                        key = null;
                        key = sno + "_" + name + "_"+ female +"_"+ age514;
                        labelID.Add(key, F514);
                        key = null;
                        key = sno + "_" + name + "_"+ female +"_"+ age15;
                        labelID.Add(key, F15);
                        key = null;
                    }
                }
            }
            try
            {
                sql = new StreamWriter(Path.Combine(root, "Import.txt"));
            }
            catch (Exception ex)
            {

            }

            string sheetName = null;
            try
            {
                // Disease OPD Aggregate Report For SNNP : RHB From Miazia 2008 to Nehasie 2008
                using (ExcelPackage ep = new ExcelPackage(new FileInfo(fullFileName)))
                {
                    sheetName = "Disease OPD Aggregate Report";
                    var ws = ep.Workbook.Worksheets[sheetName];
                    int cols = ws.Dimension.Columns;
                    int rows = ws.Dimension.Rows;
                    string date = Convert.ToString(DateTime.Now);
                    string conctName;
                    //  db.Database.Connection.Open();
                   
                        for (int i = 7; i <= rows; i++)
                        {
                            
                            var val = ws.Cells["C" + i].Value;
                        //var val = range.Cells[i, 19].Value;
                        if (val == null || val.ToString() == "NULL")
                                continue;

                            var serialNo = ws.Cells["A" + i].Value;
                        if (serialNo == null || serialNo.ToString() == "NULL")
                            continue;
                            sno = serialNo.ToString();
                        if (sno.StartsWith("0"))
                            sno = sno.Substring(1);
                   
                            name = ws.Cells["B" + i].Value.ToString();

                        string[] ageSex = new string[6];
                        ageSex[0] = "_Male_"+ ws.Cells["C" + 4].Value.ToString();
                        ageSex[1] = "_Male_" + ws.Cells["D" + 4].Value.ToString();
                        ageSex[2] = "_Male_" + ws.Cells["E" + 4].Value.ToString();
                        ageSex[3] = "_Female_" + ws.Cells["F" + 4].Value.ToString();
                        ageSex[4] = "_Female_" + ws.Cells["G" + 4].Value.ToString();
                        ageSex[5] = "_Female_" + ws.Cells["H" + 4].Value.ToString();

                        char[] excelcolumns = new char[6];
                        excelcolumns[0] = 'C';
                        excelcolumns[1] = 'D';
                        excelcolumns[2] = 'E';
                        excelcolumns[3] = 'F';
                        excelcolumns[4] = 'G';
                        excelcolumns[5] = 'H';

                        conctName = sno + "_" + name;
                        int index = 0;

                        try
                        {
                            foreach(string var in ageSex)
                            {
                                labelid = int.Parse(labelID[conctName + var].ToString());
                            if (labelid != 0)
                            {
                                long value = Convert.ToInt64(ws.Cells[excelcolumns[index].ToString() + i].Value);
                                hmisValue.LabelID = labelid;
                                hmisValue.DataEleClass = 8;
                                hmisValue.FederalID = 0;
                                hmisValue.RegionID = 7;
                                hmisValue.ZoneID = 709;
                                hmisValue.WoredaID = 70906;
                                hmisValue.LocationID = "7090600305";
                                hmisValue.Week = 0;
                                hmisValue.Month = Extensions.Month(month);
                                hmisValue.Quarter = 0;
                                hmisValue.Year = year;
                                hmisValue.Value = value;
                                hmisValue.Level = 0;
                                hmisValue.FACILITTYPE = 3;
                                hmisValue.DateEntered = Convert.ToDateTime(date);
                                hmisValue.Editable = false;
                                string stream = (GenerateInsert(hmisValue.LabelID, hmisValue.DataEleClass, hmisValue.FederalID, hmisValue.RegionID, hmisValue.ZoneID, hmisValue.WoredaID,
                               hmisValue.LocationID, hmisValue.Week, hmisValue.Month, hmisValue.Quarter, hmisValue.Year, hmisValue.Value, hmisValue.Level, hmisValue.FACILITTYPE, hmisValue.DateEntered, 0));
                                sql.WriteLine(stream);
                                sql.Flush();
                            }
                                index++;
                        }
                            }
                            catch (Exception ex)
                            {
                                string labelIdDoesntExsist = ex.ToString();
                            }
                        }
                    
                }
            }
               
            catch (Exception ex)
            {
                sql.Close();
            }

            sql.Close();
            string importFilePath = root + @"\Import.txt";
            string exceptionMessage = "";
            bool errorInsert = false;
            eHMIS.HMIS.ReportViewing.HMISImportandDownloadServerBased serverBasedDownload = new eHMIS.HMIS.ReportViewing.HMISImportandDownloadServerBased();
            serverBasedDownload.createTempTable();
            serverBasedDownload.BulkInsert(importFilePath, out exceptionMessage, out errorInsert, false);
            if (errorInsert == true)
            {
                string errorMsg = exceptionMessage + "\n" + "Import has failed, please contact System administrator ";

                //General.Util.UI.MyMessageDialogSmall.Show(msg);

            }
            // Delete previous data
                cmdText = "Delete from EthEhmis_HMISValue where RegionID = 7 and DataEleClass = 8 and Month = " + Extensions.Month(month) + " and Year = " + year;
                toExecute = new SqlCommand(cmdText);
                _helper.Execute(toExecute);
                serverBasedDownload.insertHmisValueTableSNNP();
        }
        private void ImportIPDReportFromExcel(string fullFileName)
        {
            EthEhmis_HMISValue hmisValue = new EthEhmis_HMISValue();
            string month = null, yearString = null;
            int year;

            string[] splitfullFileName = fullFileName.Split(' ');
            int length = splitfullFileName.Length - 1;
            yearString = splitfullFileName[splitfullFileName.Length - 1];
            year = int.Parse(yearString.Remove(yearString.IndexOf('.')));

            month = splitfullFileName[splitfullFileName.Length - 2];

            Hashtable labelID = new Hashtable();
            string cmdText = "Select * from EthEHMIS_SNNP_IPD_MatchLabelID";
            SqlCommand toExecute = new SqlCommand(cmdText);
            System.Data.DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            string sno, name, key;
            int M04, M514, M15, F04, F514, F15, MM04, MM514, MM15, MF04, MF514, MF15;
            string age04 = "0-4 years", age514 = "5-14 years", age15 = ">=15 years";
            string female = "Female", male = "Male";
            int labelid;
            foreach (DataRow row in dt.Rows)
            {
                if (row["M04"] != DBNull.Value)
                {
                    sno = row["SNO"].ToString();
                    name = row["Disease"].ToString();
                    M04 = int.Parse(row["M04"].ToString());
                    M514 = int.Parse(row["M514"].ToString());
                    M15 = int.Parse(row["M15"].ToString());
                    F04 = int.Parse(row["F04"].ToString());
                    F514 = int.Parse(row["F514"].ToString());
                    F15 = int.Parse(row["F15"].ToString());
                    MM04 = int.Parse(row["MM04"].ToString());
                    MM514 = int.Parse(row["MM514"].ToString());
                    MM15 = int.Parse(row["MM15"].ToString());
                    MF04 = int.Parse(row["MF04"].ToString());
                    MF514 = int.Parse(row["MF514"].ToString());
                    MF15 = int.Parse(row["MF15"].ToString());

                    string Label = row["M04"].ToString();
                    if (Label != "")
                    {
                        try
                        {
                            key = sno + "_" + name + "_" + male + "_" + age04;
                            labelID.Add(key, M04);
                            key = null;
                            key = sno + "_" + name + "_" + male + "_" + age514;
                            labelID.Add(key, M514);
                            key = null;
                            key = sno + "_" + name + "_" + male + "_" + age15;
                            labelID.Add(key, M15);
                            key = null;
                            key = sno + "_" + name + "_" + female + "_" + age04;
                            labelID.Add(key, F04);
                            key = null;
                            key = sno + "_" + name + "_" + female + "_" + age514;
                            labelID.Add(key, F514);
                            key = null;
                            key = sno + "_" + name + "_" + female + "_" + age15;
                            labelID.Add(key, F15);
                            key = null;
                            key = sno + "_" + name + "_" + male + "_M" + age04;
                            labelID.Add(key, MM04);
                            key = null;
                            key = sno + "_" + name + "_" + male + "_M" + age514;
                            labelID.Add(key, MM514);
                            key = null;
                            key = sno + "_" + name + "_" + male + "_M" + age15;
                            labelID.Add(key, MM15);
                            key = null;
                            key = sno + "_" + name + "_" + female + "_M" + age04;
                            labelID.Add(key, MF04);
                            key = null;
                            key = sno + "_" + name + "_" + female + "_M" + age514;
                            labelID.Add(key, MF514);
                            key = null;
                            key = sno + "_" + name + "_" + female + "_M" + age15;
                            labelID.Add(key, MF15);
                            key = null;
                        }
                        catch (Exception ex)
                        {

                        }
                     
                    }
                }
            }
            try
            {
                sql = new StreamWriter(Path.Combine(root, "Import.txt"));
            }
            catch (Exception ex)
            {

            }

            string sheetName = null;
            try
            {
                // Disease OPD Aggregate Report For SNNP : RHB From Miazia 2008 to Nehasie 2008
                using (ExcelPackage ep = new ExcelPackage(new FileInfo(fullFileName)))
                {
                    sheetName = "Disease IPD Aggregate Report";
                    var ws = ep.Workbook.Worksheets[sheetName];
                    int cols = ws.Dimension.Columns;
                    int rows = ws.Dimension.Rows;
                    string date = Convert.ToString(DateTime.Now);
                    string conctName;
                    string column;
                    //  db.Database.Connection.Open();

                    for (int i = 7; i <= rows; i++)
                    {

                        var val = ws.Cells["C" + i].Value;
                        //var val = range.Cells[i, 19].Value;
                        if (val == null || val.ToString() == "NULL")
                            continue;

                        var serialNo = ws.Cells["A" + i].Value;
                        if (serialNo == null || serialNo.ToString() == "NULL")
                            continue;
                        sno = serialNo.ToString();
                        //if (sno.StartsWith("0"))
                        //    sno = sno.Substring(1);

                        name = ws.Cells["B" + i].Value.ToString();

                        string[] ageSex = new string[12];
                        ageSex[0] = "_Male_" + ws.Cells["C" + 4].Value.ToString();
                        ageSex[1] = "_Male_" + ws.Cells["D" + 4].Value.ToString();
                        ageSex[2] = "_Male_" + ws.Cells["E" + 4].Value.ToString();
                        ageSex[3] = "_Female_" + ws.Cells["F" + 4].Value.ToString();
                        ageSex[4] = "_Female_" + ws.Cells["G" + 4].Value.ToString();
                        ageSex[5] = "_Female_" + ws.Cells["H" + 4].Value.ToString();
                        ageSex[6] = "_Male_M" + ws.Cells["I" + 4].Value.ToString();
                        ageSex[7] = "_Male_M" + ws.Cells["J" + 4].Value.ToString();
                        ageSex[8] = "_Male_M" + ws.Cells["K" + 4].Value.ToString();
                        ageSex[9] = "_Female_M" + ws.Cells["L" + 4].Value.ToString();
                        ageSex[10] = "_Female_M" + ws.Cells["M" + 4].Value.ToString();
                        ageSex[11] = "_Female_M" + ws.Cells["N" + 4].Value.ToString();

                        char[] excelcolumns = new char[12];
                        excelcolumns[0] = 'C';
                        excelcolumns[1] = 'D';
                        excelcolumns[2] = 'E';
                        excelcolumns[3] = 'F';
                        excelcolumns[4] = 'G';
                        excelcolumns[5] = 'H';
                        excelcolumns[6] = 'I';
                        excelcolumns[7] = 'J';
                        excelcolumns[8] = 'K';
                        excelcolumns[9] = 'L';
                        excelcolumns[10] = 'M';
                        excelcolumns[11] = 'N';

                        conctName = sno + "_" + name;
                        int index = 0;

                        try
                        {
                            foreach (string var in ageSex)
                            {
                                labelid = int.Parse(labelID[conctName + var].ToString());
                               
                                if (labelid != 0 && index <= 5)
                                {
                                    long value = Convert.ToInt64(ws.Cells[excelcolumns[index].ToString() + i].Value);
                                    hmisValue.LabelID = labelid;
                                    hmisValue.DataEleClass = 2;
                                    hmisValue.FederalID = 0;
                                    hmisValue.RegionID = 7;
                                    hmisValue.ZoneID = 0;
                                    hmisValue.WoredaID = 0;
                                    hmisValue.LocationID = "7";
                                    hmisValue.Week = 0;
                                    hmisValue.Month = Extensions.Month(month);
                                    hmisValue.Quarter = 0;
                                    hmisValue.Year = year;
                                    hmisValue.Value = value;
                                    hmisValue.Level = 0;
                                    hmisValue.FACILITTYPE = 10;
                                    hmisValue.DateEntered = Convert.ToDateTime(date);
                                    hmisValue.Editable = false;
                                    string stream = (GenerateInsert(hmisValue.LabelID, hmisValue.DataEleClass, hmisValue.FederalID, hmisValue.RegionID, hmisValue.ZoneID, hmisValue.WoredaID,
                                   hmisValue.LocationID, hmisValue.Week, hmisValue.Month, hmisValue.Quarter, hmisValue.Year, hmisValue.Value, hmisValue.Level, hmisValue.FACILITTYPE, hmisValue.DateEntered, 0));
                                    sql.WriteLine(stream);
                                    sql.Flush();
                                }
                               else if (labelid != 0 && index >= 6)
                                {
                                    long value = Convert.ToInt64(ws.Cells[excelcolumns[index].ToString() + i].Value);
                                    hmisValue.LabelID = labelid;
                                    hmisValue.DataEleClass = 3;
                                    hmisValue.FederalID = 0;
                                    hmisValue.RegionID = 7;
                                    hmisValue.ZoneID = 0;
                                    hmisValue.WoredaID = 0;
                                    hmisValue.LocationID = "7";
                                    hmisValue.Week = 0;
                                    hmisValue.Month = Extensions.Month(month);
                                    hmisValue.Quarter = 0;
                                    hmisValue.Year = year;
                                    hmisValue.Value = value;
                                    hmisValue.Level = 0;
                                    hmisValue.FACILITTYPE = 10;
                                    hmisValue.DateEntered = Convert.ToDateTime(date);
                                    hmisValue.Editable = false;
                                    string stream = (GenerateInsert(hmisValue.LabelID, hmisValue.DataEleClass, hmisValue.FederalID, hmisValue.RegionID, hmisValue.ZoneID, hmisValue.WoredaID,
                                   hmisValue.LocationID, hmisValue.Week, hmisValue.Month, hmisValue.Quarter, hmisValue.Year, hmisValue.Value, hmisValue.Level, hmisValue.FACILITTYPE, hmisValue.DateEntered, 0));
                                    sql.WriteLine(stream);
                                    sql.Flush();
                                }
                                index++;
                            }
                        }
                        catch (Exception ex)
                        {
                            string labelIdDoesntExsist = ex.ToString();
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                sql.Close();
            }

            sql.Close();
            string importFilePath = root + @"\Import.txt";
            string exceptionMessage = "";
            bool errorInsert = false;
            eHMIS.HMIS.ReportViewing.HMISImportandDownloadServerBased serverBasedDownload = new eHMIS.HMIS.ReportViewing.HMISImportandDownloadServerBased();
            serverBasedDownload.createTempTable();
            serverBasedDownload.BulkInsert(importFilePath, out exceptionMessage, out errorInsert, false);
            if (errorInsert == true)
            {
                string errorMsg = exceptionMessage + "\n" + "Import has failed, please contact System administrator ";

                //General.Util.UI.MyMessageDialogSmall.Show(msg);

            }
            // Delete previous data
            cmdText = "Delete from EthEhmis_HMISValue where RegionID = 7 and Month = " + Extensions.Month(month) + " and Year = " + year + " and DataEleClass = 2 or DataEleClass = 3";
            toExecute = new SqlCommand(cmdText);
            _helper.Execute(toExecute);
            serverBasedDownload.insertHmisValueTableSNNP();
        }
        private void ImportServiceReportFromExcel(string fullFileName)
        {
            EthEhmis_HMISValue hmisValue = new EthEhmis_HMISValue();
            int reportType = 0;
            string month = null;
            string quarter = null;
            int year;

            if (fullFileName.Contains("Annual"))
            {
                reportType = 2;
                year = Convert.ToInt16(fullFileName.Split(' ').Last().Remove(fullFileName.Split(' ').Last().IndexOf('.')));
                //year =               
            }
            else if (fullFileName.Contains("Quarter"))
            {
                string[] splitfullFileName = fullFileName.Split(',');
                string[] monthYear = splitfullFileName[1].Split(' ');
                reportType = 1;
                year = int.Parse(monthYear[1]);
                quarter = monthYear[4];
                quarter = quarter.Remove(quarter.IndexOf('.'));

            }
            else
            {
                string[] splitfullFileName = fullFileName.Split(',');
                string[] monthYear = splitfullFileName[1].Split(' ');
                reportType = 0;
                year = int.Parse(monthYear[1]);
                month = monthYear[3];
                month = month.Remove(month.IndexOf('.'));
            }

            Hashtable labelID = new Hashtable();
            string cmdText = "Select * from EthEHMIS_SNNPMatchLabelID";
            SqlCommand toExecute = new SqlCommand(cmdText);
            System.Data.DataTable dt = _helper.GetDataSet(toExecute).Tables[0];
            string sno, name;
            int labelid = 0;
            foreach (DataRow row in dt.Rows)
            {
                sno = row["SNO"].ToString();
                name = row["Name"].ToString();
                //if (Convert.ToInt16(row["Tulane_LabelID"]) > 0)
                //{
                string Label = row["Tulane_LabelID"].ToString();
                if (Label != "")
                {
                    if (Convert.ToInt16(row["ID"]) == 305 || Convert.ToInt16(row["ID"]) == 307 || Convert.ToInt16(row["ID"]) == 309 ||
                        Convert.ToInt16(row["ID"]) == 311 || Convert.ToInt16(row["ID"]) == 311 || Convert.ToInt16(row["ID"]) == 313 ||
                        Convert.ToInt16(row["ID"]) == 315 || Convert.ToInt16(row["ID"]) == 317 || Convert.ToInt16(row["ID"]) == 319 ||
                        Convert.ToInt16(row["ID"]) == 387 || Convert.ToInt16(row["ID"]) == 389 || Convert.ToInt16(row["ID"]) == 391 ||
                        Convert.ToInt16(row["ID"]) == 393)
                    {
                        labelid = Convert.ToInt16(row["Tulane_LabelID"]);
                        name = sno + "_" + name + "_2";
                        labelID.Add(name, labelid);
                    }
                    else
                    {
                        labelid = Convert.ToInt16(row["Tulane_LabelID"]);
                        name = sno + "_" + name;
                        labelID.Add(name, labelid);
                    }

                }

                //}
                //labelid = Convert.ToInt16(row["Tulane_LabelID"]);
                //name = sno + "_" + name;
                //labelID.Add(name, labelid);

            }

            //Application xlApp;
            //Workbook xlWorkBook;
            //Worksheet xlWorkSheet;
            //Range range;
            //string str;
            //int rCnt;
            //int cCnt;
            //int rows = 0;
            //int cols = 0;

            //xlApp = new Application();
            //xlWorkBook = xlApp.Workbooks.Open(fullFileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            //xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //range = xlWorkSheet.UsedRange;
            //rows = range.Rows.Count;
            //cols = range.Columns.Count;

            try
            {
                sql = new StreamWriter(Path.Combine(root, "Import.txt"));
            }
            catch (Exception ex)
            {

            }

            string sheetName = null;
            try
            {
                // Service Delivery Aggregate Report For SNNP : RHB From Hamle 2007 To Senie 2008	
                using (ExcelPackage ep = new ExcelPackage(new FileInfo(fullFileName)))
                {
                    //   ep.Workbook.Worksheets.Add("sheet2");
                    //     ExcelPackage excel = new ExcelPackage(new FileInfo(fullFileName));
                    //ExcelWorkbook wb = new ExcelWorkbook(fullFileName, ) ;

                    // string sheetName = "Cleaned";
                    if (reportType == 0 || reportType == 1)
                    { sheetName = "Service Delivery Aggregate R"; }
                    else if (reportType == 2)
                    { sheetName = "Service Delivery Annual Aggr"; }


                    var ws = ep.Workbook.Worksheets[sheetName];
                    int cols = ws.Dimension.Columns;
                    int rows = ws.Dimension.Rows;
                    string date = Convert.ToString(DateTime.Now);
                    string conctName;
                    //  db.Database.Connection.Open();
                    if (reportType == 0) // monthly
                    {
                        for (int i = 9; i <= rows; i++)
                        {
                            var val = ws.Cells["S" + i].Value;
                            //var val = range.Cells[i, 19].Value;
                            if (val == null || val.ToString() == "NULL")
                                continue;

                            var serialNo = ws.Cells["A" + i].Value;
                            if (serialNo == null || serialNo.ToString() == "NULL")
                                serialNo = "";
                            sno = serialNo.ToString();

                            name = ws.Cells["B" + i].Value.ToString();
                            if (i == 312 || i == 314 || i == 316 || i == 318 | i == 320 || i == 322 || i == 324 || i == 326 || i == 394 || i == 396 ||
                               i == 398 || i == 400)//this are rows that have the same sno and name that esists in the excel sheet
                            {
                                conctName = sno + "_" + name + "_2";
                            }
                            else
                            {
                                conctName = sno + "_" + name;
                            }
                            try
                            {
                                labelid = int.Parse(labelID[conctName].ToString());
                                if (labelid != 0)
                                {
                                    long value = Convert.ToInt64(ws.Cells["S" + i].Value);
                                    hmisValue.LabelID = labelid;
                                    hmisValue.DataEleClass = 6;
                                    hmisValue.FederalID = 0;
                                    hmisValue.RegionID = 7;
                                    hmisValue.ZoneID = 0;
                                    hmisValue.WoredaID = 0;
                                    hmisValue.LocationID = "7";
                                    hmisValue.Week = 0;
                                    hmisValue.Month = Extensions.Month(month);
                                    hmisValue.Quarter = 0;
                                    hmisValue.Year = year;
                                    hmisValue.Value = value;
                                    hmisValue.Level = 0;
                                    hmisValue.FACILITTYPE = 10;
                                    hmisValue.DateEntered = Convert.ToDateTime(date);
                                    hmisValue.Editable = false;
                                    string stream = (GenerateInsert(hmisValue.LabelID, hmisValue.DataEleClass, hmisValue.FederalID, hmisValue.RegionID, hmisValue.ZoneID, hmisValue.WoredaID,
                                   hmisValue.LocationID, hmisValue.Week, hmisValue.Month, hmisValue.Quarter, hmisValue.Year, hmisValue.Value, hmisValue.Level, hmisValue.FACILITTYPE, hmisValue.DateEntered, 0));
                                    sql.WriteLine(stream);
                                    sql.Flush();
                                }
                            }
                            catch (Exception ex)
                            {
                                string labelIdDoesntExsist = ex.ToString();
                            }
                        }
                    }


                    if (reportType == 1)//Quarterly
                    {
                        for (int i = 7; i <= rows; i++)
                        {
                            var val = ws.Cells["S" + i].Value;
                            // var val = range.Cells[i, 19].Value;
                            if (val == null || val.ToString() == "NULL")
                                continue;

                            var serialNo = ws.Cells["A" + i].Value;
                            if (serialNo == null || serialNo.ToString() == "NULL")
                                serialNo = "";
                            sno = serialNo.ToString();

                            name = ws.Cells["B" + i].Value.ToString();
                            conctName = sno + "_" + name;
                            try
                            {
                                labelid = int.Parse(labelID[conctName].ToString());

                                if (labelid != 0)
                                {
                                    long value = Convert.ToInt64(ws.Cells["S" + i].Value);
                                    hmisValue.LabelID = labelid;
                                    hmisValue.DataEleClass = 6;
                                    hmisValue.FederalID = 0;
                                    hmisValue.RegionID = 7;
                                    hmisValue.ZoneID = 0;
                                    hmisValue.WoredaID = 0;
                                    hmisValue.LocationID = "7";
                                    hmisValue.Week = 0;
                                    hmisValue.Month = Extensions.MonthFromQuarter(Convert.ToInt16(quarter));
                                    hmisValue.Quarter = Convert.ToInt16(quarter);
                                    hmisValue.Year = year;
                                    hmisValue.Value = value;
                                    hmisValue.Level = 0;
                                    hmisValue.FACILITTYPE = 10;
                                    hmisValue.DateEntered = Convert.ToDateTime(date);
                                    hmisValue.Editable = false;
                                    sql.WriteLine(GenerateInsert(hmisValue.LabelID, hmisValue.DataEleClass, hmisValue.FederalID, hmisValue.RegionID, hmisValue.ZoneID, hmisValue.WoredaID,
                                   hmisValue.LocationID, hmisValue.Week, hmisValue.Month, hmisValue.Quarter, hmisValue.Year, hmisValue.Value, hmisValue.Level, hmisValue.FACILITTYPE, hmisValue.DateEntered, 0));
                                    sql.Flush();
                                }

                            }
                            catch (Exception ex)
                            {
                                string labelIdDoesntExsist = ex.ToString();
                            }

                        }
                    }
                    if (reportType == 2)// Annual
                    {
                        for (int i = 10; i <= rows; i++)
                        {
                            var val = ws.Cells["S" + i].Value;
                            // var val = range.Cells[i , 19].Value;
                            if (val == null || val.ToString() == "NULL")
                                continue;

                            var serialNo = ws.Cells["A" + i].Value;
                            if (serialNo == null || serialNo.ToString() == "NULL")
                                serialNo = "";
                            sno = serialNo.ToString();
                            name = ws.Cells["B" + i].Value.ToString();
                            conctName = sno + "_" + name;
                            try
                            {

                                labelid = int.Parse(labelID[conctName].ToString());
                                if (labelid != 0)
                                {
                                    long value = Convert.ToInt64(ws.Cells["S" + i].Value);
                                    hmisValue.LabelID = labelid;
                                    hmisValue.DataEleClass = 7;
                                    hmisValue.FederalID = 0;
                                    hmisValue.RegionID = 7;
                                    hmisValue.ZoneID = 0;
                                    hmisValue.WoredaID = 0;
                                    hmisValue.LocationID = "7";
                                    hmisValue.Week = 0;
                                    hmisValue.Month = 0;
                                    hmisValue.Quarter = 0;
                                    hmisValue.Year = year;
                                    hmisValue.Value = value;
                                    hmisValue.Level = 0;
                                    hmisValue.FACILITTYPE = 10;
                                    hmisValue.DateEntered = Convert.ToDateTime(date);
                                    hmisValue.Editable = false;
                                    sql.WriteLine(GenerateInsert(hmisValue.LabelID, hmisValue.DataEleClass, hmisValue.FederalID, hmisValue.RegionID, hmisValue.ZoneID, hmisValue.WoredaID,
                                   hmisValue.LocationID, hmisValue.Week, hmisValue.Month, hmisValue.Quarter, hmisValue.Year, hmisValue.Value, hmisValue.Level, hmisValue.FACILITTYPE, hmisValue.DateEntered, 0));
                                }
                            }
                            catch (Exception ex)
                            {
                                string labelIdDoesntExsist = ex.ToString();
                            }


                        }
                    }



                    // db.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                sql.Close();
            }

            sql.Close();
            string importFilePath = root + @"\Import.txt";
            string exceptionMessage = "";
            bool errorInsert = false;
            eHMIS.HMIS.ReportViewing.HMISImportandDownloadServerBased serverBasedDownload = new eHMIS.HMIS.ReportViewing.HMISImportandDownloadServerBased();
            serverBasedDownload.createTempTable();
            serverBasedDownload.BulkInsert(importFilePath, out exceptionMessage, out errorInsert, false);
            if (errorInsert == true)
            {
                string errorMsg = exceptionMessage + "\n" + "Import has failed, please contact System administrator ";

                //General.Util.UI.MyMessageDialogSmall.Show(msg);

            }
            // Delete previous data
            if (reportType == 2)
            {
                cmdText = "Delete from EthEhmis_HMISValue where RegionID = 7 and DataEleClass = 7 and  Year = " + year + " and Month = 0 and Quarter = 0";
                toExecute = new SqlCommand(cmdText);
                _helper.Execute(toExecute);
            }
            else if (reportType == 0)
            {
                cmdText = "Delete from EthEhmis_HMISValue where RegionID = 7 and DataEleClass = 6 and Month = " + Extensions.Month(month) + " and Year = " + year;
                toExecute = new SqlCommand(cmdText);
                _helper.Execute(toExecute);

            }
            if (reportType == 1)
            {
                cmdText = "Delete from EthEhmis_HMISValue where RegionID = 7 and DataEleClass = 6 and Quarter = " + quarter + " and Year = " + year;
                toExecute = new SqlCommand(cmdText);
                _helper.Execute(toExecute);
            }

            serverBasedDownload.insertHmisValueTableSNNP();
        }
      
   
    }
}
