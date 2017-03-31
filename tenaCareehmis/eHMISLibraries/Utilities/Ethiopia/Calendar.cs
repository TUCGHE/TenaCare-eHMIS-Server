/*
 * @(#)Calendar.java        1.0 10/12/2005
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
 * Ealet is distributed in the hope that it will be useful,
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace General.Util.Ethiopia
{
    /**
     * History
     * 09/05/2006 
     *    Solved the leap year error in method whenIsEthNewYearInGregSeptember(int year) 
     *    changed the line
     *    from:  newYearDate = ( year-1 % 4 == 3 ? ++newYearDate : newYearDate ) ;      
     *    to  :  newYearDate = ( ((year-1) % 4) == 3 ? ++newYearDate : newYearDate ) ; 
     *    
     */

    /**
     * Ethiopian calendar generator including monthly, annual, and moving holidays.
     * It is limited to CE (Common Era); that is, from 00. The calendar calculation,
     * religious and national holidays system is based on Dr. Getatchew Haile's book
     * "Bahra Hassab" published in 2000.
     * <p>
     *
     * This class can be used for these specific tasks.
     * <ul>
     *    <li>
     *       To generate annual Ethioian calendar<br><br>
     *       <pre>
     *          Calendar cal = new Calendar(year) ;
     *       </pre>
     *    </li>
     *    <li>
     *       To convert from  Ethiopian date to Gregorian <br><br>
     *       <pre>
     *          Day day = Calendar.toGregorian(month, date, year) ;
     *       </pre>
     *    </li>
     *    <li>
     *       To convert from Gregorian date to Ethiopian<br><br>
     *       <pre>
     *          Day day = Calendar.toEthiopian(month, date, year) ;
     *       </pre>
     *    </li>
     * </ul>
     *
     * Each calendar is generated and organized in a set of linked-list data structure.
     * The following crude figure shows how that structure looks like.
     * <pre>
     *             ---------   ---------   ---------
     *    Year     |  Sep  |---|  Oct  |---|  Nov  |--- ...
     *             ---------   ---------   ---------
     *                 +           +           +
     *                 |           |           |
     *                 |           |           |
     *             ---------   ---------   ---------   -------------
     *    Day     |  Mon   |   |  Tue  |   |  Thu  |---|  holiday  |....
     *             ---------   ---------   ---------   -------------
     *                 .           .           .
     *                 .           .           .
     *                 .           .           .
     * </pre>
     *
     * Some important terms:
     * <ul>
     *    <li>
     *       <tt><b>abekitee</b> refers to the difference or overlapping days between Lunar and
     *       Solar calendar. Lunar's calendar year is shorter than Solar's by 11 days and this 
     *       number increases every year. The Ethiopian Orthodox Church calculates its some of 
     *       fasting holidays based on lunar calendar year. In the process, when the 
     *       difference reaches over the 30 days, it is modulated by 30 and the result is used.</tt>
     *    </li>
     *    <li>
     *       <tt><b>metqee</b> is the number of days it takes for Lunar month to complete
     *       when it is overlapped with the beginning month of new year. <b>metiqee</b>
     *       is calculate by subtracting abekitte from 30. If the result is equal or less than
     *       to 8, metqee is said to occur in October; otherwise, in September</tt>
     *    </li>
     *    <li>
     *       <tt><b>Tintyon</b> is a system by which one can determine the first day of each
     *       month if one is known. For instance, if the 1st day of September is
     *       Monday, April's 1st day will occur Monday and January's on Tuesday.</tt>
     *    </li>
     *    <li>
     *       <tt><b>CE (Common Era)</b> is used in place of <b>AD</b> (anno Domini)</tt>
     *    </li>
     *    <li>
     *       <tt><b>BCE (Before Common Era)</b> is used in place of <b>BC</b> (before Christ)</tt>
     *    </li>
     * </ul>
     *
     * @version 1.0   October 12, 2005
     * @author        abass alamnehe
     */
    public class Calendar
    {
        #region Member Variables
        /* Linked list is used to organize the month and year of the calendar */
        //private LinkedList[] calendarYear = new LinkedList[13] ;
        private LinkedList<Day>[] calendarYear = new LinkedList<Day>[13];

        /* class variables and constant variables */
        private const int bce = 5500;     // before common era (BCE)
        private int _ce = 0;                // commen era (CE)

        private int date = 0;
        private int month = 0;
        private int year = 0;
        #endregion

        #region Constructor
        /** 
         * Constructor: A linked-list for months is constructed.    *
         *
         * @param year the year to which a calendar will be built 
         */
        public Calendar(int year)
        {
            /* assign the Ethiopian year with user value */
            this._ce = year;

            /* initialize calendarYear with 13 linked list objects for months */
            for (int i = 0; i < calendarYear.Length; i++)
                calendarYear[i] = new LinkedList<Day>();
        }

        #endregion

        #region Instance methods for calendar manipulation

        private void ClearCalendar()
        {
            for (int i = 0; i < calendarYear.Length; i++)
            {
                calendarYear[i].Clear();
            }
        }

        /** 
         * Returns the metqee for the given year
         *
         * @param ce  a year for which metqee is generated
         * @return    metiqee is equal to 30 - abekitee
         * @since     2.0
         */
        public int getMetqee(int ce)
        {
            int era = bce + ce;
            era -= 1;
            int wember = era % 19;

            int abikete = (wember * 11) % 30;
            int metqee = 30 - abikete;
            return metqee;
        }

        /** 
         * Returns the Ethiopian new year day which will be used as starting day for
         * the given year
         *
         * @param ce  A year for which metqee is generated
         * @return    The name of the first day in String form
         * @since     2.0
         */
        public String getNewYearDay(int ce)
        {
            /* tintyon is used to determine what the day is on new year */
            String[] tintyon = { "Tue", "Wed", "Thu", "Fri", "Sat", "Sun", "Mon" };

            /* number of years until the give year  */
            int era = bce + ce;
            era -= 1;

            /* determines the number of leap year until */
            int tmp = 0;
            for (int i = 0; i <= era; i++)
            {
                if (i % 4 == 3) tmp++;
            }
            /* calculates the days upto the given year */
            int daysSince = (era * 365) + tmp;
            /* returns the day of ethiopian new year */
            return tintyon[daysSince % 7];
        }



        /**
         * It prepares monthly holidays with integer array. It fetches each
         * Day instance and adds the monthly holidays to its list. 
         * These holidays will be mapped to proper names by the XMLer class during 
         * transformation. For that purpose, the XMLer class utilizes a properties
         * file.
         * <p> 
         * The properties file is "ealet_holiday_properties.xml" and has the following
         * form.
         * <p>
         * The key and values ares
         * <pre>
         *     1         abo
         *     ^          ^
         *    key       value
         *     ^          ^
         *     |          |
         *     |           ----- the name of the holiday
         *     | 
         *      ----- the date of the month to the holiday belongs
         * </pre>
         * @since      2.0
         */

        /**
         * The method prepares annual holidays with integer array. It fetches each
         * Day instance from the calendar and inserts these holidays.
         * The month and date where each annual holiday
         * occurs is used in combination as a key in the holiday properties 
         * file and used by the XMLer class during transformation to retrieve
         * the holiday names.
         * <p> 
         * The properties file is "ealet_holiday_properties.xml" and has the following
         * field and value content.
         * <p>
         * The key and values ares
         * <pre>
         *    1-1     Enkutatash
         *     ^          ^
         *    key       value
         *     ^          ^
         *     |          |
         *     |           ----- the name of the holiday
         *     | 
         *      ----- the 1st is month while the 2nd is date
         * </pre>
         * @since      2.0
         */


        /** 
         * Returns the day of a given Gregorian date
         *
         * @param month   Gregorian calendar month
         * @param date    Gregorian calendar date
         * @param ce    Gregorian calendar year
         * @return        The name of the day on the given Gregorian date
         * @since         2.0
         */
        public String getGregorianDay(int ce, int month, int date)
        {
            String[] days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            int[] monthDates = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };


            if ((ce % 4 == 0 && ce % 100 != 0) || ce % 400 == 0) monthDates[1] = 29;

            int tmp = 0;
            for (int i = 0; i < ce; i++)
                if ((i % 4 == 0 && i % 100 != 0) || i % 400 == 0) tmp++;

            int within = 0;
            for (int i = 0; i < (month - 1); i++)
                within += monthDates[i];

            ce -= 1;
            int dates = (ce * 365) + tmp + within + (date - 1);

            return days[dates % 7];
        }


        /**
         * This method generates the calendar for the requested year. It involves
         * a serious of steps.
         * <ul>
         *    <li>Determins the 1st day of both the Eth and Greg year</li>
         *    <li>Generates the equivalent Greg year for Eth</li>
         *    <li>If Greg leap year, March will have 29 days</li>
         *    <li>Using linked list for months and days, calendar is built</li>
         *    <li>Updates the calendar by adding holidays, and fasting holidays</li>
         * </ul>
         * Finally, it returns an array of a linked list consisting of the calendar.
         *
         * @since      2.0
         */
        public LinkedList<Day>[] getEthiopianGregorianCalendar()
        {
            this.ClearCalendar();
            /* if ce is bad, terminate method */
            if (_ce <= 0) return null;

            /* Saint name assigned to Ethiopian calendar */
            String[] saintName = { "Yohaniss", "Mathiwoss", "Markos", "Lukus" };

            /* the 1st day, in counting, begins with Tuesday, thus Tintyon */
            String[] tintyon = { "Tue", "Wed", "Thu", "Fri", "Sat", "Sun", "Mon" };

            /* Gregorian days */
            String[] gDays = {"Tuesday", "Wednesday", "Thursday", "Friday", 
                        "Saturday", "Sunday", "Monday"};

            /* get the index of ethiopian day for (1,1,am) */
            int dayIndex = getEthiopianDayIndex(1, 1, _ce);

            /* create a Day obj with a value of greg. converted from ethiopian */
            Day gDay = EthiopianToGregorian(1, 1, _ce);

            /* initialize the starting day for gregorian calendar */
            int gDate = gDay.getGregorianDate();
            int gMonth = gDay.getGregorianMonth();
            int gYear = gDay.getGregorianYear();

            /* convert the ethiopian year to gregorian */
            int gregorianYear = _ce + 7;

            /* numbers of days in every month of gregorian year modified to start from september */
            int[] gregorianMonths = { 0, 30, 31, 30, 31, 31, 28, 31, 30, 31, 30, 31, 31, 30 };

            /* if (gregorianYear + 1) leap year, then february has 29 days */
            int nextYear = gregorianYear + 1;
            if (((nextYear % 4 == 0) && (nextYear % 100 != 0)) || (nextYear % 400 == 0))
                gregorianMonths[6] = 29;

            /* If this is Ethiopian leap year, the 13th month will have 6 days */
            bool isEthiopianLeapYear = false;
            isEthiopianLeapYear = (_ce % 4 == 3 ? true : false);

            /* determine the number of days upto month/day/year */
            int until = ((month - 1) * 30) + date;             // a big ???
            if (until <= 37 && year <= 1575)
                gregorianMonths[0] = 31;

            /* get the saint name for the given year */
            String yearName = saintName[_ce % 4];
            int counter = 1;                                  // a big ???

            /* generate the calendar based on Ethiopian, but include Gregorian */
            for (int i = 0, m = 30; i < calendarYear.Length; i++)
            {
                /* if it is Ethiopian leap year, make the 13th month 6, else 5 */
                if (isEthiopianLeapYear && i == 12)
                    m = 6;
                else if (i == 12)
                    m = 5;

                /* work on months and days */
                for (int j = 0; j < m; j++)
                {
                    Day dayObj = new Day(yearName, dayIndex, i + 1, j + 1, _ce,
                                         gDays[dayIndex++], gMonth, gDate, gYear);
                    calendarYear[i].AddLast(dayObj);
                    dayIndex = (dayIndex == 7 ? 0 : dayIndex);
                    gDate++;
                    if (gDate > gregorianMonths[counter])
                    {
                        gMonth = (++gMonth) % 12 + (gMonth == 12 ? 12 : 0);
                        if (gMonth == 1) gYear++;    // if it is Jan, gYear is up by 1
                        counter++;
                        gDate = 1;
                    }
                }
            }
            return calendarYear;
        }


#endregion
        
        #region Static helper functions - private

        /**
         * Ethiopian new year occurs either on Gregorian September 11/12. If the 
         * year preceeding the new year is a leap year, Paguemain will have 6 days;
         * therefore, Ethiopian new year occurs on Gregorian September 12. This 
         * method determines when new year occurs on Gregorian September. 
         * 
         * @param year    Ethiopian year
         * @return        The date of September when Ethiopian new year occurs
         * @since         2.0
         */
        private static int whenIsEthNewYearInGregSeptember(int year)
        {
            /* it turns out that this simple formula gives when new year is */
            int newYearDate = ((year / 100) - (year / 400) - 4);

            /* if the prev ethiopian year is a leap year, new-year occrus on 12th */
            newYearDate = (((year - 1) % 4) == 3 ? ++newYearDate : newYearDate);
            return newYearDate;
        }

        
        /** 
         * Returns the day of the given date. It starts counting from Tuesday
         * of BCE era 1st day to arrive to the right one.
         *
         * @param month   Ethiopian month
         * @param date    Ethiopian date
         * @param ce      Ethiopian Common Era year  
         * @return        The day of the given date in String form
         * @since         2.0
         */
        public static String getDay(int month, int date, int ce)
        {
            /* tintyon is used to determine what the day is on new year */
            String[] tintyon = { "Tue", "Wed", "Thu", "Fri", "Sat", "Sun", "Mon" };
            int era = bce + ce;
            era -= 1;

            int tmp = 0;
            for (int i = 0; i <= era; i++)
                if (i % 4 == 3) tmp++;

            int p = (ce % 4 == 0 ? 1 : 0);
            int days = (era * 365) + tmp + ((month - 1) * 30) + (date - 1);

            return tintyon[days % 7];
        }


        /// <summary>
        /// Method to give how many days in the ethiopian month/year
        /// </summary>
        /// <param name="year">Ethiopian year</param>
        /// <param name="month">Ethiopian month</param>
        /// <returns></returns>
        public static int DaysInMonth(int year, int month)
        {
            if(month > 13)
            {
                throw new ArgumentOutOfRangeException("The month is invalid");
            }
            Calendar cal = new Calendar(year);
            LinkedList<Day>[] calendar = cal.getEthiopianGregorianCalendar();
            return calendar[month-1].Count;

        }


        /**
         * Determines what the day is for a given Ethiopian date. It starts counting
         * from 5500, the number of years in BCE, continuing the BC years. The 1st 
         * day starts with "Tuesday".
         * 
         * @param month   Ethiopian month
         * @param date    Ethiopian date
         * @param year    Ethiopian year
         * @return        Ethiopian day
         */
        private static int getEthiopianDayIndex(int month, int date, int year)
        {
            String[] tintyon = { "Tue", "Wed", "Thu", "Fri", "Sat", "Sun", "Mon" };
            int era = bce + year;
            era -= 1;

            /* Count the number of Ethiopian leap years */
            int tmp = 0;
            for (int i = 0; i <= era; i++)
            {
                if (i % 4 == 3) tmp++;
            }
            /* calculates number of days since the beginning upto ehtYear */
            int days = (era * 365) + tmp + ((month - 1) * 30) + (date - 1);

            return (days % 7);
        }
#endregion

        #region Public static conversion and reference functions for global usage
        /// <summary>
        /// The method performs conversion from Ethiopian date to Gregorian. Breifely,
        ///  <ul>
        ///    <li>Determines when Ethiopian new years occurs in Greg. September</li>
        ///    <li>Initially, converts Eth year to Greg using the 7 years diff</li>
        ///    <li>If Eth date falls after Dec, the Greg year must be up by 1 year</li>
        ///    <li>Since the Eth new year is on September, its overlaps with Gregorian; 
        ///        thus to calculate the Greg month an <b>order</b> is introduced</li>
        ///    <li>Once the Greg date is found, the day is easily obtained</li>      
        /// </ul>
        /// </summary>
        /// <param name="EthMonth">Ethiopian calendar month</param>
        /// <param name="EthDate">Ethiopian calendar date</param>
        /// <param name="EthYear">Ethiopian calendar year</param>
        /// <returns>Ethiopian day class</returns>
        
        public static Day EthiopianToGregorian(int EthMonth, int EthDate, int EthYear)
        {
            /* creates an instance of Day */
            Day dayObj = new Day("", 0, EthMonth, EthDate, EthYear);

            /* if malformed date, then terminate while returning null */
            if (EthMonth == 0 || EthDate == 0 || EthYear == 0)
                return null;

            /* determines when Eth new years occurs in Geg September */
            int newYearDay = whenIsEthNewYearInGregSeptember(EthYear);


            /* On Ethiopian September the difference between Greg. and Eth year is 7 */
            int gregorianYear = EthYear + 7;

            /* Gregorian months sequenced with respect to Ethiopian; begins Sept */
            /* Note: index 0 is used a position holder */
            int[] gregorianMonths = { 0, 30, 31, 30, 31, 31, 28, 31, 30, 31, 30, 31, 31, 30 };

            /* if (gregorianYear + 1) leap year, then february has 29 days */
            int nextYear = gregorianYear + 1;
            if (((nextYear % 4 == 0) && (nextYear % 100 != 0)) || (nextYear % 400 == 0))
                gregorianMonths[6] = 29;

            /* determine the number of days upto month/day/year */
            int until = ((EthMonth - 1) * 30) + EthDate;
            if (until <= 37 && EthYear <= 1575)
            {   // I don't remember what i did here
                until += 28;
                gregorianMonths[0] = 31;
            }
            else
            {
                until += (newYearDay - 1);
            }

            /* if it is ethiopian leap year, paguemain has six days */
            until = (EthYear - 1 % 4 == 3 && EthMonth == 13 ? ++until : until);

            /* determine the exact matching month, date, and year of gregorian */
            int m = 0;
            for (int i = 0; i < gregorianMonths.Length; i++)
            {
                if (until <= gregorianMonths[i])
                {
                    m = i;
                    dayObj.setGregorianDate(until);
                    break;
                }
                else
                {
                    m = i;
                    until -= gregorianMonths[i];
                }
            }

            /* Makes the gregorian months order with respect to the Ethiopian */
            int[] order = new int[] { 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            /* if (m > 4) is true, gregorianYear must advance by 1. for m, 
             * the lower bound is 0; that is counting starts at 0 */
            dayObj.setGregorianYear((m > 4 ? ++gregorianYear : gregorianYear));
            dayObj.setGregorianMonth(order[m]);
            dayObj.setYearName("");
            /* it is ok to get the day using ethiopian date */
            dayObj.setDay(getEthiopianDayIndex(EthMonth, EthDate, EthYear));

            return dayObj;
        }

        ///<summary>
        /// The method performs conversion from Gregorian date to Ethiopian. Breifely,
        /// <ul>
        ///  <li>Determines when Ethiopian new years occurs in Greg. September</li>
        ///    <li>Initially, converts Greg year to Eth using the 8 years diff</li>
        ///    <li>If Greg date falls after Sept 10/11, the Eth year must be up by 1
        ///        year</li>
        ///    <li>Since the Eth new year is on September, its overlaps with Gregorian; 
        ///        thus to calculate the Greg month an <b>order</b> is introduced</li>
        ///    <li>The Eth month and date are determined </li>      
        /// </ul>   
        /// </summary>
        /// <param name="GregMonth">Gregorian calendar month</param>
        /// <param name="GregDate">Gregorian calendar date</param>
        /// <param name="GregYear">Gregorian calendar year</param>
        /// <returns>Gregorian day class</returns>
        /// 
        public static Day GregorianToEthiopian(int GregMonth, int GregDate, int GregYear)
        {
            /* Saint name assigned to Ethiopian calendar */
            String[] saintName = { "Yohaniss", "Mathiwoss", "Markos", "Lukus" };

            /* create a day object */
            Day dayObj = new Day();

            /* if malformed date, then terminate while returning null */
            if (GregMonth == 0 || GregDate == 0 || GregYear == 0)
                return null;

            /* if m/d/y falls in between 10/5/1583--10/14/1583, it is invalid */
            if ((GregMonth == 10) && (GregDate >= 5) && (GregDate <= 14) && (GregYear == 1582))
                return null;

            /* Gregorian months sequenced with respect to Ethiopian; begins Sept */
            /* Note: index 0 is used a position holder */
            int[] gregorianMonths = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            /* Ethiopian months sequenced with respect to Gregorian; begins Jan */
            /* Note: index 0 is used a position holder */
            int[] ethiopianMonths = { 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 5, 30, 30, 30, 30 };

            /* if it is gregorian leap year, then february has 29 days */
            if ((GregYear % 4 == 0 && GregYear % 100 != 0) || GregYear % 400 == 0)
                gregorianMonths[2] = 29;

            /* In Sept there is 8 years deifference between Greg and Eth */
            int ethiopianYear = GregYear - 8;

            /* if it is ethiopian leap year, then pagumain has 6 days */
            ethiopianMonths[10] = (ethiopianYear % 4 == 3 ? 6 : 5);

            /* determines when Eth new years occurs in Geg September */
            int newYearDay = whenIsEthNewYearInGregSeptember(GregYear - 8);

            /* determine the number of days upto month/day/year */
            int until = 0;
            for (int i = 1; i < GregMonth; i++)
                until += gregorianMonths[i];
            until += GregDate;

            /* updates tahissas to match january 1st and */
            int tahissas = 25 + (ethiopianYear % 4 == 0 ? 1 : 0);

            /* Note the change on oct, 1582 */
            if (GregYear < 1582)
            {
                ethiopianMonths[1] = 0;
                ethiopianMonths[2] = tahissas;
            }
            else if ((until <= 277) && (GregYear == 1582))
            {
                ethiopianMonths[1] = 0;
                ethiopianMonths[2] = tahissas;
            }
            else
            {
                tahissas = newYearDay - 3;
                ethiopianMonths[1] = tahissas;
            }

            /* determine the actual equivalent ethiopian date */
            int m;
            for (m = 1; m < ethiopianMonths.Length; m++)
            {
                // System.out.println(m + " ==>\t " + until + "  " + "[" + ethiopianMonths[m] + "]") ;
                if (until <= ethiopianMonths[m])
                {
                    dayObj.setDate((m == 1 || ethiopianMonths[m] == 0 ? until + (30 - tahissas) : until));
                    break;
                }
                else
                {
                    until -= ethiopianMonths[m];
                }
            }

            /* reorder to the proper ethiopian calendar and return result */
            int[] order = { 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 1, 2, 3, 4 };
            /* if  (m > 10) is true, ethiopian year must be up by 1 */
            dayObj.setYear((m > 10 ? ++ethiopianYear : ethiopianYear));
            dayObj.setMonth(order[m]);
            dayObj.setDay(getEthiopianDayIndex(dayObj.getMonth(), dayObj.getDate(), dayObj.getYear()));
            // if filled with, e.g. a new DateTime() the year can actually be negative.
            // in this case the modulus is negative, which does all kinds of odd things,
            // but if we add 4 and mod again we are guaranteed a positive modulus.
            int index = (dayObj.getYear() % 4 + 4) % 4;
            dayObj.setYearName(saintName[index]);
            return dayObj;
        }


        /// <summary>
        /// Input a system (gregorian) DateTime value to get a class to interpret Ethiopian Dates
        /// </summary>
        /// <param name="gregorianDT">The Gregorian DateTime Object</param>
        /// <returns>Ethiopian Day class</returns>
        public static Day DateTimeToEthiopian(DateTime gregorianDT)
        {
            return GregorianToEthiopian(gregorianDT.Month, gregorianDT.Day, gregorianDT.Year);
        }
        
        /// <summary>
        /// Convert an Ethiopian date to a DateTime value (Gregorian)
        /// </summary>
        /// <param name="EthYear">Ethiopian calendar year</param
        /// <param name="EthMonth">Ethiopian calendar month</param>
        /// <param name="EthDate">Ethiopian calendar date</param>
        /// <returns>Gregorian DateTime value</returns>
        public static DateTime EthiopianToDateTime(int EthMonth, int EthDate, int EthYear)
        {
            Day myDay = EthiopianToGregorian(EthMonth, EthDate, EthYear);
            if (myDay == null) {
                return General.Util.Constants.NULLDATE;
            }
            DateTime tmp_gregorian = myDay.getGregorianDateTime();

            Day confirmDay = DateTimeToEthiopian(tmp_gregorian);

            if (confirmDay.getMonth() != EthMonth && confirmDay.getDate() != EthDate)
            {
                return General.Util.Constants.NULLDATE;
            } else
            {
                return tmp_gregorian;
            }
        }
        #endregion

    }
}