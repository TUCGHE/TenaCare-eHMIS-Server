using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.IO;
using General.Util.Logging;

namespace eHMIS.XML
{
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
        private static string _configFileName = System.Windows.Forms.Application.StartupPath + "\\ccpts.config.xml";
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
                    throw new ApplicationException("Attempt to set config file to non-existent location" +
                                                    "in AppSettingsUtility.  filename:" + value);
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
                doc.Load(reader);
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
                Logger.Log("*** Application Settings Changed ****");
            }
            catch (IOException e)
            {
                Logger.Log("ERROR WRITING TO: " + _configFileName);
                Logger.Log(e);
            }

            return isConfigFileUpdated;
        }


        /// <summary>
        /// returns the app setting key if this has been initialized, otherwise
        /// returns null.
        /// </summary>
        /// <param name="passedKey"></param>
        /// <returns></returns>
        public static string GetAppSetting(String passedKey)
        {
            if (doc != null)
            {
                String appSettingValue = null;

                // Find the appSettings Node
                XmlNodeList nodeList = doc.GetElementsByTagName("appSettings");
                XmlNode appSettingsParentNode = nodeList.Item(0);

                // Get all the child nodes
                XmlNodeList appSettingsNodeList = appSettingsParentNode.ChildNodes;

                // Get the key - value pair node which is a node named '<add>' 
                for (int i = 0; i < appSettingsNodeList.Count; i++)
                {
                    XmlNode appSettingsNode = appSettingsNodeList.Item(i);

                    if (appSettingsNode.Name.Equals("add"))
                    {
                        XmlAttributeCollection attributes = appSettingsNode.Attributes;
                        XmlNode keyAttribute = attributes.GetNamedItem("key");
                        String keyAttributeValue = keyAttribute.Value;

                        if (keyAttributeValue.Equals(passedKey))
                        {
                            appSettingValue = attributes.GetNamedItem("value").Value;
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

        public static void SetAppSetting(String passedKey, String passedValue)
        {
            bool wasKeyFound = false;

            // Find the appSettings Node
            XmlNodeList nodeList = doc.GetElementsByTagName("appSettings");
            XmlNode appSettingsParentNode = nodeList.Item(0);

            // Get all the child nodes
            XmlNodeList appSettingsNodeList = appSettingsParentNode.ChildNodes;

            // Get the key - value pair node which is a node named '<add>' 
            for (int i = 0; i < appSettingsNodeList.Count; i++)
            {
                XmlNode appSettingsNode = appSettingsNodeList.Item(i);

                if (appSettingsNode.Name.Equals("add"))
                {
                    XmlAttributeCollection addNodeAttributes = appSettingsNode.Attributes;

                    XmlNode keyAttributeNode = addNodeAttributes.GetNamedItem("key");
                    String keyAttributeName = keyAttributeNode.Value;

                    if (keyAttributeName.Equals(passedKey))
                    {
                        XmlNode valueAttributeNode = addNodeAttributes.GetNamedItem("value");
                        valueAttributeNode.Value = passedValue;
                        wasKeyFound = true;
                    }
                }
            }
            if (!wasKeyFound)
            {
                throw new Exception("AppSetting NOT FOUND!");
            }
        }

        public static String AppSettingsToString()
        {
            StringBuilder sBuilder = new StringBuilder();

            XmlNodeList nodeList = doc.GetElementsByTagName("appSettings");

            XmlNode appSettingsParentNode = nodeList.Item(0);

            sBuilder.Append("appSettingsParentNode.Name: " + appSettingsParentNode.Name + "\n");

            XmlNodeList appSettingsNodeList = appSettingsParentNode.ChildNodes;
            for (int i = 0; i < appSettingsNodeList.Count; i++)
            {

                XmlNode appSettingsNode = appSettingsNodeList.Item(i);

                if (appSettingsNode.Name.Equals("add"))
                {
                    sBuilder.Append("\n    key: " + appSettingsNode.Attributes.GetNamedItem("key").Value + "\n");
                    sBuilder.Append("    value: " + appSettingsNode.Attributes.GetNamedItem("value").Value + "\n");
                }

            }
            return sBuilder.ToString();
        }

        #endregion

    }
}
