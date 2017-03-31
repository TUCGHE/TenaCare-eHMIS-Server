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


namespace Admin.Security.Model
{
	[Serializable]
	public class Permission: IEquatable<Permission>
	{

		#region Private Members

		private int _permissionid; 
		private int _roleid; 
		private int _securableid; 
		private bool _canwrite; 
		private bool _canread; 
		private bool _isactive; 		
		#endregion

		#region Constructor

		public Permission()
		{
			_permissionid = 0;
            _roleid = 0;
            _securableid = 0;
			_canwrite = false; 
			_canread = false; 
			_isactive = false; 
		}
		#endregion // End of Default ( Empty ) Class Constuctor

		#region Required Fields Only Constructor
		/// <summary>
		/// required (not null) fields only constructor
		/// </summary>
		public Permission(
			int permissionid, 
			int roleid, 
			int securableid, 
			bool canwrite, 
			bool canread, 
			bool isactive)
			: this()
		{
			_permissionid = permissionid;
			_roleid = roleid;
			_securableid = securableid;
			_canwrite = canwrite;
			_canread = canread;
			_isactive = isactive;
		}
		#endregion // End Constructor

		#region Public Properties
			
		public virtual int PermissionId
		{
			get
			{ 
				return _permissionid;
			}
			set
			{
				_permissionid = value;
			}

		}
			
		public virtual int RoleId
		{
			get
			{ 
				return _roleid;
			}
			set
			{
				_roleid = value;
			}

		}
			
		public virtual int SecurableId
		{
			get
			{ 
				return _securableid;
			}
			set
			{
				_securableid = value;
			}

		}
			
		public virtual bool CanWrite
		{
			get
			{ 
				return _canwrite;
			}
			set
			{
				_canwrite = value;
			}

		}
			
		public virtual bool CanRead
		{
			get
			{ 
				return _canread;
			}
			set
			{
				_canread = value;
			}

		}
			
		public virtual bool IsActive
		{
			get
			{ 
				return _isactive;
			}
			set
			{
				_isactive = value;
			}

		}
			
				
		#endregion 

		#region Public Functions

		#endregion //Public Functions

		#region Equals And HashCode Overrides
		/// <summary>
		/// local implementation of Equals based on unique value members
		/// </summary>
		public override bool Equals( object obj )
		{
			if( this == obj ) return true;
			if( ( obj == null ) || ( obj.GetType() != this.GetType() ) ) return false;
            Permission castObj = (Permission)obj; 
			return ( castObj != null ) &&
				( this._permissionid == castObj.PermissionId );
		}
		
		/// <summary>
		/// local implementation of GetHashCode based on unique value members
		/// </summary>
		public override int GetHashCode()
		{
			
			int hash = 57; 
			hash = 27 ^ hash ^ _permissionid.GetHashCode();
			return hash; 
		}
		#endregion
		
		#region IEquatable members

		public bool Equals(Permission other)
		{
			if (other == this)
				return true;
		
			return ( other != null ) &&
				( this._permissionid == other.PermissionId );
				   
		}

		#endregion
		
	}
}
