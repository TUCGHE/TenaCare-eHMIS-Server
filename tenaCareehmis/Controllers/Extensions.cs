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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace eHMISWebApi.Controllers
{
    static class Extensions
    {
        public static T Map<T>(this IDataReader dr)
        {
            T obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (!object.Equals(dr[prop.Name], DBNull.Value))
                {
                    prop.SetValue(obj, dr[prop.Name], null);
                }
            }
            return obj;
        }

        public static int Quarter(this int month)
        {
            if (month == 11 || month == 12 || month == 1)
                return 1;

            if (month >= 2 && month <= 4)
                return 2;

            if (month >= 5 && month <= 7)
                return 3;

            if (month >= 8 && month <= 10)
                return 4;

            throw new Exception("Invalid month");
        }

        public static int Month(string month)
        {
            //string tikimit = "Tikimit";
            if (month == "Meskerem")
                return 1;

            if (month == "Tikimt")
                return 2;

            if (month == "Hidar")
                return 3;

            if (month == "Tahisas")
                return 4;

            if (month == "Tir")
                return 5;

            if (month == "Yekatit")
                return 6;

            if (month == "Megabit")
                return 7;

            if (month == "Miazia")
                return 8;

            if (month == "Ginbot")
                return 9;

            if (month == "Sene")
                return 10;

            if (month == "Hamle")
                return 11;

            if (month == "Nehase")
                return 12;
            throw new Exception("Invalid month");
        }
        public static int MonthFromQuarter(this int quarter)
        {
            if (quarter == 1)
                return 1;

            if (quarter == 2)
                return 4;

            if (quarter == 3)
                return 7;

            if (quarter == 4)
                return 10;

            throw new Exception("Invalid quarter");
        }
    }
}
