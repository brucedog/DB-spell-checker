using System.Collections;

namespace BaseLibrary
{
    public interface IIgnoreDictionaryHelper
    {
        void BuildIgnoreList();

        Hashtable IgnoreList { get; }

        bool DoesIgnoreFileExist();
    }
}