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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using System.Collections;
using System.Runtime.Serialization;
using System.Reflection;

namespace UtilitiesNew.GeneralUtilities
{
	/// <summary>
	/// A class to hold information about the locale of this application.
	/// This class has methods to determine which modules should be loaded, 
	/// which Reports should be configured, etc.
	/// </summary>
	public class LocaleConfigurationProvider
	{		

		#region Member Variables
		
        private Hashtable _resourceHashtable = new Hashtable();
        private Hashtable _versionsTable = new Hashtable();

	    private Dictionary<string, string> _currentLocaleStrings;
	    private Dictionary<string, string[]> _currentLocaleEnums;
        

        private string _imagePath;
        private string _localeDatabaseConfigDir;
        private string _filepathForInfoFile;
        private string _resourceFilePath;
        private string _secondLocaleDatabaseConfigDir;	   


	    public const string ETHIOPIA = "Ethiopia";
        public const string ZNBTS = "Znbts";
	    public const string ZAMBIA = "Zambia";
	    public const string SOUTHAFRICA = "SouthAfrica";

        public const string TRANSPORT_FILENAME = "TRANSPORT_FILENAME";
        public const string SERVICE_CATEGORIES = "SERVICE_CATEGORIES";

        //	private ArrayList _hierarchicalFacilityItems = null;
		#endregion

		#region Properties
		public static String CurrentLocaleName
		{
			get 
			{
                return "Ethiopia";
			}
		}


	    #endregion		

		               
       
        #region PublicMethods

        /// <summary>
        /// Gets the date time format for the locale, which can be overridden in the locale's configuration file.
        /// This defaults to the machine's format.
        /// </summary>
        /// <returns></returns>
        public string GetShortDateTimeFormat()
        {
            //if (_localeConfiguration != null && !string.IsNullOrEmpty(_localeConfiguration.DateTimeFormat))
            //{
            //    return _localeConfiguration.DateTimeFormat;
            //}
            //else
            //{
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            //}
        }

        public static string GetLongDateDisplayString(DateTime date)
        {
            if (CurrentLocaleName == ETHIOPIA)
            {
                General.Util.Ethiopia.Day ethDay =
                    General.Util.Ethiopia.Calendar.DateTimeToEthiopian(date);

                string datepart = String.Format("E.C. {0}/{1}/{2} ", ethDay.getDate(), ethDay.getMonth(), ethDay.getYear());
                return datepart + " " + date.ToString("hh : mm tt ");
            }
            else
            {
                return date.DayOfWeek.ToString() + ", "
                              + date.ToString("d MMMM yyyy") + " "
                              + date.ToString("hh : mm tt ");
            }
        }

        public static string GetDateDisplayString(DateTime date)
        {
            if (string.Compare(CurrentLocaleName, ETHIOPIA, true) == 0)
            {
                General.Util.Ethiopia.Day ethDay =
                        General.Util.Ethiopia.Calendar.DateTimeToEthiopian(date);

                //dmyung 04/18/2007 :: Initial testing - see if custom formatter for ethiopian dates will work in the grid.
                //specifying format for MMDDYYYY will come later.
                //    return String.Format("E.C. {0}/{1}/{2}", ethDay.getMonth(), ethDay.getDate(), ethDay.getYear());
                return String.Format("E.C. {0}/{1}/{2}", ethDay.getDate(), ethDay.getMonth(), ethDay.getYear());
            }
            else
            {
                return date.ToShortDateString();
            }

        }
        public string GetDateDisplayStringForEthio(DateTime date)
        {
            General.Util.Ethiopia.Day ethDay =
                    General.Util.Ethiopia.Calendar.DateTimeToEthiopian(date);
            return String.Format("E.C. {0}/{1}/{2}", ethDay.getDate(), ethDay.getMonth(), ethDay.getYear());
        }
        
        /// <summary>
        /// Same behavior as GetDateDisplayString except using YY instead of YYYY, and checks against
        /// new DateTime() and Constants.NULLDATE and returns an empty string if the date matches either
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetDateDisplayStringShortWithEmptyChecks(DateTime date)
        {
            if (new DateTime().Equals(date) || Constants.NULLDATE.Equals(date)) {
                return "";
            }
            if (string.Compare(CurrentLocaleName, ETHIOPIA, true) == 0)
            {
                General.Util.Ethiopia.Day ethDay =
                        General.Util.Ethiopia.Calendar.DateTimeToEthiopian(date);

                //dmyung 04/18/2007 :: Initial testing - see if custom formatter for ethiopian dates will work in the grid.
                //specifying format for MMDDYYYY will come later.
                //    return String.Format("E.C. {0}/{1}/{2}", ethDay.getMonth(), ethDay.getDate(), ethDay.getYear());
                return
                    String.Format("E.C. {0}/{1}/{2}", ethDay.getDate(), ethDay.getMonth(),
                                  ethDay.getYear().ToString().Substring(2));
            }
            else
            {
                string dateString = date.ToShortDateString();
                string[] dmy = dateString.Split('/');
                string day = dmy[0];
                // pad days and months
                if (day.Length == 1)
                {
                    day = "0" + day;
                }
                string month = dmy[1];
                if (month.Length == 1) {
                    month = "0" + month;
                }
                string year = dmy[2];
                year = year.Substring(2);
                return day + "/" + month + "/" + year;
                //return dateString.Substring(0, dateString.Length - 4) + dateString.Substring(dateString.Length - 2);
            }
        }



	    // 2008-04-22 @CZUE:  removed this call.  either let the control parse the date or use the one above
        //public EthiopianDate GetDateDisplayStringDayMonthYear(DateTime date)
        //{
        //    EthiopianDate EthDate = new EthiopianDate();
        //    if (string.Compare(_localeConfiguration.Name, ETHIOPIA, true) == 0)
        //    {
        //        General.Util.Ethiopia.Day ethDay =
        //                General.Util.Ethiopia.Calendar.DateTimeToEthiopian(date);

        //        //dmyung 04/18/2007 :: Initial testing - see if custom formatter for ethiopian dates will work in the grid.
        //        //specifying format for MMDDYYYY will come later.
        //        //    return String.Format("E.C. {0}/{1}/{2}", ethDay.getMonth(), ethDay.getDate(), ethDay.getYear());
                      

        //        EthDate.EthDay = ethDay.getDate();
        //        EthDate.EthMonth = ethDay.getMonth();
        //        EthDate.EthYear = ethDay.getYear();

        //        return EthDate;
        //    }
        //    else
        //    {
        //        return EthDate;
        //    }

        //}

        #endregion

		#region Private Methods
              	               							       
        #endregion
	   
	}
}

