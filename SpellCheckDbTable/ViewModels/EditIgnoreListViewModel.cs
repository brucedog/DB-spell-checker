using BaseLibrary;
using Caliburn.Micro;

namespace SpellCheckDbTable.ViewModels
{
    /// <summary>
    /// Class allows user to view and edit a list of words/miss spellings
    /// </summary>
    public class EditIgnoreListViewModel : Screen
    {
        private string _ignoreList;
        private string _orginalList = string.Empty;
        private readonly IIgnoreDictionaryHelper _ignoreDictionaryHelper;

        public EditIgnoreListViewModel(IIgnoreDictionaryHelper ignoreDictionaryHelper)
        {
            _ignoreDictionaryHelper = ignoreDictionaryHelper;
            PopulateIgnoreList();
        }

        #region public actions

        public void SaveIgnoreList()
        {
            _ignoreDictionaryHelper.SaveIgnoreList(IgnoreList);
        }

        public void Close()
        {
            TryClose(_orginalList.Equals(IgnoreList));
        }

        #endregion

        #region private methods


        private void PopulateIgnoreList()
        {
            if (_ignoreDictionaryHelper.DoesIgnoreFileExist())
            {
                IgnoreList = _ignoreDictionaryHelper.ToString();
                _orginalList = IgnoreList;
            }
        }

        #endregion

        #region properties

        public string IgnoreList
        {
            get { return _ignoreList; }
            set
            {
                _ignoreList = value;
                NotifyOfPropertyChange(IgnoreList);
            }
        }

        #endregion
    }
}