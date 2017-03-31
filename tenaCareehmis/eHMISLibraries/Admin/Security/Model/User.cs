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
	public class User: IEquatable<User>
	{

		#region Private Members
        private string _fullName;
		private string _userid; 
		//private IList<UserRole> _TuserRoleList; 
		private string _password; 
		private bool _isactive;

        private Int16 _title;
        private Int16 _jobTilte;

        public Int16 JobTilte
        {
            get { return _jobTilte; }
            set { _jobTilte = value; }
        }

        public Int16 Title
        {
            get { return _title; }
            set { _title = value; }
        }
      
		#endregion

		#region Constructor

		public User()
		{
			_userid = String.Empty; 
			//_TuserRoleList = new List<UserRole>(); 
			_password = String.Empty; 
			_isactive = false; 
		}
		#endregion // End of Default ( Empty ) Class Constuctor

		#region Required Fields Only Constructor
		/// <summary>
		/// required (not null) fields only constructor
		/// </summary>
		public User(
			string userid, 
            string fullname,
			string password, 
			bool isactive,
            Int16 title,
            Int16 jobTitle)
			: this()
		{
            _fullName = fullname;
			_userid = userid;
			_password = password;
			_isactive = isactive;
            _title = title;
            _jobTilte = jobTitle;
		}
        public User(
            string userid,
            string fullname,
            string password,
            bool isactive)
            : this()
        {
            _fullName = fullname;
            _userid = userid;
            _password = password;
            _isactive = isactive;
        }
		#endregion // End Constructor

		#region Public Properties
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }
		public virtual string UserId
		{
			get
			{ 
				return _userid;
			}

			set	
			{	
				if( value == null )
					throw new ArgumentOutOfRangeException("Null value not allowed for UserId", value, "null");
				
				if(  value.Length > 50)
					throw new ArgumentOutOfRangeException("Invalid value for UserId", value, value.ToString());
				
				_userid = value;
			}
		}
			
        //public virtual IList<UserRole> TuserRoleList
        //{
        //    get
        //    {
        //        return _TuserRoleList;
        //    }
        //    set
        //    {
        //        _TuserRoleList = value;
        //    }
        //}

		public virtual string Password
		{
			get
			{ 
				return _password;
			}

			set	
			{	
				if( value == null )
					throw new ArgumentOutOfRangeException("Null value not allowed for Password", value, "null");
				
				if(  value.Length > 45)
					throw new ArgumentOutOfRangeException("Invalid value for Password", value, value.ToString());
				
				_password = value;
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

		public virtual void AddTuserRole(UserRole obj)
		{
			#region Check if null
			if (obj == null)
				throw new ArgumentNullException("obj", "El parametro no puede ser nulo");
			#endregion
			//_TuserRoleList.Add(obj);
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
			User castObj = (User)obj; 
			return ( castObj != null ) &&
				( this._userid == castObj.UserId );
		}
		
		/// <summary>
		/// local implementation of GetHashCode based on unique value members
		/// </summary>
		public override int GetHashCode()
		{
			
			int hash = 57; 
			hash = 27 ^ hash ^ _userid.GetHashCode();
			return hash; 
		}
		#endregion
		
		#region IEquatable members

		public bool Equals(User other)
		{
			if (other == this)
				return true;
		
			return ( other != null ) &&
				( this._userid == other.UserId );
				   
		}

		#endregion
		
	}
}
