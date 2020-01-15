using System;
using System.Data;
using NUnit.Framework;

namespace GridBeyondTask.Modules
{
    [TestFixture]
    class AutomatedTesting
    {
        //Setting hardcoded values for testing
        private string databaseFile = @"C:\Kostal\GridBeyondTask\GridBeyondTask\Executable\Database\SampleDatabase.mdf";
        private string databaseServer = @".\SQLEXPRESS";
        private DatabaseAccess da;

        /// <summary>
        /// This Test check if database make good connection with supplied server and database file
        /// </summary>
        [Test, Order(1)]
        public void CheckDatabaseGoodConnection()
        {
            string sResp = "";
            bool bResp = false;

            da = new DatabaseAccess(databaseServer, databaseFile);
            bResp = da.CheckDatabaseConnection(ref sResp);

            Assert.AreEqual(true, bResp);
        }

        /// <summary>
        /// This test check if database fail to connect if supplied wrong server name
        /// </summary>
        [Test, Order(2)]
        public void CheckDatabaseBadConnection()
        {
            string sResp = "";
            bool bResp = false;

            da = new DatabaseAccess("", databaseFile);
            bResp = da.CheckDatabaseConnection(ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(false, bResp);
        }

        /// <summary>
        /// This test check if data added to the table successfully. 
        /// </summary>
        [Test, Order(3)]
        public void AddDataGoodResult()
        { 
            bool bResp = false;

            da = new DatabaseAccess(databaseServer, databaseFile);
            bResp = da.AddDataToTable(DateTime.Now, 0.2f);

            //Compare Response with expected result
            Assert.AreEqual(true, bResp);
        }

        /// <summary>
        /// This test check if function fail to add data into the table 
        /// </summary>
        [Test, Order(4)]
        public void AddDataBadResult()
        {
            bool bResp = false;

            da = new DatabaseAccess("", databaseFile);
            bResp = da.AddDataToTable(DateTime.Now, 0.2f);

            //Compare Response with expected result
            Assert.AreEqual(false, bResp);
        }
        
       
        /// <summary>
        /// This test check if maximum value retreived from table successfully
        /// </summary>
        [Test, Order(5)]
        public void GetMaxMarketValueFromTable_Pass()
        {
            string sResp = "";
            double dMax = 0.0;
            bool bResp = false;

            da = new DatabaseAccess(databaseServer, databaseFile);
            bResp = da.GetMaxMarketPrice(ref dMax, ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(true, bResp);
        }

        /// <summary>
        /// This test check if maximum value fail to retreive from table
        /// </summary>
        [Test, Order(6)]
        public void GetMaxMarketValueFromTable_Fail()
        {
            string sResp = "";
            double dMax = 0.0;
            bool bResp = false;

            da = new DatabaseAccess("", databaseFile);
            bResp = da.GetMaxMarketPrice(ref dMax, ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(false, bResp);
        }

        /// <summary>
        /// This test check if data retrieved successfully
        /// </summary>
        [Test, Order(7)]
        public void RetreiveDataFromTable_Pass()
        {
            string sResp = "";
            DataSet ds = new DataSet();
            bool bResp = false;

            da = new DatabaseAccess(databaseServer, databaseFile);
            bResp = da.RetrieveDataFromTable(ref ds, ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(true, bResp);
        }

        /// <summary>
        /// This test check if data fail to retrieve
        /// </summary>
        [Test, Order(8)]
        public void RetreiveDataFromTable_Fail()
        {
            string sResp = "";
            DataSet ds = new DataSet();
            bool bResp = false;

            da = new DatabaseAccess("", databaseFile);
            bResp = da.RetrieveDataFromTable(ref ds, ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(false, bResp);
        }

        /// <summary>
        /// This test check if data successfully deleted from table
        /// </summary>
        [Test, Order(9)]
        public void DeleteAllDataFromTable_Pass()
        {
            string sResp = "";
            bool bResp = false;

            da = new DatabaseAccess(databaseServer, databaseFile);
            bResp = da.DeleteDataFromTable(ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(true, bResp);
        }

        /// <summary>
        /// This test check if function fail to delete data from table
        /// </summary>
        [Test, Order(10)]
        public void DeleteAllDataFromTable_Fail()
        {
            string sResp = "";
            bool bResp = false;

            da = new DatabaseAccess("", databaseFile);
            bResp = da.DeleteDataFromTable(ref sResp);

            //Compare Response with expected result
            Assert.AreEqual(false, bResp);
        }

    }
}
