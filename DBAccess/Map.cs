using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Windows.Forms;

namespace DBAccess
{
    public class Map
    {
        //"Data Source=compaqserver;User ID=admcont;Password=vadmcont;Unicode=True"

        public bool GetColumns(string connectionString, string tableName, string outputLocation, string projectName, string modelName)
        {
            try
            {
                List<string> FieldNames = new List<string>();
                Dictionary<string, string> dicNames = new Dictionary<string, string>();


                OracleConnection oraConn = new OracleConnection();

                OracleCommand oraCmd = new OracleCommand();

                DataTable schemaTable;
                OracleDataReader oraReader;

                oraConn.ConnectionString = connectionString;

                oraConn.Open();

                oraCmd.Connection = oraConn;

                oraCmd.CommandText = "SELECT column_name, data_type FROM USER_TAB_COLUMNS WHERE table_name = '" + tableName + "'";
                oraReader = oraCmd.ExecuteReader();

                string type = "";

                if (oraReader.HasRows)
                {
                    while (oraReader.Read())
                    {
                        if (oraReader.GetString(1).Equals("NUMBER"))
                        {
                            type = "int";
                        }
                        else
                        {
                            type = "string";
                        }
                        dicNames.Add(oraReader.GetString(0), type);
                    }
                }

                ConstructFile(outputLocation, dicNames, projectName, modelName);

                oraReader.Close();
                oraConn.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string SelectFolder()
        {
            FolderBrowserDialog fbdFolderSearch = new FolderBrowserDialog();

            fbdFolderSearch.ShowDialog();

            return fbdFolderSearch.SelectedPath;
        }

        public List<string> GetTableNames(string connectionString)
        {
            try
            {
                List<string> TableNames = new List<string>();

                OracleConnection oraConn = new OracleConnection();

                OracleCommand oraCmd = new OracleCommand();

                DataTable schemaTable;
                OracleDataReader oraReader;

                oraConn.ConnectionString = connectionString;

                oraConn.Open();

                oraCmd.Connection = oraConn;

                oraCmd.CommandText = "SELECT table_name FROM dba_tables where owner = 'ADMCONT' order by table_name"; //Substitute ADMCONT for your owner
                oraReader = oraCmd.ExecuteReader();

                if (oraReader.HasRows)
                {
                    while (oraReader.Read())
                    {
                        TableNames.Add(oraReader.GetString(0));
                    }
                }

                oraReader.Close();
                oraConn.Close();

                return TableNames;
            }
            catch (Exception)
            {
                List<string> list = new List<string>();

                return list;
            }
            
        }

        private void ConstructFile(string outputLocation, Dictionary<string, string> dicNames, string projectName, string  modelName)
        {
            outputLocation = outputLocation + "\\" + modelName + ".cs";

            string header = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", "using System;", Environment.NewLine,
                                                                                    "using System.Collections.Generic;", Environment.NewLine,
                                                                                    "using System.Linq;", Environment.NewLine,
                                                                                    "using System.Web;", Environment.NewLine,
                                                                                    Environment.NewLine);

            string openKey = "{";
            string closeKey = "}";
            string nameSpace = string.Format("{0} {1}{2}{3}{4}", "namespace", projectName + ".Models", Environment.NewLine, openKey, Environment.NewLine);
            string className = string.Format("{0} {1}{2}{3}{4}", "public class", modelName, Environment.NewLine, openKey, Environment.NewLine);

            File.AppendAllText(outputLocation, header);
            File.AppendAllText(outputLocation, nameSpace);
            File.AppendAllText(outputLocation, className);

            foreach (var item in dicNames)
            {
                File.AppendAllText(outputLocation, "public " + item.Value + " " + item.Key + " {get; set; }" + Environment.NewLine);
            }

            File.AppendAllText(outputLocation, Environment.NewLine + closeKey);
            File.AppendAllText(outputLocation, Environment.NewLine + closeKey);
        }

    }
}
