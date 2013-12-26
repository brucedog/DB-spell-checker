using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace BaseLibrary
{
    public interface IDbHandler
    {
        DbConnection DbConnection { get; }

        string DbConnectionString { get; set; }

        bool ValidConnectionString { get; set; }

        string DbUserName { get; set; }

        string DbPassword { get; set; }

        string TableToSearch { get; set; }

        string ColumnToSpellChecker { get; set; }

        string DbCommandText { get; set; }

        DataTable GetRows(string tableToSearch, string columnToSpellCheck);

        List<string> GetTableNames();

        List<string> GetColumnNames(string tableName);
    }
}