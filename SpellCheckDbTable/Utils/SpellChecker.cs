using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using BaseLibrary;

namespace SpellCheckDbTable.Utils
{
    /// <summary>
    /// Class is used to spell check word from a database using the nhunspell.
    /// </summary>
    public class SpellChecker : ISpellChecker
    {
        private readonly IIgnoreDictionaryHelper _ignoreDictionaryHelper;

        public SpellChecker(IDbHandler dbHandler, IIgnoreDictionaryHelper ignoreDictionaryHelper)
        {
            Db = dbHandler;
            _ignoreDictionaryHelper = ignoreDictionaryHelper;
        }

        public DataTable SpellCheckTable(string tableToSpellCheck)
        {
            return SpellCheckTable(tableToSpellCheck, string.Empty, null);
        }

        private IDbHandler Db { get; set; }

        public DataTable SpellCheckTable(string tableToSpellCheck, string columnToSpellCheck, BackgroundWorker worker)
        {
            DataTable dataTable = Db.GetRows(tableToSpellCheck, columnToSpellCheck);
            if (dataTable.Rows.Count < 1)
                return null;

            int rowCounter = 0;

            NHunspell.Hunspell hunspell = new NHunspell.Hunspell("en_us.aff", "en_us.dic");
            dataTable.Columns.Add("IsSpelledCorrectly", typeof(bool));

            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (worker != null)
                    {
                        int percent = (rowCounter * 100) / dataTable.Rows.Count;
                        worker.ReportProgress(percent);
                    }

                    row["IsSpelledCorrectly"] = true;
                    if (string.IsNullOrWhiteSpace(row[columnToSpellCheck].ToString()))
                        continue;

                    // split row based on spaces in case more than one word is in the result
                    string[] multiWords = row[columnToSpellCheck].ToString().Split(' ');
                    foreach (string wordToCheck in multiWords)
                    {
                        double isANumber;
                        // No need to check word against word if the word is in the ignore list or is a number.
                        if (_ignoreDictionaryHelper.IgnoreList.ContainsKey(wordToCheck)
                            || double.TryParse(wordToCheck, out isANumber))
                            continue;

                        row["IsSpelledCorrectly"] = hunspell.Spell(wordToCheck);
                        rowCounter++;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Sorry Error", exception.InnerException.ToString());
            }
            finally
            {
                hunspell.Dispose();
            }

            return dataTable;
        }
    }
}