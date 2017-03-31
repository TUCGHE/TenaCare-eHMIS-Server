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
	public class Securable: IEquatable<Securable>
	{

		#region Private Members

		private int _securableid; 
		private IList<Permission> _TpermissionList; 
		private string _description; 
		private bool _isactive; 		
		#endregion

		#region Constructor

		public Securable()
		{
			_securableid = 0; 
			_TpermissionList = new List<Permission>(); 
			_description = String.Empty; 
			_isactive = false; 
		}
		#endregion // End of Default ( Empty ) Class Constuctor

		#region Required Fields Only Constructor
		/// <summary>
		/// required (not null) fields only constructor
		/// </summary>
		public Securable(
			int securableid)
			: this()
		{
			_securableid = securableid;
			_description = String.Empty;
			_isactive = false;
		}
		#endregion // End Constructor

		#region Public Properties
			
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

		public virtual string Description
		{
			get
			{ 
				return _description;
			}

			set	
			{	
				if(  value != null &&  value.Length > 45)
					throw new ArgumentOutOfRangeException("Invalid value for Description", value, value.ToString());
				
				_description = value;
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
		

		#endregion //Public Functions

		#region Equals And HashCode Overrides
		/// <summary>
		/// local implementation of Equals based on unique value members
		/// </summary>
		public override bool Equals( object obj )
		{
			if( this == obj ) return true;
			if( ( obj == null ) || ( obj.GetType() != this.GetType() ) ) return false;
			Securable castObj = (Securable)obj; 
			return ( castObj != null ) &&
				( this._securableid == castObj.SecurableId );
		}
		
		/// <summary>
		/// local implementation of GetHashCode based on unique value members
		/// </summary>
		public override int GetHashCode()
		{
			
			int hash = 57; 
			hash = 27 ^ hash ^ _securableid.GetHashCode();
			return hash; 
		}
		#endregion
		
		#region IEquatable members

		public bool Equals(Securable other)
		{
			if (other == this)
				return true;
		
			return ( other != null ) &&
				( this._securableid == other.SecurableId );
				   
		}

		#endregion
		
	}
}
