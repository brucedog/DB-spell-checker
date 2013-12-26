using System.Collections.Generic;

namespace SpellCheckDbTable.Utils
{
    public interface ISmoTasks
    {
        IEnumerable<string> SqlServers {get;}
        
        List<string> GetDatabases(SqlConnectionString connectionString);
        
        List<DatabaseTable> GetTables(SqlConnectionString connectionString);
    }
}
