/*
 * 
 * Copyright � 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
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

namespace Admin.Security.Utility
{
    public class eHMISROLES 
    {
        private static  string[] all_roles = new string[] { "HMIS Data Encoder", 
                                             "HMIS Reporter",
                                             "Admin",
                                             "Super Admin",
                                              };
        public static List<string> GetAllRoles()
        {
            List<string> roles = new List<string>();
            foreach (string r in all_roles)
                roles.Add(r);
            return roles;
        }
    }
    public class eHMISSECURABLES
    {
        private static string[] all_securables = new string[] { "HMIS Data Entry Form", 
                                             "HMIS Report Form",
                                             "HMIS User Admin Form",
                                             "Super Administration",
                                               };


        public static List<string> GetAllSecurable()
        {
            List<string> securables = new List<string>();
            foreach (string r in all_securables)
                securables.Add(r);
            return securables;
        }

    }
}