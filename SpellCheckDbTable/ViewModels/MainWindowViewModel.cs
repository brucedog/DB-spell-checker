using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using BaseLibrary;
using SpellCheckDbTable.Utils;
using ViewModelBaseLibrary;
using ViewModelBaseLibrary.Commands;

namespace SpellCheckDbTable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private RelayCommand _spellCheckCommand;
        private RelayCommand _instigateWorkCommand;
        private BackgroundWorker _worker;
        private readonly ISpellChecker _spellChecker;
        private readonly IDbHandler _dbHandler;
        private readonly IIgnoreDictionaryHelper _ignoreDictionaryHelper;
        private int _currentProgress;
        private DataTable _spellCheckResults;
        private bool _isMissSpelledOnly;
        private List<string> _tableNames;
        private List<string> _columnNames = new List<string>();
        private string _tableToSearch;
        private string _columnToSpellCheck;
        private bool _isSpellCheckEnabled;
        private const string NumberOfMisspling = "Number of Miss Spellings: ";
        private int _missSpellingCount;
        private string _numberOfMissSpellingsLabel;

        public MainWindowViewModel()
        {
            _dbHandler = new DbHandler.DbHandler();
            _ignoreDictionaryHelper = new IgnoreDictionaryHelper();
            if (!_dbHandler.ValidConnectionString)
                Process.GetCurrentProcess().Kill();
            _spellChecker = new SpellChecker(_dbHandler, _ignoreDictionaryHelper);
            InitBackgroundWorker();
            IsSpellCheckEnabled = false;
            IsMissSpelledOnly = true;
            NumberOfMissSpellings = NumberOfMisspling;
        }

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
            NumberOfMissSpellings = NumberOfMisspling + _missSpellingCount;
        }

        public RelayCommand SpellCheck { get { return _spellCheckCommand ?? (_spellCheckCommand = new RelayCommand(param => BeginSpellCheck())); } }

        private void BeginSpellCheck()
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("This is a background process");
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            IsSpellCheckEnabled = false;
            SpellCheckResults = null;
            DataTable temp = _spellChecker.SpellCheckTable(TableToSearch, ColumnToSpellCheck, _worker);
            if (IsMissSpelledOnly && temp != null)
            {
                SpellCheckResults = (from row in temp.AsEnumerable().AsParallel()
                                     where row.Field<bool>("IsSpelledCorrectly") == false
                                     select row).CopyToDataTable();

                _missSpellingCount = SpellCheckResults == null ? 0 : SpellCheckResults.Rows.Count;
            }
            else
            {
                SpellCheckResults = temp;

                _missSpellingCount = (from row in temp.AsEnumerable().AsParallel()
                                      where row.Field<bool>("IsSpelledCorrectly") == false
                                      select row).CopyToDataTable().Rows.Count;
            }
        }

        public RelayCommand InstigateWorkCommand
        {
            get { return _instigateWorkCommand ?? (_instigateWorkCommand = new RelayCommand(o => _worker.RunWorkerAsync(), o => !_worker.IsBusy)); }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        #region properties

        public string TableToSearch
        {
            get { return _tableToSearch; }
            set
            {
                _tableToSearch = value;
                IsSpellCheckEnabled = false;
                NotifyPropertyChanged(() => TableToSearch);
                NotifyPropertyChanged(() => ColumnNames);
            }
        }

        public string ColumnToSpellCheck
        {
            get { return _columnToSpellCheck; }
            set
            {
                _columnToSpellCheck = value;
                IsSpellCheckEnabled = true;
                NotifyPropertyChanged(() => ColumnToSpellCheck);
            }
        }

        public string NumberOfMissSpellings
        {
            get { return _numberOfMissSpellingsLabel; }
            set
            {
                _numberOfMissSpellingsLabel = value;
                NotifyPropertyChanged(() => NumberOfMissSpellings);
            }
        }

        public string IgnoreListFile
        {
            get
            {
                return _ignoreDictionaryHelper.DoesIgnoreFileExist()
                ? "List of words to ignore found."
                : "No list of words to ignore for database spell checker.";
            }
        }

        public bool IsMissSpelledOnly
        {
            get { return _isMissSpelledOnly; }
            set
            {
                _isMissSpelledOnly = value;
                NotifyPropertyChanged(() => IsMissSpelledOnly);
            }
        }

        public DataTable SpellCheckResults
        {
            get { return _spellCheckResults; }
            set
            {
                _spellCheckResults = value;
                NotifyPropertyChanged(() => SpellCheckResults);
            }
        }

        public List<string> TableNames
        {
            get
            {
                return _tableNames ?? (_tableNames = _dbHandler.GetTableNames());
            }
            set
            {
                _tableNames = value;
                NotifyPropertyChanged(() => TableNames);
            }
        }

        public List<string> ColumnNames
        {
            get
            {
                return (_columnNames = string.IsNullOrWhiteSpace(TableToSearch)
                    ? new List<string>()
                    : _dbHandler.GetColumnNames(TableToSearch));
            }
            set
            {
                _columnNames = value;
                NotifyPropertyChanged(() => ColumnNames);
            }
        }

        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                NotifyPropertyChanged(() => CurrentProgress);
            }
        }

        public bool IsSpellCheckEnabled
        {
            get { return _isSpellCheckEnabled; }
            set
            {
                _isSpellCheckEnabled = value;
                NotifyPropertyChanged(() => IsSpellCheckEnabled);
            }
        }

        #endregion properties
    }
}