using System.Data;
using System.Linq;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SpellCheckDbTable;
using Utils;

namespace TestSpellCheckOfTable
{
    [TestClass]
    public class SpellCheckerUnitTest
    {
        private IDbHandler _mockDb;
        private IIgnoreDictionaryHelper _ignoreDictionaryHelper;
        private const string TableName = "SomeTable";
        private const string ColumnToSearch = "SomeColumn";
        
        [TestInitialize]
        public void TestSetup()
        {
            _ignoreDictionaryHelper = MockRepository.GenerateStrictMock<IIgnoreDictionaryHelper>();
            _mockDb = MockRepository.GenerateStrictMock<IDbHandler>();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(ColumnToSearch));
            DataRow row = table.NewRow();
            row[ColumnToSearch] = "spelled";
            table.Rows.Add(row);
            row = table.NewRow();
            row[ColumnToSearch] = "spelledz";
            table.Rows.Add(row);
            row = table.NewRow();
            row[ColumnToSearch] = "yes";
            table.Rows.Add(row);
            
            _mockDb.Stub(s => s.GetRows(Arg<string>.Is.Equal(TableName), Arg<string>.Is.Equal(ColumnToSearch)))
                .Return(table);
        }


        /// <summary>
        /// Doesnt really test anything 
        /// </summary>
        [TestMethod]
        public void DbHandlerGetRowsAndColumns()
        {
            IDbHandler dbHandler = _mockDb;
            var test = dbHandler.GetRows(TableName, ColumnToSearch);

            Assert.IsTrue(test.Columns.Count > 0);
            Assert.IsTrue(test.Rows.Count > 0);
        }

        [TestMethod]
        public void MissSpellsFoundInTable()
        {
            ISpellChecker spellChecker = new SpellChecker(new IgnoreDictionaryHelper());
            var dataTable = spellChecker.SpellCheckTable(TableName, ColumnToSearch, null);
            var errors = (from error in dataTable.AsEnumerable()
                          where error.Field<bool>("IsSpelledCorrectly") == false
                          select error).ToList();

            Assert.IsTrue(dataTable.Columns.Count > 0);
            Assert.IsTrue(dataTable.Rows.Count > 0);
            Assert.IsTrue(errors.Count > 0);
        }

        [TestMethod]
        public void GetListOfTableNames()
        {
            IDbHandler dbHandler = _mockDb;
            var sut = dbHandler.GetTableNames();

            Assert.IsTrue(sut.Count > 0);
        }

        [TestMethod]
        public void GetListOfColumnNamesForGivenTable()
        {
            IDbHandler dbHandler = _mockDb;
            var sut = dbHandler.GetColumnNames(TableName);

            Assert.IsTrue(sut.Count > 0);
        }

        [TestMethod]
        public void GetListOfWordsToIgnore()
        {
            IIgnoreDictionaryHelper ignoreDictionaryHelper = new IgnoreDictionaryHelper();
            ignoreDictionaryHelper.BuildIgnoreList();
            Assert.IsTrue(ignoreDictionaryHelper.IgnoreList.Count > 0);
        }
    }
}