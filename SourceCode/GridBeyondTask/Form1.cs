using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GridBeyondTask.Modules;

namespace GridBeyondTask
{
    public partial class frmSampleDataProcessing : Form
    {
        //Declare Database Access
        private DatabaseAccess da;
        //Declare cSettings
        private cSettings configSettings;

        public frmSampleDataProcessing()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Open Application in full screen
            this.WindowState = FormWindowState.Maximized;

            //check & Create Directories for SW if not exists.
            CreateDirectory(System.Environment.CurrentDirectory + @"\CSV Files");
            CreateDirectory(System.Environment.CurrentDirectory + @"\Database");

            //File Dropdown list with csv file exist in directory
            FillBox();

            configSettings = new cSettings();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string sResp = "";
            //Start Process
            if (!StartProcessing(ref sResp)) { ShowFailMessage(sResp); }
            else
            {
                resultChart.Visible = true;
                ShowPassMessage("File processed successfully.");
            }
        }

        /// <summary>
        /// Start process: read csv file, retreive data,
        /// add data to the sql database, retreive data,
        /// show in graph.
        /// </summary>
        /// <param name="sResp">return result as string</param>
        /// <returns>boolean</returns>
        private bool StartProcessing(ref string sResp)
        {
            bool processOK = true;
            DataTable readData = new DataTable();
            if (ExistingFileList.Items.Count > 0)
            {
                //Read data from csv file
                readData = ReadCsvFile(System.Environment.CurrentDirectory + @"\CSV Files\" + ExistingFileList.SelectedItem.ToString());

                //Create Database Connection
                da = new DatabaseAccess(configSettings.DatabaseServer, System.Environment.CurrentDirectory + configSettings.DatabaseFilePath);
            }
            else { processOK = false; sResp = "No File Selected by user."; return false; }
            
            //Check Database Connection
            if (da.CheckDatabaseConnection(ref sResp))
            {
                //Delete current data from the table.
                if (!da.DeleteDataFromTable(ref sResp)) { sResp = "Fail to clear data from the existing table."; processOK = false; }

                //Add Data from CSV to Database
                if (processOK)
                {
                    //Loop through the data rows
                    foreach (DataRow rd in readData.Rows)
                    {
                        //convert retrieved data to float
                        float fNum = float.Parse(rd.ItemArray[1].ToString());
                        //convert retrieved data to dataetime format
                        DateTime dt = Convert.ToDateTime(rd.ItemArray[0]);
                        //add converted data to the table
                        da.AddDataToTable(dt, fNum);
                    }
                }

                //create dataset and datatable
                DataSet dataSet = new DataSet();
                DataTable dataTable;

                //retreive data from table and fill it in datatable
                if (processOK)
                {
                    //Retrieve data from the table, passsing empty dataset
                    if (!da.RetrieveDataFromTable(ref dataSet, ref sResp)) { processOK = false; }
                    else
                    {
                        //if data reteive from table pass, fill it in empty datatable
                        dataTable = dataSet.Tables[0]; }
                }
                
                // Declare an object variable.  
                //object AvgTotal = dataTable.Compute("Avg(Market_Price_EX1)", "");
                //object Min = dataTable.Compute("Min(Market_Price_EX1)", "");
                //object Max = dataTable.Compute("Max(Market_Price_EX1)", "");

                //set graph chart datasource to the retreived data from database
                if (processOK)
                {
                    resultChart.DataSource = dataSet;
                    //Set Chart X-Values
                    resultChart.Series["DataSampleSeries"].XValueMember = "Date";
                    //Set Chart Y-Values
                    resultChart.Series["DataSampleSeries"].YValueMembers = "Market_Price_EX1";
                    //Set Chart Title
                    resultChart.Titles.Add("Market Price By Time");
                }

                //Retreive Maximum Value from the database for the "Market Price"
                double maxVal = 0.0;
                if (processOK)
                {
                    if (!da.GetMaxMarketPrice(ref maxVal, ref sResp)) { processOK = false; }
                }

                //Retreive Minimum Value from the database for the "Market Price"
                double minVal = 0.0;
                if (processOK)
                {
                    if (!da.GetMinMarketPrice(ref minVal, ref sResp)) { processOK = false; }
                }

                //Retreive Average Value from the database for the "Market Price"
                double avgVal = 0.0;
                if (processOK)
                {
                    if (!da.GetAvgMarketPrice(ref avgVal, ref sResp)) { processOK = false; }
                }

                //Get Most Expensive Date & Hour
                DateTime expensiveHour = da.GetExpensiveHours();

                //Set Maximum, Minimum & Average Market Price in the labels
                if (processOK)
                {
                    lblMin.Text = minVal.ToString();
                    lblMax.Text = maxVal.ToString();
                    lblAverage.Text = avgVal.ToString();
                    lblMostExpensiveHour.Text = expensiveHour.ToString();
                }
            }
            else
            {
                //database connection fail
                processOK = false;
                sResp = "Fail to Connect with Database." + sResp;
            }
            return processOK;
        }

        /// <summary>
        /// Read CSV file, process data and return back as datatable
        /// </summary>
        /// <param name="fullpath">filename & file location</param>
        /// <returns>DataTable</returns>
        private DataTable ReadCsvFile(string fullpath)
        {
            //create empty datatable
            DataTable dt = new DataTable();
            try
            {
                //read data from csv file
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    //get first row and split to add it as header
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        //add header to the datatable
                        dt.Columns.Add(header);
                    }
                    //loop till the end of the file
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        //adding rows into the datatable
                        dt.Rows.Add(dr);
                    }
                }
                //return new datatable
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        /// <summary>
        /// Check if directory exists or not.
        /// If not then create.
        /// </summary>
        /// <param name="directoryPath">directory path</param>
        private void CreateDirectory(string directoryPath)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(directoryPath))
                { // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(directoryPath);
                }
            }
            catch (Exception ex)
            {}
        }


        /// <summary>
        /// Get string array list of the csv files exist in specified directory.
        /// </summary>
        /// <param name="filePath">directory loaction where csv files stored</param>
        /// <returns>string array</returns>
        public string[] GetExistingCSVFiles(string filePath)
        {
            //Creating string List
            List<String> fileNames = new List<string>();
            try
            {
                //Get directory info
                DirectoryInfo di = new DirectoryInfo(filePath);
                //Get files exist in directory
                FileInfo[] files = di.GetFiles("*.csv");

                //read all files and add to the list
                foreach (FileInfo file in files)
                {
                    fileNames.Add(file.Name);
                }
                //conver to string array and return
                return fileNames.ToArray();
            }
            catch (Exception ex)
            {
                return fileNames.ToArray(); ;
            }
        }


        /// <summary>
        /// Fill dropdown box with csv files and select first.
        /// </summary>
        private void FillBox()
        {
            try
            {

                ExistingFileList.Items.Clear();
                string[] existingFiles = GetExistingCSVFiles(System.Environment.CurrentDirectory + @"\CSV Files\");
                if (existingFiles.Length > 0)
                {
                    ExistingFileList.Items.AddRange(existingFiles);
                    ExistingFileList.SelectedIndex = 0;
                }
                else
                {
                    ShowFailMessage("No File Exist in CSV File directory.");
                }
            }
            catch (Exception ex)
            {
                ShowFailMessage("Error Retreiving CSV Files: " + ex.Message);
            }
        }


        /// <summary>
        /// show user fail message in message box
        /// </summary>
        /// <param name="msg"></param>
        private void ShowFailMessage(string msg)
        {
            txtMessageBox.BackColor = Color.DarkOrange;
            txtMessageBox.Text = msg;
        }

        /// <summary>
        /// show user pass message in message box
        /// </summary>
        /// <param name="msg"></param>
        private void ShowPassMessage(string msg)
        {
            txtMessageBox.BackColor = Color.GreenYellow;
            txtMessageBox.Text = msg;
        }
    }
}
