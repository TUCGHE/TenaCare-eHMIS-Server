using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace eHMISWebApi.DataEngine
{
    public class Utilities
    {
        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        internal static String EthiopianMonthName(int m)
        {
            switch (m)
            {
                case 0: return "Maskaram";
                case 1: return "Təqəmt";
                case 2: return "Hədar";
                case 3: return "Tahsas";
                case 4: return "Tərr";
                case 5: return "Yakatit";
                case 6: return "Magabit";
                case 7: return "Miyazya";
                case 8: return "Gənbot";
                case 9: return "Sane";
                case 10: return "Hamle";
                case 11: return "Nahase";
                case 12: return "Pagwəmen";
                default:
                    return "Unknown";
            }
        }

        internal static Dictionary<int, string[]> getReportsDictionaryForPeriod(string period, int locationLevel = 0)
        {
            Dictionary<int, String[]> map = new Dictionary<int, string[]>();

            switch (period.ToUpper())
            {
                case "WEEKLY":
                    {
                        map.Add(0, new String[] { "Weekly PHEM Report for Outpatient and Inpatient Cases and Deaths", "Case Based Report", "AFP", "NNT", "Laboratory" });
                        break;
                    }
                case "QUARTERLY":
                    {
                        //0-health center, 1-hospital , 2-health post, 3-Woreda HO
                        // 4-zone HD, 5-region HB, 6-FMoH
                        if (locationLevel > 0)
                        {
                            map.Add(8, new String[]{"Woreda Health Office Quarterly Inpatient (IPD) Disease Report","Woreda Health Office Quarterly Outpatient (OPD) Disease Report","Woreda Health Office Quarterly Service Delivery Report"
                            ,"Weekly" });

                            map.Add(9, new String[]{"ZHD Quarterly Inpatient (IPD) Disease Report","ZHD Quarterly Outpatient (OPD) Disease Report"
                            ,"ZHD Quarterly Service Delivery Report", "Weekly"});

                            map.Add(10, new String[]{"RHB Quarterly Inpatient (IPD) Disease Report","RHB Quarterly Outpatient (OPD) Disease Report"
                            ,"RHB Quarterly Service Delivery Report"
                         ,
                                "Weekly" });
                            map.Add(11, new String[]{ "FMoH Quarterly Inpatient (IPD) Disease Report","FMoH Quarterly Outpatient (OPD) Disease Report"
                            ,"FMoH Quarterly Service Delivery Report","Weekly"});
                        }
                        else
                        {
                            map.Add(2, new String[]{"Health Center or Clinic or Hospital Quarterly Inpatient (IPD) Disease Report"
                            ,"Health Center or Clinic or Hospital Quarterly Outpatient (OPD) Disease Report"
                            ,"Health Center or Clinic or Hospital Quarterly Service Delivery Report"
                            ,"Health Center Quarterly Aggregated Service Delivery Report"
                            ,"Health Center Quarterly Aggregated OPD Report"
                            ,"Weekly" });

                            int[] grpOne = { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 7, 6, 5, 4, 1 };
                            Array.ForEach(grpOne, i => map.Add(i, new String[] {
                   "Health Center or Clinic or Hospital Quarterly Inpatient (IPD) Disease Report"
                            ,"Health Center or Clinic or Hospital Quarterly Outpatient (OPD) Disease Report"
                            ,"Health Center or Clinic or Hospital Quarterly Service Delivery Report"
                            ,"Weekly"
                    }));

                            int[] grpTwo = { 3, 0 };
                            Array.ForEach(grpTwo, i => map.Add(i, new String[] {
                   "Health Post Quarterly Disease Report"
                            ,"Health Post Quarterly Service Delivery Report"
                    }));
                        }

                        break;
                    }
                case "ANNUAL":
                    {
                        if (locationLevel > 0)
                        {
                            int[] grpTwo = { 0, 1, 2 };
                            Array.ForEach(grpTwo, i => map.Add(i, new String[] { "Health Center or Clinic or Hospital Annual Service Delivery Report" }));

                            map.Add(3, new String[] { "Health Post Annual Service Delivery Report" });
                            map.Add(8, new String[] { "Woreda Health Office Annual Service Delivery Report" });
                            map.Add(9, new String[] { "ZHD Annual Service Delivery Report" });
                            map.Add(10, new String[] { "RHB Annual Service Delivery Report" });
                            map.Add(11, new String[] { "FMoH Annual Service Delivery Report" });
                        }
                        else
                        {
                            int[] grpOne = { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 7, 6, 5, 4, 2 };
                            Array.ForEach(grpOne, i => map.Add(i, new String[] {
                   "Health Center or Clinic or Hospital Annual Service Delivery Report"
                    }));

                            int[] grpTwo = { 0, 3 };
                            Array.ForEach(grpTwo, i => map.Add(i, new String[] { "Health Post Annual Service Delivery Report" }));
                        }

                        break;
                    }
                case "MONTHLY":
                    {
                        if (locationLevel > 0)
                        {
                            map.Add(8, new String[]{"Woreda Health Office Monthly Inpatient (IPD) Disease Report"
                            ,"Woreda Health Office Monthly Outpatient (OPD) Disease Report"
                            ,"Woreda Health Office Monthly Service Delivery Report"
                            ,"Weekly" });

                            map.Add(9, new String[]{"ZHD Monthly Inpatient (IPD) Disease Report"
                            ,"ZHD Monthly Outpatient (OPD) Disease Report"
                            ,"ZHD Monthly Service Delivery Report"
                            ,"Weekly" });

                            map.Add(10, new String[]{"RHB Monthly Inpatient (IPD) Disease Report",
                            "RHB Monthly Outpatient (OPD) Disease Report",
                            "RHB Monthly Service Delivery Report",
                            "Weekly" });

                            map.Add(11, new String[]{"FMoH Monthly Inpatient (IPD) Disease Report"
                            ,"FMoH Monthly Outpatient (OPD) Disease Report"
                            ,"FMoH Monthly Service Delivery Report"
                            ,"Weekly" });
                        }
                        else
                        {
                            int[] grpOne = { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 7, 6, 5, 4, 1 };
                            Array.ForEach(grpOne, i => map.Add(i, new String[] {
                  "Health Center or Clinic or Hospital Monthly Inpatient (IPD) Disease Report"
                            ,"Health Center or Clinic or Hospital Monthly Outpatient (OPD) Disease Report"
                            ,"Health Center or Clinic or Hospital Monthly Service Delivery Report"
                            ,"Weekly"
                    }));

                            map.Add(2, new String[]{"Health Center or Clinic or Hospital Monthly Inpatient (IPD) Disease Report",
                            "Health Center or Clinic or Hospital Monthly Outpatient (OPD) Disease Report",
                            "Health Center or Clinic or Hospital Monthly Service Delivery Report",
                            "Health Center Monthly Aggregated Service Delivery Report",
                            "Health Center Monthly Aggregated OPD Report",
                            "Weekly" });

                            int[] grpTwo = { 0, 3 };
                            Array.ForEach(grpTwo, i => map.Add(i, new String[] { "Health Post Monthly Disease Report", "Health Post Monthly Service Delivery Report" }));
                        }

                        break;
                    }
            }

            return map;
        }
    }
}