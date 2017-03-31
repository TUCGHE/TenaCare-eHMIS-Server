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
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Xml.XPath;

namespace SqlManagement.Database
{
	/// <summary>
    /// Wrapper for access to the app.config alternative ccpts.config.xml.
    /// Was needed in .NET 1.0 to allow for writing changes at runtime.
    /// To be replaced by System.Configuration.ConfigurationManager and the app.config file.
	/// </summary>
    [Obsolete("Since upgrading to .NET 2.0 this class is obsolete. Please use System.Configuration.ConfigurationManager and the app.config file.", false)]
	public class AppSettingsUtility
	{
	    #region Member Variables
	    static private XmlDocument doc;

	    //2007-02-16 @JJ: allowing this class to not crash if the ccpts.config.xml directory wants to be set externalliy. 
	    //if this is the case, you must call LoadConfigFile after setting the directory.
	    public static bool AllowBadPath = true;


	    /*
		 * IMPORTANT NOTE THE FOLLOWING:
		 * An alternate XML file was necessary because I was unable to get the Executor to release the handle on the MS standard app config file.
		 * -TCHUBS
		 * 
		 * 2006-10-10 JJ:  we should think of a better way to do this as it screws up unit testing pretty annoyingly.  
		 * I'm making it public so that I can access this in the Unit Tests.
		 * 
		 * 2006-10-10 JJ:  That didn't work, I copied for file into my unit tests runtime by hand.
		 */
	    private static string _configFileName = "";//System.Windows.Forms.Application.StartupPath + "\\ccpts.config.xml";
	    #endregion

	    #region Constructors
	    static AppSettingsUtility()
	    {
            LoadConfigFile();
	    }
	    #endregion

        #region Properties
        /// <summary>
        /// The config filename location.  if you change this you must LoadConfigFile()
        /// </summary>
	    public static string ConfigFileName
	    {
	        get
	        {
	            return _configFileName;
	        }
            set
            {
                if (!File.Exists(value))
                {
                    throw new ApplicationException( "Attempt to set config file to non-existent location" +
                                                    "in AppSettingsUtility.  filename:" + value );
                }
                else
                {
                    _configFileName = value;
                }
            }
	    }
        #endregion

        #region Public Static Methods
        /// <summary>
	    /// Attempts to load a config file from Config which defaults to 
	    /// Application.StartupPath + "\\ccpts.config.xml".  If this fails, you can later 
	    /// change the config file name and call this method again.
	    /// </summary>
	    public static void LoadConfigFile()
	    {
	        if (File.Exists(_configFileName))
	        {
	            doc = new XmlDocument();
	            XmlTextReader reader = new XmlTextReader(_configFileName);
	            doc.Load( reader );
	            reader.Close();
	        }
	        else
	        {
	            doc = null;
	        }
	    }

	    /* 
		 * This method is currently private because the CCPAPP maintains a lock on those files
		 * As the app becomes more sophisticated (and this class gets more use) we will need 
		 * a more elegant solution. - TCHUBS
		 */
	    public static bool SaveConfigFile()
	    {
	        bool isConfigFileUpdated = false;
	        try	
	        {
	            File.Copy(_configFileName, _configFileName + ".bak", true);
	            File.Delete(_configFileName);
	            doc.Save(_configFileName);
	            isConfigFileUpdated = true;	            
	        }
	        catch( IOException e )
	        {	           
	        }
		
	        return isConfigFileUpdated;
	    }


	    /// <summary>
	    /// returns the app setting key if this has been initialized, otherwise
	    /// returns null.
	    /// </summary>
	    /// <param name="passedKey"></param>
	    /// <returns></returns>
	    public static string GetAppSetting( String passedKey )
	    {
	        if (doc != null)
	        {
	            String appSettingValue = null;

	            // Find the appSettings Node
	            XmlNodeList nodeList = doc.GetElementsByTagName( "appSettings" );
	            XmlNode appSettingsParentNode = nodeList.Item( 0 );

	            // Get all the child nodes
	            XmlNodeList appSettingsNodeList = appSettingsParentNode.ChildNodes;

	            // Get the key - value pair node which is a node named '<add>' 
	            for ( int i = 0; i < appSettingsNodeList.Count; i++ )
	            {
	                XmlNode appSettingsNode = appSettingsNodeList.Item( i );

	                if ( appSettingsNode.Name.Equals( "add" ) )
	                {
	                    XmlAttributeCollection attributes = appSettingsNode.Attributes;
	                    XmlNode keyAttribute = attributes.GetNamedItem( "key" );
	                    String keyAttributeValue = keyAttribute.Value;

	                    if ( keyAttributeValue.Equals( passedKey ) )
	                    {
	                        appSettingValue = attributes.GetNamedItem( "value" ).Value;
	                    }
	                }
	            }

	            return appSettingValue;
	        }
	        else
	        {
	            return null;
	        }
	    }

	    public static void SetAppSetting( String passedKey, String passedValue )
	    {
	        bool wasKeyFound = false;

	        // Find the appSettings Node
	        XmlNodeList nodeList = doc.GetElementsByTagName( "appSettings" );
	        XmlNode appSettingsParentNode = nodeList.Item( 0 );
			
	        // Get all the child nodes
	        XmlNodeList appSettingsNodeList = appSettingsParentNode.ChildNodes;

	        // Get the key - value pair node which is a node named '<add>' 
	        for( int i = 0; i < appSettingsNodeList.Count; i++ )
	        {
	            XmlNode appSettingsNode = appSettingsNodeList.Item( i );

	            if( appSettingsNode.Name.Equals( "add" ) )
	            {
	                XmlAttributeCollection addNodeAttributes = appSettingsNode.Attributes;

	                XmlNode keyAttributeNode = addNodeAttributes.GetNamedItem( "key" );
	                String keyAttributeName = keyAttributeNode.Value;
					
	                if( keyAttributeName.Equals( passedKey ) )
	                {
	                    XmlNode valueAttributeNode = addNodeAttributes.GetNamedItem( "value" );
	                    valueAttributeNode.Value = passedValue;
	                    wasKeyFound = true;
	                }
	            }
	        }				
	        if( !wasKeyFound )
	        {
	            throw new Exception( "AppSetting NOT FOUND!" );
	        }
	    }

	    public static String AppSettingsToString()
	    {
	        StringBuilder sBuilder = new StringBuilder();
			
	        XmlNodeList nodeList = doc.GetElementsByTagName( "appSettings" );

	        XmlNode appSettingsParentNode = nodeList.Item( 0 );

	        sBuilder.Append( "appSettingsParentNode.Name: " + appSettingsParentNode.Name + "\n" );

	        XmlNodeList appSettingsNodeList = appSettingsParentNode.ChildNodes;
	        for( int i = 0; i < appSettingsNodeList.Count; i++ )
	        {
	
	            XmlNode appSettingsNode = appSettingsNodeList.Item( i );

	            if( appSettingsNode.Name.Equals( "add" ) )
	            {
	                sBuilder.Append( "\n    key: " + appSettingsNode.Attributes.GetNamedItem( "key" ).Value + "\n" );
	                sBuilder.Append( "    value: " + appSettingsNode.Attributes.GetNamedItem( "value" ).Value + "\n" );
	            }				

	        }
	        return sBuilder.ToString();		
	    }

	    public static DatabaseConnectionStringWrapper GetDBConnectionStringFromConfigFile()
	    {
	        DatabaseConnectionStringWrapper dbConString = new DatabaseConnectionStringWrapper();
	        dbConString.HostName = GetAppSetting( "DatabaseHost" );
	        dbConString.InstanceName = GetAppSetting( "DatabaseInstanceName" );
	        dbConString.DbName = GetAppSetting( "DatabaseName" );
	        dbConString.User = GetAppSetting( "DatabaseUser" );
	        dbConString.Password = GetAppSetting( "DatabasePassword" );
			
	        return dbConString;
	    }

	    public static void SetDBConnectionString( DatabaseConnectionStringWrapper dbConString )
	    {
	        SetAppSetting( "DatabaseHost", dbConString.HostName );
	        SetAppSetting( "DatabaseInstanceName", dbConString.InstanceName);
	        SetAppSetting( "DatabaseName", dbConString.DbName );
	        SetAppSetting( "DatabaseUser", dbConString.User );
	        SetAppSetting( "DatabasePassword", dbConString.Password );
        }
        #endregion

    }
}
