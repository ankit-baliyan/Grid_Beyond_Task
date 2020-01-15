using System;
using System.Configuration;

namespace GridBeyondTask.Modules
{
    class cSettings
    {
        private string _databaseServer;
        private string _databaseUserID;
        private string _databasePassword;
        private string _databaseFilePath;

        public string DatabaseServer
        {
            get { return _databaseServer; }
        }

        public string DatabaseUserID
        {
            get { return _databaseUserID; }
        }

        public string DatabasePassword
        {
            get { return _databasePassword; }
        }

        public string DatabaseFilePath
        {
            get { return _databaseFilePath; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public cSettings()
        {
            _databaseServer = GetConfig("DatabaseServer");
            _databaseUserID = GetConfig("DatabaseUserID");
            _databasePassword = GetConfig("DatabasePassword");
            _databaseFilePath = GetConfig("DatabaseFilePath");
        }

        /// <summary>
        /// Retrieve Value for the variable supplied from config
        /// </summary>
        /// <param name="key">varaible</param>
        /// <returns>value as string</returns>
        private string GetConfig(string key)
        {
            string value = "";

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                value = appSettings[key] ?? "Not Found";
                return value;
            }
            catch (Exception ex)
            {
                value = "";
                return value;
            }
        }
    }
}
