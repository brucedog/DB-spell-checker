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
        private Hashtable _ignoreList = new Hashtable();
        private const string FileName = "\\ignoreList.txt";        

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
            string directory = Directory.GetCurrentDirectory() + FileName;
            using (StreamReader streamReader = new StreamReader(directory, Encoding.ASCII))
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
                if(!IgnoreList.ContainsKey(item))
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
            string directory = Directory.GetCurrentDirectory() + FileName;
            return File.Exists(directory);
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
                string directory = Directory.GetCurrentDirectory() + FileName;
                using (StreamWriter outfile = new StreamWriter(directory, false, Encoding.ASCII))
                {
                    outfile.Write(ignoreList);
                }
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
                string directory = Directory.GetCurrentDirectory() + FileName;
                using (StreamWriter outfile = new StreamWriter(directory))
                {
                    outfile.Write(string.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating ignore list file");
            }
        }

        #endregion
    }
}