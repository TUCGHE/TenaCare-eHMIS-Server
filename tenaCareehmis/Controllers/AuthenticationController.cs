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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SqlManagement.Database;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Web.Http.Cors;
using Admin.Security.Model;
using Admin.Security.Utility;
using eHMISWebApi.Models;

namespace eHMISWebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/authentication")]
    public class AuthenticationController : ApiController
    {
        public class LoginCredentials
        {
            public String username, password;
        }

        eHMISEntities db = new eHMISEntities();

        // POST api/authentication/login
        [Route("login")]
        [AcceptVerbs("Post")]   
        public HttpResponseMessage Login(LoginCredentials credentials)
        {           

            if ((credentials == null) || (credentials.username == null) || (credentials.password == null))
                return this.Request.CreateResponse(HttpStatusCode.Forbidden, "Please provide all credentials");

            User user = new User(credentials.username, "", credentials.password, true);
            if (SecurityHelper.AuthenticateUser(user))
            {
                return this.Request.CreateResponse<UserView>(
               HttpStatusCode.OK, new UserView(user));
            }

            return this.Request.CreateResponse(HttpStatusCode.Forbidden, "INVALID Credentials!");
        }

        // POST api/authentication/users
        [Route("users")]
        [AcceptVerbs("Post")]
        public HttpResponseMessage GetUsers(UserID UserId)
        {
            if(UserId == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden, "Please Provide a valid User Object with the 'UserId' field!");
            }

            if (UserId.UserId == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden, "Please Provide a valid User Object with the 'UserId' field!");
            }

            // find user with userView.UserId

            bool canGivePermission = SecurityHelper.ApplyWritePermissionPolicy(SecurityHelper.AuthorizeUser(UserId.UserId),
               "Super Administration");
            List<User> users = UserManagement.GetAllUsersList(canGivePermission, true); // include in-active users
            Dictionary<String, List<UserRole>> userRoles = new Dictionary<String, List<UserRole>>();
            for(int u = 0; u < users.Count; u++)
            {
                List<UserRole> roles = new List<UserRole>();
                DataTable dtRoles = SecurityDAO.GetRolesPerUser(users[u].UserId);
                foreach(DataRow row in dtRoles.Rows)
                {
                    roles.Add(new UserRole((int)row["UserRoleId"],(string)row["UserId"],(int)row["RoleId"], (bool)row["IsActive"]));
                }
                
                userRoles.Add(users[u].UserId, roles);
            }

            UserCollection collection = new UserCollection();
            collection.Users = users;
            collection.UserRoles = userRoles;

            return this.Request.CreateResponse<UserCollection>(
               HttpStatusCode.OK, collection);          
        }

        // POST api/authentication/roles
        [Route("roles")]
        [AcceptVerbs("Get")]
        public HttpResponseMessage GetAllRoles()
        {

            var roles = SecurityDAO.GetAllRolesList();

            return this.Request.CreateResponse<List<Role>>(
               HttpStatusCode.OK, roles);
        }



        // POST api/authentication/update_user
        [Route("update_user")]
        [AcceptVerbs("Post")]
        public HttpResponseMessage UpdateUser(UpdateUserView updateUser)
        {
            User user = updateUser.user;
            List<ViewUserRole> roles = updateUser.roles;

            // to avoid modifying a pre-existing user-password, we check if it was encrypted and decrypt it 
            // before calling SaveUser() -- which currently will re-encrypt an already encrypted password even for existing records!
            if (user.Password.EndsWith("="))
            {
                string decryptedPass = CryptorEngine.Decrypt(user.Password, false);
                user.Password = decryptedPass;
            }

            bool success = SecurityDAO.SaveUser(user, user.FullName, false, user.UserId); // note: we don't expect users to alter existing login-id

            if(roles != null)
            {
                foreach (ViewUserRole vrole in roles)
                {
                    SecurityDAO.deleteAllRolesForUser(user.UserId, String.Format("{0}", vrole._roleid));

                    UserRole role = new UserRole();
                    role.UserId = user.UserId;
                    role.RoleId = vrole._roleid;
                    role.IsActive = vrole._isactive;
                    SecurityDAO.SaveUserRole(true, role);
                }
            }            
           

            return this.Request.CreateResponse<String>(
               success? HttpStatusCode.OK : HttpStatusCode.BadRequest, success?"Update Successful": "Update Failed!");
        }

        // POST api/authentication/update_user
        [Route("create_user")]
        [AcceptVerbs("Post")]
        public HttpResponseMessage CreateUser(User newUser)
        {           

            bool success = SecurityDAO.SaveUser(newUser, newUser.FullName, true, newUser.UserId); // note: we don't expect users to alter existing login-id


            return this.Request.CreateResponse<String>(
               success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, success ? "User Creation Successful" : "User Creation Failed!");
        }

        // POST api/authentication/delete_user
        [Route("delete_user")]
        [AcceptVerbs("Post")]
        public HttpResponseMessage DeleteUser(UserID userID)
        {
            if((userID != null) && (userID.UserId != null))
            {
                SecurityDAO.DeleteUserInfo(userID.UserId);

                return this.Request.CreateResponse<String>(
                   HttpStatusCode.OK, "User Deletion Successful");
            }else
            {

                return this.Request.CreateResponse<String>(
                   HttpStatusCode.BadRequest, "User Deletion Failed");
            }
            
           
        }

        /// <summary>
        /// Should help prevent leaking sensitive user info back to API clients such as if we
        /// were to directly serialize the User object, which has a password field in plain!
        /// </summary>
        public class UserView
        {
            public String FullName;
            public String UserId;

            public UserView(User user)
            {
                this.FullName = user.FullName == null ? "" : user.FullName;
                this.UserId = user.UserId;
            }
        }

        public class UserID
        {
            public String UserId;

        }

        public class UserCollection
        {
            public List<User> Users;
            public Dictionary<String,List<UserRole>> UserRoles;

        }

        public class ViewUserRole
        {
            public int _roleid;
            public bool _isactive;
        }

        public class UpdateUserView
        {
            public User user;
            public List<ViewUserRole> roles;

        }
    }
}