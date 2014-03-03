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
        private int _missSpellingCount;
        private string _numberOfMissSpellingsLabel;
        private readonly IWindowManager _windowManager;
        private readonly IKernel _kernel;

        public MainWindowViewModel(IKernel kernel, IWindowManager windowManager)
        {
            DisplayName = Resources.DisplayName;
             _kernel = kernel;
            _windowManager = windowManager;
            _ignoreDictionaryHelper = new IgnoreDictionaryHelper();
            _spellChecker = new SpellChecker(_dbHandler, _ignoreDictionaryHelper);
            IsSpellCheckEnabled = false;
            IsMissSpelledOnly = true;
        }

        #region public action 
        
        
        public void BeginSpellCheck()
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("This is a background process");
        }

        public void ViewEditIqnoreList()
        {
            _windowManager.ShowDialog(_kernel.Get<EditIgnoreListViewModel>(), null);
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
            NumberOfMissSpellings = Resources.NumberOfMissSpelling + _missSpellingCount;
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

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        #endregion

        #region properties

        public string TableToSearch
        {
            get { return _tableToSearch; }
            set
            {
                _tableToSearch = value;
                IsSpellCheckEnabled = false;
                NotifyOfPropertyChange(() => TableToSearch);
                NotifyOfPropertyChange(() => ColumnNames);
            }
        }

        public string ColumnToSpellCheck
        {
            get { return _columnToSpellCheck; }
            set
            {
                _columnToSpellCheck = value;
                IsSpellCheckEnabled = true;
                NotifyOfPropertyChange(() => ColumnToSpellCheck);
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
                NotifyOfPropertyChange(() => IsMissSpelledOnly);
            }
        }

        public DataTable SpellCheckResults
        {
            get { return _spellCheckResults; }
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
                return _tableNames;
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
                return _columnNames;
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

        #endregion properties
    }
}