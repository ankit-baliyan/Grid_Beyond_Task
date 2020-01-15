using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace GridBeyondTask.Modules
{
    /// <summary>
    /// DatabaseAccess Class contains methods which retreive data 
    /// from the database
    /// Filename:               DatabaseAccess.cs
    /// Type:                   Class
    /// Author:                 Ankit Baliyan
    /// Initial Implementation: Jan 2020          
    /// Changes *:  
    /// </summary>
    class DatabaseAccess
    {
        #region "Variables and Instance Declaration"

        private SqlConnection dbConnection;
        private SqlCommand dbCommand;
        private SqlDataAdapter dbAdapter;
        private string connectionString;

        #endregion

        #region "Constructor"

        /// <summary>
        /// Constructor : Create connection with specified
        /// server, username and password for database.
        /// </summary>
        /// <param name="server">SQL Server</param>
        /// <param name="userName">SQL Server Username</param>
        /// <param name="password">SQL Server Password</param>
        public DatabaseAccess(string server, string userName, string password)
        {
            connectionString = "Initial Catalog=SampleData;User Id="+userName+";Password = "+password+";Data Source="+server+";Trusted_Connection=true";
        }

        /// <summary>
        /// Constructor : Create connection with specified
        /// server & file for database. 
        /// </summary>
        /// <param name="server">SQL Server</param>
        /// <param name="file">filepath with mdb filename</param>
        public DatabaseAccess(string server, string file)
        {
            connectionString = @"Data Source="+server+";Integrated Security=True;Connect Timeout=30;" + "AttachDbFilename =" + file;           
        }

        /// <summary>
        /// Constructor : Create connection with passing
        /// full connection string.
        /// </summary>
        /// <param name="connString">full connection string</param>
        public DatabaseAccess(string connString)
        {
            connectionString = connString;
        }

        #endregion

        /// <summary>
        /// Test database connection
        /// </summary>
        /// <param name="sResp">returns result from function</param>
        /// <returns>boolean</returns>
        public bool CheckDatabaseConnection(ref string sResp)
        {
            sResp = "";
            try
            {
                //Create database connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //open connection
                    dbConnection.Open();
                }
                sResp = "Connection Successful.";
                return true;
            }
            catch(Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Delete all data from table
        /// </summary>
        /// <param name="sResp">return result as string</param>
        /// <returns>boolean</returns>
        public bool DeleteDataFromTable(ref string sResp)
        {
            try
            {
                //Create Query
                string sqlTrunc = "TRUNCATE TABLE sample_data";
                //Create Connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    using (dbCommand = new SqlCommand(sqlTrunc, dbConnection))
                    {
                        //Open Connection
                        dbConnection.Open();
                        //Execute Query
                        dbCommand.ExecuteNonQuery();
                    }
                }
                sResp = "Table deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Add data in the table.
        /// </summary>
        /// <param name="dt">datetime</param>
        /// <param name="marketPrice">float</param>
        /// <returns></returns>
        public bool AddDataToTable(DateTime dt, float marketPrice)
        {
            //Create Query  
            string qry = @"INSERT INTO sample_data(Date, Market_Price_EX1)
                     Values(@Date, @marketPrice)";

            try
            {
                //Create connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //Create command
                    using (dbCommand = new SqlCommand(qry, dbConnection))
                    {
                        //add parameters in to the command
                        dbCommand.Parameters.AddWithValue("@Date", dt);
                        dbCommand.Parameters.AddWithValue("@marketPrice", marketPrice);
                        //open connection
                        dbConnection.Open();
                        //execute query
                        dbCommand.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Retreive Data from the table and return as dataset
        /// </summary>
        /// <param name="sResp">return result as string</param>
        /// <param name="ds">Dataset</param>
        /// <returns>Boolean</returns>
        public bool RetrieveDataFromTable(ref DataSet ds, ref string sResp)
        {
            //Create Query
            string qry = "SELECT Date, Market_Price_EX1 FROM sample_data";
            //Create empty dataset
            ds = new DataSet();
            try
            {
                //create connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //create command
                    using (dbCommand = new SqlCommand(qry, dbConnection))
                    {
                        //create adapter
                        using (dbAdapter = new SqlDataAdapter(dbCommand))
                        {
                            //retrieve data and fill into the dataset
                            dbAdapter.Fill(ds);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime GetExpensiveHours()
        {
            string qry1 = "SELECT DATEADD(hour, DATEDIFF(hour, 0, [Date]), 0) AS Hourly, SUM([Market_Price_EX1]) as MarketPrice "+
                         "FROM sample_data "+
                         "GROUP BY DATEADD(hour, DATEDIFF(hour, 0, [Date]), 0) "+
                         "ORDER BY Hourly";
            DateTime dateTime = new DateTime();
            DataSet ds = new DataSet();
            try
            {
                using (dbConnection = new SqlConnection(connectionString))
                {
                    using (dbCommand = new SqlCommand(qry1, dbConnection))
                    {
                        using (dbAdapter = new SqlDataAdapter(dbCommand))
                        {
                            dbAdapter.Fill(ds);
                        }
                    }
                }

                DataTable dt = ds.Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "MarketPrice DESC";
                dt = dv.ToTable();

                DataRow dr = dt.Rows[0];
                dateTime = Convert.ToDateTime(dr.ItemArray[0]);
                //foreach (DataRow rd in dt.Rows)
                //{
                //   dateTime =  Convert.ToDateTime(rd.ItemArray[0]);
                //}

                return dateTime;
            }
            catch (Exception ex) { return dateTime; }
        }
        
        /// <summary>
        /// Get Max Value from the market price
        /// </summary>
        /// <param name="maxValue">maximum value to return as float</param>
        /// <param name="sResp">result as string</param>
        /// <returns>boolean</returns>
        public bool GetMaxMarketPrice(ref double maxValue, ref string sResp)
        {
            //create query
            string qry = "SELECT MAX(Market_Price_EX1) FROM sample_data";
            //create empty dataset
            DataSet ds = new DataSet();
            try
            {
                //create connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //create command
                    using (dbCommand = new SqlCommand(qry, dbConnection))
                    {
                        //open database connection
                        dbConnection.Open();
                        //execute query and get first value from the result.
                        maxValue = Convert.ToDouble(dbCommand.ExecuteScalar());
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Get Min Value from the market price
        /// </summary>
        /// <param name="minValue">minimum value to return as float</param>
        /// <param name="sResp">result as string</param>
        /// <returns>bool</returns>
        public bool GetMinMarketPrice(ref double minValue, ref string sResp)
        {
            //create query
            string qry = "SELECT MIN(Market_Price_EX1) FROM sample_data";
            //create empty dataset
            DataSet ds = new DataSet();
            try
            {
                //create connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //create command
                    using (dbCommand = new SqlCommand(qry, dbConnection))
                    {
                        //open database connection
                        dbConnection.Open();
                        //execute query and get first value from the result.
                        minValue = Convert.ToDouble(dbCommand.ExecuteScalar());
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Get Average Value from the market price
        /// </summary>
        /// <param name="avgValue">average value to return as float</param>
        /// <param name="sResp">result as string</param>
        /// <returns>boolean</returns>
        public bool GetAvgMarketPrice(ref double avgValue, ref string sResp)
        {
            //create query
            string qry = "SELECT AVG(Market_Price_EX1) FROM sample_data";
            //create empty dataset
            DataSet ds = new DataSet();
            try
            {
                //create connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //create command
                    using (dbCommand = new SqlCommand(qry, dbConnection))
                    {
                        //open database connection
                        dbConnection.Open();
                        //execute query and get first value from the result.
                        avgValue = Convert.ToDouble(dbCommand.ExecuteScalar());
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Create database mdf file
        /// </summary>
        /// <param name="databaseName">dabase name</param>
        /// <param name="databasePath">path to store mdf file</param>
        /// <returns>bool</returns>
        public bool CreateDatabase(string databaseName, string databasePath, ref string sResp)
        {
            //Create Query
            string qry = "CREATE DATABASE "+databaseName+" ON PRIMARY " +
                        "(NAME = Sample_Data, " +
                        "FILENAME = '"+databasePath+"\\"+databaseName+".mdf', " +
                        "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                        "LOG ON (NAME = SampleData_Log, " +
                        "FILENAME = 'C:\\Kostal\\GridBeyondTask\\GridBeyondTask\\Executable\\Database\\SampleDataLog.ldf', " +
                        "SIZE = 1MB, " +
                        "MAXSIZE = 5MB, " +
                        "FILEGROWTH = 10%)";
            try
            {
                //Check if file already exist
                if (!File.Exists(databasePath + "\\" + databaseName + ".mdf"))
                {
                    //create database connection
                    using (dbConnection = new SqlConnection(connectionString))
                    {
                        //create command
                        using (dbCommand = new SqlCommand(qry, dbConnection))
                        {
                            //open database connection
                            dbConnection.Open();
                            //execute query
                            dbCommand.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    //File already exist
                    sResp = "Database file already exist.";
                }
                return true;
            }
            catch (System.Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }
        
        /// <summary>
        /// Create table with columns
        /// </summary>
        /// <returns></returns>
        public bool CreateTable(ref string sResp)
        {
            try
            {
                //create query
                string qry = "CREATE TABLE sample_table(Date datetime, Market_Price_EX1 FLOAT)";
                //create connection
                using (dbConnection = new SqlConnection(connectionString))
                {
                    //create command
                    using (dbCommand = new SqlCommand(qry, dbConnection))
                    {
                        //open connection
                        dbConnection.Open();
                        //execute query
                        dbCommand.ExecuteNonQuery();
                    }
                }
                sResp = "Table created successfully.";
                return true;
            }
            catch(Exception ex)
            {
                //fail message
                sResp = ex.Message;
                return false;
            }
        }
    }
}
