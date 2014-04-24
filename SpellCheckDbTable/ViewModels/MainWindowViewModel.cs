using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using BaseLibrary;
using Caliburn.Micro;
using Ninject;
using SpellCheckDbTable.Properties;
using Utils;

namespace SpellCheckDbTable.ViewModels
{
    public class MainWindowViewModel : Screen
    {
        private BackgroundWorker _worker;
        private readonly ISpellChecker _spellChecker;
        private readonly IIgnoreDictionaryHelper _ignoreDictionaryHelper;
        private int _currentProgress;
        private DataTable _spellCheckResults;
        private bool _isMissSpelledOnly;
        private List<string> _tableNames;
        private List<string> _columnNames = new List<string>();
        private string _tableToSearch;
        private string _columnToSpellCheck;
        private bool _isSpellCheckEnabled;
        private int _missSpellingCount;
        private string _numberOfMissSpellingsLabel;
        private readonly IWindowManager _windowManager;
        private readonly IKernel _kernel;
        private List<string> _dataBaseNames;
        private string _dataBaseToSearch;
        private DataTable _fullSpellCheckedColumn = new DataTable();
        private bool _isBusySpellChecking;
        private bool _hasDataBaseNames;
        private bool _isDatabaseNameSelected;
        private bool _isTableSelected;
        private bool _canConnectToServer;

        public MainWindowViewModel(IKernel kernel, IWindowManager windowManager)
        {
            DisplayName = Resources.DisplayName;
            
            _kernel = kernel;
            _windowManager = windowManager;
            _ignoreDictionaryHelper = new IgnoreDictionaryHelper();
            _spellChecker = new SpellChecker(_ignoreDictionaryHelper);
            IsSpellCheckEnabled = false;
            IsMissSpelledOnly = true;

            InitBackgroundWorker();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ConnectToServer();
        }

        #region public action 
        
        public void SpellCheck()
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("This is a background process");
        }

        public void ViewEditIqnoreList()
        {
            _windowManager.ShowDialog(_kernel.Get<EditIgnoreListViewModel>(), null);
        }

        public void OnSelectionChangedDatabaseName(string databaseName)
        {
            DbConnectionManager.ConnectionManager.DbHandler.TableToSearch = databaseName;
            TableNames = DbConnectionManager.ConnectionManager.DbHandler.GetTableNames();
            IsDatabaseNameSelected = true;
            IsTableSelected = false;
            IsSpellCheckEnabled = false;

            ColumnToSpellCheck = string.Empty;
            ColumnNames.Clear();            
        }

        public void OnSelectionChangedTableName(string tableName)
        {
            ColumnNames = DbConnectionManager.ConnectionManager.DbHandler.GetColumnNames(tableName);
            ColumnToSpellCheck = string.Empty;
            IsTableSelected = true;
            IsSpellCheckEnabled = false;
        }

        public void OnSelectionChangedColumnName()
        {
            IsSpellCheckEnabled = !string.IsNullOrWhiteSpace(ColumnToSpellCheck);
        }

        public void ConnectToServer()
        {
            var dbConnectionViewModel = new DbConnectionViewModel(this);
            _windowManager.ShowDialog(dbConnectionViewModel);

            HasDataBaseNames = DataBaseNames.Any();
            CanConnectToServer = true;
        }

        #endregion

        #region private methods

        private void InitBackgroundWorker()
        {
            _worker = new BackgroundWorker { WorkerReportsProgress = true };
            _worker.DoWork += DoWork;
            _worker.RunWorkerCompleted += WorkCompleted;
            _worker.ProgressChanged += ProgressChanged;
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrentProgress = 0;
            IsSpellCheckEnabled = true;
            CanConnectToServer = true;
            NumberOfMissSpellings = Resources.NumberOfMissSpelling + _missSpellingCount;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            IsSpellCheckEnabled = false;
            CanConnectToServer = false;
            SpellCheckResults = null;
            _fullSpellCheckedColumn = _spellChecker.SpellCheckTable(TableToSearch, ColumnToSpellCheck, _worker);
            SpellCheckResults = _fullSpellCheckedColumn;

            FitlerSpellCheckResults();        
        }

        private void FitlerSpellCheckResults()
        {
            if (IsMissSpelledOnly && SpellCheckResults != null)
            {
                var results = (from row in SpellCheckResults.AsEnumerable().AsParallel()
                               where row.Field<bool>("IsSpelledCorrectly") == false
                               select row);

                if (results.Any())
                {
                    SpellCheckResults = results.CopyToDataTable();
                }

                _missSpellingCount = SpellCheckResults.Rows.Count;
            }
            else
            {
                SpellCheckResults = _fullSpellCheckedColumn;
                _missSpellingCount = _fullSpellCheckedColumn.Rows.Count;
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        #endregion

        #region properties

        public bool CanSpellCheck
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DataBaseToSearch) 
                    && !string.IsNullOrWhiteSpace(TableToSearch)
                    && !string.IsNullOrWhiteSpace(ColumnToSpellCheck);
            }
        }

        public bool IsDatabaseNameSelected
        {
            get { return _isDatabaseNameSelected; }
            set
            {
                _isDatabaseNameSelected = value;
                NotifyOfPropertyChange(() => IsDatabaseNameSelected);
            }
        }

        public bool HasDataBaseNames
        {
            get { return _hasDataBaseNames; }
            set
            {
                _hasDataBaseNames = value;
                NotifyOfPropertyChange(() => HasDataBaseNames);
            }
        }

        public string DataBaseToSearch
        {
            get { return _dataBaseToSearch; }
            set
            {
                _dataBaseToSearch = value;
                NotifyOfPropertyChange(() => DataBaseToSearch);
                NotifyOfPropertyChange(() => CanSpellCheck);
            }
        }

        public List<string> DataBaseNames
        {
            get { return _dataBaseNames ?? (_dataBaseNames = new List<string>()); }
            set
            {
                _dataBaseNames = value;
                NotifyOfPropertyChange(() => DataBaseNames);
            }
        }

        public string TableToSearch
        {
            get { return _tableToSearch; }
            set
            {
                _tableToSearch = value;
                NotifyOfPropertyChange(() => TableToSearch);
                NotifyOfPropertyChange(() => CanSpellCheck);
            }
        }

        public string ColumnToSpellCheck
        {
            get { return _columnToSpellCheck; }
            set
            {
                _columnToSpellCheck = value;
                NotifyOfPropertyChange(() => ColumnToSpellCheck);
                NotifyOfPropertyChange(() => CanSpellCheck);
            }
        }

        public string NumberOfMissSpellings
        {
            get { return _numberOfMissSpellingsLabel; }
            set
            {
                _numberOfMissSpellingsLabel = value;
                NotifyOfPropertyChange(() => NumberOfMissSpellings);
            }
        }

        public string IgnoreListFile
        {
            get
            {
                return _ignoreDictionaryHelper.DoesIgnoreFileExist()
                  ? Resources.IgnoreListFound
                  : Resources.IgnoreListNotFound;
            }
        }

        public bool IsMissSpelledOnly
        {
            get { return _isMissSpelledOnly; }
            set
            {
                _isMissSpelledOnly = value;
                FitlerSpellCheckResults();
                NotifyOfPropertyChange(() => IsMissSpelledOnly);
            }
        }

        public DataTable SpellCheckResults
        {
            get { return _spellCheckResults ?? (_spellCheckResults = new DataTable()); }
            set
            {
                _spellCheckResults = value;
                NotifyOfPropertyChange(() => SpellCheckResults);
            }
        }

        public List<string> TableNames
        {
            get
            {
                return _tableNames ?? (_tableNames = new List<string>());
            }
            set
            {
                _tableNames = value;
                NotifyOfPropertyChange(() => TableNames);
            }
        }

        public List<string> ColumnNames
        {
            get
            {
                return _columnNames ?? (_columnNames = new List<string>());
            }
            set
            {
                _columnNames = value;
                NotifyOfPropertyChange(() => ColumnNames);
            }
        }

        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                NotifyOfPropertyChange(() => CurrentProgress);
            }
        }

        public bool IsSpellCheckEnabled
        {
            get { return _isSpellCheckEnabled; }
            set
            {
                _isSpellCheckEnabled = value;
                NotifyOfPropertyChange(() => IsSpellCheckEnabled);
            }
        }

        public bool IsBusySpellChecking
        {
            get { return _isBusySpellChecking; }
            set
            {
                _isBusySpellChecking = value;
                NotifyOfPropertyChange(() => IsBusySpellChecking);
            }
        }

        public bool IsTableSelected
        {
            get { return _isTableSelected; }
            set
            {
                _isTableSelected = value;
                NotifyOfPropertyChange(() => IsTableSelected);
            }
        }

        public bool CanConnectToServer
        {
            get { return _canConnectToServer; }
            set
            {
                _canConnectToServer = value;
                NotifyOfPropertyChange(() => CanConnectToServer);
            }
        }

        #endregion properties
    }
}