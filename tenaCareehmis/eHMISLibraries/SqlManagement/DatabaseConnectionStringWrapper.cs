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
using System.Text;

namespace SqlManagement.Database
{
	public class DatabaseConnectionStringWrapper
	{   
		#region Class Variables
		private int[] hostIPOctets = { 0, 0, 0, 0 };
	    private String dbName, user, password, hostName, instanceName;
	    #endregion

		#region Properties
		public int[] HostIPOctets
		{
			get { return hostIPOctets; }
			set 
			{ 
				hostIPOctets = value; 

				for( int i = 0; i < hostIPOctets.Length; i++ )
				{
					if( hostIPOctets[i] > 255 || hostIPOctets[i] < 0 )
					{
						hostIPOctets = null; // Invalid IP Address return to null

						throw new ArgumentOutOfRangeException( "\nOctet #: " + i + " is out of range. "
															   + "\nValue: " + hostIPOctets[i] 
															   + "\nValid Range: ( -1 < x < 255 )\n" );
					}
				}

				if( hostIPOctets[0] == 127 && hostIPOctets[1] == 0 && hostIPOctets[2] == 0 && hostIPOctets[3] == 1 )
				{
					hostName = "(local)";
				}
				else
				{
					hostName = null;
				}
			}
		}

		public String HostName
		{
			get { 
				if( hostName == null )
					{
					hostName = GetHostIP();
					}
				return hostName;
				}
			set { hostName = value; }
		}

		public String DbName
		{
			get { return dbName; }
			set { dbName = value; }
		}

		public String User
		{
			get { return user; }
			set { user = value; }
		}

		public String Password
		{
			get { return password; }
			set { password = value; }
		}

	    public String InstanceName
	    {
	        get { return instanceName; }
            set { instanceName = value; }
	    }

	    #endregion 

		#region Constructors
		public DatabaseConnectionStringWrapper()
		{
		}
		
		public DatabaseConnectionStringWrapper( String hostName, String dbName, String user, String password  )
		{
			this.HostName = hostName;
			this.DbName = dbName;
			this.User = user;
			this.Password = password;
		}

		public DatabaseConnectionStringWrapper( int[] ipOctets, String dbName, String user, String password  )
		{
			this.HostIPOctets = ipOctets;
			this.DbName = dbName;
			this.User = user;
			this.Password = password;
		}

		public DatabaseConnectionStringWrapper( int[] ipOctets, String hostName, String dbName, String user, String password  )
		{
			this.HostIPOctets = ipOctets;
			this.HostName = hostName;
			this.DbName = dbName;
			this.User = user;
			this.Password = password;
		}
		#endregion 
		
		#region Public Methods
		public override String ToString()
		{
		StringBuilder aBuilder = new StringBuilder();
		aBuilder.Append( "Data Source=" + this.HostName );

        aBuilder.Append( @"\" + this.instanceName + "; ");

		aBuilder.Append( "Initial Catalog=" + dbName + "; " );

		aBuilder.Append( "User ID=" + user + "; " );

		aBuilder.Append( "Password=" + password + "; " );		
		
		return aBuilder.ToString();
		}

		/// <summary>
		/// 2006-07-10 JJ: Adding an equals comparison method
		/// </summary>
		/// <param name="obj">the object to compare</param>
		/// <returns>true if all properties are the same, false otherwise</returns>
		public override bool Equals(object obj)
		{
			if(obj.GetType() != this.GetType())
			{
				return false;
			}
			else
			{
				DatabaseConnectionStringWrapper dbwrapper = (DatabaseConnectionStringWrapper)obj;
				if(dbwrapper.DbName == this.DbName &&
					dbwrapper.hostName == this.hostName &&
                    dbwrapper.instanceName == this.instanceName &&
					dbwrapper.password == this.password &&
					dbwrapper.user == this.user)
					return true;
				else
					return false;
			}
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public String GetHostIP()
		{
		StringBuilder aBuilder = new StringBuilder();

		aBuilder.Append( hostIPOctets[0] + "." );
		aBuilder.Append( hostIPOctets[1] + "." );
		aBuilder.Append( hostIPOctets[2] + "." );
		aBuilder.Append( hostIPOctets[3] );		

		return aBuilder.ToString();
		}

		public DatabaseConnectionStringWrapper Clone()
		{
		return new DatabaseConnectionStringWrapper( this.hostIPOctets, this.hostName, this.dbName, this.user, this.password );
		}
		#endregion

	}
}
