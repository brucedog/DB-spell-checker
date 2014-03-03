using System.Collections;

namespace BaseLibrary
{
    public interface IIgnoreDictionaryHelper
    {
        void BuildIgnoreList();

        /// <summary>
        /// Gets the ignore list.
        /// </summary>
        /// <value>
        /// The ignore list.
        /// </value>
        Hashtable IgnoreList { get; }

        /// <summary>
        /// Doeses the ignore file exist.
        /// </summary>
        /// <returns></returns>
        bool DoesIgnoreFileExist();

        /// <summary>
        /// Saves the ignore list.
        /// </summary>
        /// <param name="ignoreList">The ignore list.</param>
        void SaveIgnoreList(string ignoreList);
    }
}