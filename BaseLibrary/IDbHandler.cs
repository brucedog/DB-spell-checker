using System.Collections.Generic;
using System.Data;

namespace BaseLibrary
{
    public interface IDbHandler
    {
        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        /// <value>
        /// The database connection string.
        /// </value>
        string DbConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [valid connection string].
        /// </summary>
        /// <value>
        /// <c>true</c> if [valid connection string]; otherwise, <c>false</c>.
        /// </value>
        bool ValidConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is windows authentication].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is windows authentication]; otherwise, <c>false</c>.
        /// </value>
        bool IsWindowsAuth { get; set; }

        /// <summary>
        /// Gets or sets the name of the database user.
        /// </summary>
        /// <value>
        /// The name of the database user.
        /// </value>
        string DbUserName { get; set; }

        /// <summary>
        /// Gets or sets the database password.
        /// </summary>
        /// <value>
        /// The database password.
        /// </value>
        string DbPassword { get; set; }

        /// <summary>
        /// Gets or sets the table to search.
        /// </summary>
        /// <value>
        /// The table to search.
        /// </value>
        string TableToSearch { get; set; }

        /// <summary>
        /// Gets or sets the column to spell checker.
        /// </summary>
        /// <value>
        /// The column to spell checker.
        /// </value>
        string ColumnToSpellChecker { get; set; }

        /// <summary>
        /// Gets or sets the database command text.
        /// </summary>
        /// <value>
        /// The database command text.
        /// </value>
        string DbCommandText { get; set; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <param name="tableToSearch">The table to search.</param>
        /// <param name="columnToSpellCheck">The column to spell check.</param>
        /// <returns></returns>
        DataTable GetRows(string tableToSearch, string columnToSpellCheck);

        /// <summary>
        /// Gets the table names.
        /// </summary>
        /// <returns></returns>
        List<string> GetTableNames();

        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        List<string> GetColumnNames(string tableName);

        /// <summary>
        /// Gets the databases.
        /// </summary>
        /// <returns></returns>
        List<string> GetDatabases();
    }
}