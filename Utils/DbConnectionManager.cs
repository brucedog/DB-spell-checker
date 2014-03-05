using BaseLibrary;

namespace Utils
{
    public class DbConnectionManager
    {
        private static readonly DbConnectionManager dbConnectionManager = new DbConnectionManager();
        private DbConnectionManager(){}

        public static DbConnectionManager ConnectionManager
        {
            get { return dbConnectionManager; }
        }

        public IDbHandler DbHandler { get; set; }
    }
}