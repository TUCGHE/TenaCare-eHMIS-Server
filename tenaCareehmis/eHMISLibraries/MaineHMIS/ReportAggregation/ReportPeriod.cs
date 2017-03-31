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

namespace eHMIS.HMIS.ReportAggregation
{
    public enum PeriodType
    {
        WEEKLY = 0,
        MONTHLY = 1,
        QUARTERLY = 2,
        YEARLY = 3,
    }


   public class ReportPeriod
    {
        PeriodType _type;

        int _month;
        int _quarter;
        int _week;
        decimal _year;


        public PeriodType Type
        {
            get { return _type; }
            set { _type = value; }
        }


       public int Month
       {
           get { return _month; }
           set { _month = value; }
       }

        public int Quarter
        {
            get { return _quarter; }
            set { _quarter = value; }
        }

        public decimal Year
        {
            get { return _year; }
            set { _year = value; }
        }




        public ReportPeriod(PeriodType type, int mon, int quar, Decimal yer)
        {
            _type = type;

            _month = mon;

            _quarter = quar;

            _year = yer;


        }

        public int getStartingMonth()
        {

            switch (_quarter)
            {

                case 1: return 11;
                case 2: return 2;
                case 3: return 5;
                case 4: return 8;
            }
            return 11;
        }


       public int getMiddleMonth()
       {

           switch (_quarter)
           {

               case 1: return 12;
               case 2: return 3;
               case 3: return 6;
               case 4: return 9;
           }
           return 12;
       }

       public int getLastMonth()
       {

           switch (_quarter)
           {

               case 1: return 1;
               case 2: return 4;
               case 3: return 7;
               case 4: return 10;
           }
           return _month;
       }

        public string getQueryParameters()
        {
            switch (_type)
            {

                case PeriodType.MONTHLY:
                    string monthQueryStr = "( month=" + _month.ToString() + " ) and ( Year = " + _year.ToString() + ")";
                    return monthQueryStr;                    

                case PeriodType.QUARTERLY:
                    int monthBeg = getStartingMonth();

                    int monthMid = (monthBeg < 12) ? monthBeg + 1 : 1;
                    int monthEnd = (monthMid < 12) ? monthMid + 1 : 1;

                    string month1Str = "(( month=" + monthBeg.ToString() + " ) and ( Year = " + _year.ToString() + "))";
                    if ((monthBeg == 11))
                    {
                        month1Str = "(( month=" + monthBeg.ToString() + " ) and ( Year = " + (_year - 1).ToString() + "))";
                    }
                    string month2Str = "(( month=" + monthMid.ToString() + " ) and ( Year = " + _year.ToString() + "))";
                    if ((monthMid == 12))
                    {
                        month2Str = "(( month=" + monthMid.ToString() + " ) and ( Year = " + (_year - 1).ToString() + "))";
                    }
                    string month3Str = "(( month=" + monthEnd.ToString() + " ) and ( Year = " + _year.ToString() + "))";
                    string quarterlyQuery = "((" + month1Str + " or " + month2Str + " or " + month3Str + "))";
                    return quarterlyQuery;                    
                case PeriodType.YEARLY:
                    string yearQueryStr = " ( Year = " + _year.ToString() + ")";
                    return yearQueryStr;                   
            }

            return "(1=1)";
        }

 
    }

}
