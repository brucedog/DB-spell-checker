using System.Windows.Controls;
using Caliburn.Micro;
using SpellCheckDbTable.Properties;
using SpellCheckDbTable.Views;
using Utils;

namespace SpellCheckDbTable.ViewModels
{
    public class DbConnectionViewModel : Screen
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private bool _isSqlServerAuthenticationSelected;
        private string _serverName;

        public DbConnectionViewModel(MainWindowViewModel mainWindowViewModel)
        {
            DisplayName = Resources.DatabaseConection;
            _mainWindowViewModel = mainWindowViewModel;
            IsSqlServerAuthenticationSelected = false;
            ServerName = "localhost";
        }

        #region public actions

        public void Connect(DbConnectionView view)
        {
            DbConnectionManager.ConnectionManager.DbHandler = new MsSqlDbHandler.MsSqlDbHandler(BuildConectionString(view));
            _mainWindowViewModel.DataBaseNames = DbConnectionManager.ConnectionManager.DbHandler.GetDatabases();

            Close();
        }

        public void OnSelectionChangedAction(ComboBoxItem comboBoxItem)
        {
            IsSqlServerAuthenticationSelected = comboBoxItem.Content.Equals("SQL Server Authentication");            
        }

        public void OnSelectionChangedServerType(ComboBoxItem comboBoxItem)
        {
            //TODO get proper connection type
            //_connection = new MsSqlDbHandler.MsSqlDbHandler(BuildConectionString(view));
        }

        public void Close()
        {
            TryClose();
        }

        #endregion

        #region properties

        public string ServerName
        {
            get { return _serverName; }
            set
            {
                _serverName = value;
                NotifyOfPropertyChange(() => ServerName);
            }
        }

        public bool IsSqlServerAuthenticationSelected
        {
            get { return _isSqlServerAuthenticationSelected; }
            set
            {
                _isSqlServerAuthenticationSelected = value;
                NotifyOfPropertyChange(() => IsSqlServerAuthenticationSelected);
            }
        }

        #endregion

        #region private methods

        private string BuildConectionString(DbConnectionView view)
        {
            string userAuth = GetUserAuth(view);
            return string.Format(@"Data Source={0};{1}", ServerName.Trim(), userAuth);
        }

        private string GetUserAuth(DbConnectionView view)
        {
            if (!view.AuthType.Text.Equals("Windows Authentication"))
            {
                return string.Format("user Id={0};Password={1};", view.Username.Text.Trim(), view.PasswordBox.Password.Trim());
            }

            return string.Format("Integrated Security=True;");
        }

        #endregion
    }
}