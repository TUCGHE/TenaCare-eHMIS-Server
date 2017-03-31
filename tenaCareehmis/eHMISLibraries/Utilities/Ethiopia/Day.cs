/*
 * @(#)Day.java        1.0 10/12/2005
 * 
 * Copyright (c) 2005 Senamirmir Project. All Rights Reserved.
 *
 * This file is part of Software: Ealet 2.0
 *
 * Ealet 2.0 is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * Ealet 2.0 is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 */

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
    using System.Text;
/**
 * Keeps all possible attributes of a date including name, date, month, year, 
 * holidays, and others.
 *
 * @version    1.0; October 12, 2005
 * @author abass alamnehe
 */
namespace General.Util.Ethiopia
{
    public class Day
    {

        #region Ethiopian Variables
        /* Variables for Ethiopian day */
        private String _yearName = "";

        private int _day = 0;
        private int _date = 0;
        private int _month = 0;
        private int _year = 0;

        /* Ethiopian week days starting from Sunday in integer form */
        private int[] _weekDays = { 5, 6, 0, 1, 2, 3, 4 };  // reorders Tintyon              
        #endregion         

        #region Gregorian Variables
        /* Variables for Gregorian day */
        private String _gregorianDay = "";
        private int _gregorianDate = 0;
        private int _gregorianMonth = 0;
        private int _gregorianYear = 0;
        #endregion


        #region Constructors
        /** 
         * Empty constructor
         */
        public Day() { }
        /**
         * Instance with Ethiopian day attributes
         */
        public Day(String yearName, int day, int month, int date, int year)
        {
            this._yearName = yearName;
            this._day = day;
            this._date = date;
            this._month = month;
            this._year = year;
        }

        /** 
         * Instance with both Ethiopian and Gregorian day attributes
         */
        public Day(String yearName, int day, int month, int date, int year,
            String gregorianDay, int gregorianMonth, int gregorianDate,
                                                     int gregorianYear)
        {
            this._yearName = yearName;
            this._day = day;
            this._date = date;
            this._month = month;
            this._year = year;
            this._gregorianDay = gregorianDay;
            this._gregorianDate = gregorianDate;
            this._gregorianMonth = gregorianMonth;
            this._gregorianYear = gregorianYear;
        }

        #endregion


        /**
         * Returns Tintyon, which is an integer form of the day of a week
         * @since          2.0
         */
        private int[] getWeekDays()
        {
            return _weekDays;
        }

        /**
         * Sets the name variable to the saint name year
         * @param yearName  Ethiopian calendar years are assigned saint names
         * @since           2.0
         */
        internal void setYearName(String yearName)
        {
            this._yearName = yearName;
        }
        /**
         * Returns yearName Ethiopian calendar years are assigned saint names
         * @since           2.0
         */
        public String getYearName() { return _yearName; }
        /**
         * Sets the day variable
         * @param day       Ethiopian calendar day
         * @since           2.0
         */
        public void setDay(int day)
        {
            this._day = day;
        }
       
        /// <summary>
        /// Get Ethiopian Calendar Day of week
        /// </summary>
        /// <returns>int 0-6 day of week</returns>
        public int getDay() { return _day; }
        /**
         * Sets the month variable
         * @param month     Ethiopian calendar month
         * @since           2.0
         */
        public void setMonth(int month)
        {
            this._month = month;
        }
        
        /// <summary>
        /// Get the Ethiopian Calendar Month
        /// </summary>
        /// <returns>Integer value 1-13</returns>
        public int getMonth()
        {
            return _month;
        }
        /**
         * Sets the date variable
         * @param date      Ethiopian calendar date
         * @since           2.0
         */
        public void setDate(int date)
        {
            this._date = date;
        }
        
        /// <summary>
        /// Get Ethiopian Calendar Date (Day of month)
        /// </summary>
        /// <returns>integer day of Ethiopian month 1-30</returns>
        public int getDate()
        {
            return _date;
        }
        /**
         * Sets the year variable
         * @param year      Ethiopian calendar year
         * @since           2.0
         */
        public void setYear(int year)
        {
            this._year = year;
        }
       
        /// <summary>
        /// Return Ethiopian Calendar Year
        /// </summary>
        /// <returns>integer year</returns>
        public int getYear()
        {
            return _year;
        }

        public string GetDisplayString()
        {
            return String.Format( "EC. {0}/{1}/{2}", this.getDate(), this.getMonth(), this.getYear() );
        }

        public static string GetDisplayString(DateTime dt)
        {
            Day theDay =  General.Util.Ethiopia.Calendar.DateTimeToEthiopian(dt);
            return String.Format("EC. {0}/{1}/{2}", theDay.getDate(), theDay.getMonth(), theDay.getYear());
        }


        #region Gregorian Getters/Setters

        /**
         * Sets gregorian day
         * @param gregorianDay 
         * @since           2.0
         */
        public void setGregorianDay(String gregorianDay)
        {
            this._gregorianDay = gregorianDay;
        }
        
        /// <summary>
        /// Returns the Gregorian day of week
        /// </summary>
        /// <returns>integer day of week 0-6</returns>
        public String getGregorianDay()
        {
            return _gregorianDay;
        }

        /**
         * Sets gregorian month
         * @param gregorianMonth 
         * @since           2.0
         */
        public void setGregorianMonth(int gregorianMonth)
        {
            this._gregorianMonth = gregorianMonth;
        }
       
        /// <summary>
        /// Get the Gregorian month
        /// </summary>
        /// <returns>integer month 1-12</returns>
        public int getGregorianMonth()
        {
            return _gregorianMonth;
        }
        /**
         * Sets gregorian date
         * @param gregorianDate 
         * @since           2.0
         */
        public void setGregorianDate(int gregorianDate)
        {
            this._gregorianDate = gregorianDate;
        }
       
        /// <summary>
        /// Get the Gregorian day of month
        /// </summary>
        /// <returns>integer day of month 0-30/31</returns>
        public int getGregorianDate()
        {
            return _gregorianDate;
        }
        /**
         * Sets gregorian year
         * @param gregorianYear 
         * @since           2.0
         */
        public void setGregorianYear(int gregorianYear)
        {
            this._gregorianYear = gregorianYear;
        }
        
        /// <summary>
        /// Get the Gregorian Year
        /// </summary>
        /// <returns>Integer year value</returns>
        public int getGregorianYear()
        {
            return _gregorianYear;
        }

        public DateTime getGregorianDateTime()
        {
            try
            {
                //dmyung 11/10/2007 :: added a catch statement as this was causing a hard exception when the day object was made with wrong inputs prior to this call.
                DateTime ret = new DateTime(this.getGregorianYear(), this.getGregorianMonth(), this.getGregorianDate());
                return ret;
            } catch
            {
                return General.Util.Constants.NULLDATE;        
            }
        }
        #endregion
    }
}