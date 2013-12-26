using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary;

namespace SpellCheckDbTable
{
    /// <summary>
    /// Class takes a text file and build it into a list for the spell checker to ignore if found.
    /// </summary>
    public class IgnoreDictionaryHelper : IIgnoreDictionaryHelper
    {
        Hashtable _ignoreList = new Hashtable();

        public IgnoreDictionaryHelper()
        {
            BuildIgnoreList();
        }

        public void BuildIgnoreList()
        {
            if (!DoesIgnoreFileExist())
                return;

            List<string> tempList = new List<string>();
            using (StreamReader streamReader = new StreamReader(Directory.GetCurrentDirectory() + "\\ignoreList.txt"))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    tempList.Add(line);
                }
            }

            // removed duplicate entries of words that should be ignored by the spell checker.
            var unquieTempList = tempList.Distinct();
            foreach (var item in unquieTempList)
            {
                IgnoreList.Add(item, item);
            }
        }

        public bool DoesIgnoreFileExist()
        {
            return File.Exists(Directory.GetCurrentDirectory() + "\\ignoreList.txt");
        }

        public Hashtable IgnoreList
        {
            get { return _ignoreList; }
            private set { _ignoreList = value; }
        }
    }
}