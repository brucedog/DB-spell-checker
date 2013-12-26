using System.ComponentModel;
using System.Data;

namespace BaseLibrary
{
    public interface ISpellChecker
    {
        DataTable SpellCheckTable(string tableToSpellCheck, string columnToSpellCheck, BackgroundWorker worker);

        DataTable SpellCheckTable(string tableToSpellCheck);
    }
}