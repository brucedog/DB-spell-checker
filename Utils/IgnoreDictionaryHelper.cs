using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace Utils
{
    /// <summary>
    /// Class takes a text file and build it into a list for the spell checker to ignore if found.
    /// </summary>
    public class IgnoreDictionaryHelper : IIgnoreDictionaryHelper
    {
        Hashtable _ignoreList = new Hashtable();

        public IgnoreDictionaryHelper()
        {
            if (!DoesIgnoreFileExist())
                CreateIgnoreFile();

            BuildIgnoreList();
        }

#region public methods

        public void BuildIgnoreList()
        {
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
            var unquieTempList = tempList.AsParallel().Distinct();
            foreach (var item in unquieTempList)
            {
                IgnoreList.Add(item, item);
            }
        }

        public override string ToString()
        {
            IDictionaryEnumerator enumerator = _ignoreList.GetEnumerator();
            StringBuilder stringBuilder = new StringBuilder();
            while (enumerator.MoveNext())
            {
                stringBuilder.AppendLine(enumerator.Value.ToString());
            }
            return stringBuilder.ToString();
        }

        public bool DoesIgnoreFileExist()
        {
            return File.Exists(Directory.GetCurrentDirectory() + "\\ignoreList.txt");
        }

        public void SaveIgnoreList(string ignoreList)
        {
            string[] tempList = ignoreList.Split('\n');
            // removed duplicate entries of words that should be ignored by the spell checker.
            var unquieTempList = tempList.AsParallel().Distinct();
            Hashtable tempIgnoreList = new Hashtable();
            foreach (var item in unquieTempList)
            {
                tempIgnoreList.Add(item, item);
            }

            if (SaveIgnoreListToDisk(ignoreList))
            {
                IgnoreList.Clear();
                IgnoreList = tempIgnoreList;
            }
        }

        #endregion

        public Hashtable IgnoreList
        {
            get { return _ignoreList; }
            private set { _ignoreList = value; }
        }

        #region private methods

        private bool SaveIgnoreListToDisk(string ignoreList)
        {
            try
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\ignoreList.txt", ignoreList);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving ignore list file to disk");
                return false;
            }
        }

        private void CreateIgnoreFile()
        {
            try
            {
                File.Create(Directory.GetCurrentDirectory() + "\\ignoreList.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating ignore list file");
            }
        }

        #endregion
    }
}