using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using BaseLibrary;

namespace DbHandler
{
    public class DbHandler : IDbHandler
    {
        public DbHandler()
        {
            DbConnectionString = ConfigurationManager.AppSettings["ConnString"];
            if (!IsValidConnectionString(DbConnectionString))
                MessageBox.Show("Connection string in app.config was not configured correctly. App will shutdown when you click ok.");
        }

        public DbHandler(string dbConnectionString, string dbUserName, string dbPassword)
        {
            DbConnectionString = dbConnectionString;
            DbUserName = dbUserName;
            DbPassword = dbPassword;
        }

        private bool IsValidConnectionString(string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                return ValidConnectionString = false;

            try
            {
                using (IDbConnection connection = DbConnection)
                {
                    connection.Open();
                    connection.Close();
                    return ValidConnectionString = true;
                }
            }
            catch (Exception)
            {
                return ValidConnectionString = false;
            }
        }

        public bool ValidConnectionString { get; set; }

        public DbConnection DbConnection { get { return new SqlConnection(DbConnectionString); } }

        public string DbConnectionString { get; set; }

        public string DbUserName { get; set; }

        public string DbPassword { get; set; }

        public string TableToSearch { get; set; }

        public string ColumnToSpellChecker { get; set; }

        public string DbCommandText { get; set; }

        public List<string> GetTableNames()
        {
            using (IDbConnection connection = DbConnection)
            {
                connection.Open();

                DataSet dataSet = CreateDataSet("Select name FROM sys.tables ORDER BY name ", null, connection);
                if (dataSet.Tables.Count == 0)
                    return null;
                List<string> tblnames = (from tableNames in dataSet.Tables[0].AsEnumerable()
                                         select tableNames.Field<string>("name")).ToList();
                return tblnames;
            }
        }

        public List<string> GetColumnNames(string tableName)
        {
            using (IDbConnection connection = DbConnection)
            {
                connection.Open();

                DataSet dataSet = CreateDataSet("SELECT column_name FROM information_schema.columns WHERE table_name = '" + tableName + "' ORDER BY column_name", null, connection);
                if (dataSet.Tables.Count == 0)
                    return null;

                List<string> columnNames = (from tableNames in dataSet.Tables[0].AsEnumerable()
                                            select tableNames.Field<string>("column_name")).ToList();

                return columnNames;
            }
        }

        public DataTable GetRows(string tableToSearch, string columnToSpellCheck)
        {
            if (string.IsNullOrWhiteSpace(tableToSearch) || string.IsNullOrWhiteSpace(columnToSpellCheck))
                return new DataTable();

            using (IDbConnection connection = DbConnection)
            {
                connection.Open();

                DataSet dataSet = CreateDataSet("SELECT * FROM " + tableToSearch, null, connection);
                if (dataSet.Tables.Count == 0)
                    return null;

                int columnCounter = 0;
                while (columnCounter < dataSet.Tables[0].Columns.Count)
                {
                    if (dataSet.Tables[0].Columns[columnCounter].ColumnName.Contains("_id")
                        || dataSet.Tables[0].Columns[columnCounter].ColumnName.Contains(columnToSpellCheck))
                    {
                        columnCounter++;
                    }
                    else
                    {
                        dataSet.Tables[0].Columns.Remove(dataSet.Tables[0].Columns[columnCounter].ColumnName);
                    }
                }

                return dataSet.Tables[0];
            }
        }

        private DataSet CreateDataSet(string commandText, Dictionary<string, object> parameters, IDbConnection connection)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                return new DataSet();

            using (IDbCommand dbCommand = CreateQueryCommand(commandText, parameters, connection))
            {
                DataTable dataTable = new DataTable();

                using (IDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SingleResult))
                {
                    try
                    {
                        dataTable.Load(reader, LoadOption.OverwriteChanges);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);

                return dataSet;
            }
        }

        private IDbCommand CreateQueryCommand(string commandText, Dictionary<string, object> parameters, IDbConnection connection)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            command.Transaction = connection.BeginTransaction();

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                    command.Parameters.Add(CreateParameter(key, parameters[key]));
            }

            return command;
        }

        private IDbDataParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }
    }
}