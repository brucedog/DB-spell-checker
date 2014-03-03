using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Caliburn.Micro;
using SpellCheckDbTable.Views;

namespace SpellCheckDbTable.ViewModels
{
    public class DbConnectionViewModel : Screen
    {

        public void Connect(DbConnectionView view)
        {
            SqlConnection getConnection = null;
            try
            {
                getConnection = new SqlConnection(BuildConectionString(view));
                DataTable databases = new DataTable("Databases");

                using (IDbConnection connection = getConnection)
                {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM sys.Databases";
                    connection.Open();
                    databases.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error while loading available databases", ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading available databases", ex.Message);
            }
            finally
            {
                if (getConnection != null)
                    getConnection.Close();
            }
        }

        private string BuildConectionString(DbConnectionView view)
        {
            string userAuth = GetUserAuth(view);
            return string.Format("Data Source={0};", view.ServerName.Text.Trim() + userAuth);
        }

        private string GetUserAuth(DbConnectionView view)
        {
            if (view.AuthType.Equals("Windows Authentication"))
            {
                return string.Format("user Id={0};Password={1};", view.Username.Text.Trim(), view.PasswordBox.Password.Trim());
            }
            
            return string.Format("Integrated Security=True;");
        }
    }
}