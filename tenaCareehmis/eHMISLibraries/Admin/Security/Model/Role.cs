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
	public class Role: IEquatable<Role>
	{

		#region Private Members

		private int _roleid; 
		private IList<Permission> _TpermissionList; 
		private IList<UserRole> _TuserRoleList; 
		private string _roledescription; 
		private bool _isactive; 		
		#endregion

		#region Constructor

		public Role()
		{
			_roleid = 0; 
			_TpermissionList = new List<Permission>(); 
			_TuserRoleList = new List<UserRole>(); 
			_roledescription = String.Empty; 
			_isactive = false; 
		}
		#endregion // End of Default ( Empty ) Class Constuctor

		#region Required Fields Only Constructor
		/// <summary>
		/// required (not null) fields only constructor
		/// </summary>
		public Role(
			int roleid, 
			string roledescription, 
			bool isactive)
			: this()
		{
			_roleid = roleid;
			_roledescription = roledescription;
			_isactive = isactive;
		}
		#endregion // End Constructor

		#region Public Properties
			
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
			
		public virtual IList<Permission> TpermissionList
		{
			get
			{
				return _TpermissionList;
			}
			set
			{
				_TpermissionList = value;
			}
		}

		public virtual IList<UserRole> TuserRoleList
		{
			get
			{
				return _TuserRoleList;
			}
			set
			{
				_TuserRoleList = value;
			}
		}

		public virtual string RoleDescription
		{
			get
			{ 
				return _roledescription;
			}

			set	
			{	
				if( value == null )
					throw new ArgumentOutOfRangeException("Null value not allowed for RoleDescription", value, "null");
				
				if(  value.Length > 45)
					throw new ArgumentOutOfRangeException("Invalid value for RoleDescription", value, value.ToString());
				
				_roledescription = value;
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

		public virtual void AddTpermission(Permission obj)
		{
			#region Check if null
			if (obj == null)
				throw new ArgumentNullException("obj", "El parametro no puede ser nulo");
			#endregion
			_TpermissionList.Add(obj);
		}
		

		public virtual void AddTuserRole(UserRole obj)
		{
			#region Check if null
			if (obj == null)
				throw new ArgumentNullException("obj", "El parametro no puede ser nulo");
			#endregion
			_TuserRoleList.Add(obj);
		}
		

		#endregion //Public Functions

		#region Equals And HashCode Overrides
		/// <summary>
		/// local implementation of Equals based on unique value members
		/// </summary>
		public override bool Equals( object obj )
		{
			if( this == obj ) return true;
			if( ( obj == null ) || ( obj.GetType() != this.GetType() ) ) return false;
			Role castObj = (Role)obj; 
			return ( castObj != null ) &&
				( this._roleid == castObj.RoleId );
		}
		
		/// <summary>
		/// local implementation of GetHashCode based on unique value members
		/// </summary>
		public override int GetHashCode()
		{
			
			int hash = 57; 
			hash = 27 ^ hash ^ _roleid.GetHashCode();
			return hash; 
		}
		#endregion
		
		#region IEquatable members

		public bool Equals(Role other)
		{
			if (other == this)
				return true;
		
			return ( other != null ) &&
				( this._roleid == other.RoleId );
				   
		}

		#endregion
		
	}
}
