using System.Collections;
using System.Data;
using System.Linq;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Utils;

namespace TestSpellCheckOfTable
{
    [TestClass]
    public class SpellCheckerUnitTests
    {
        private IDbHandler _mockDb;
        private IIgnoreDictionaryHelper _ignoreDictionaryHelper;
        private const string TableName = "SomeTable";
        private const string ColumnToSearch = "SomeColumn";
        
        [TestInitialize]
        public void TestSetup()
        {
            _ignoreDictionaryHelper = MockRepository.GenerateStrictMock<IIgnoreDictionaryHelper>();
            _ignoreDictionaryHelper.Stub(s => s.IgnoreList).Return(new Hashtable());
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

        [TestMethod]
        public void MissSpellsFoundInTable()
        {            
            ISpellChecker spellChecker = new SpellChecker(_ignoreDictionaryHelper);
            DbConnectionManager.ConnectionManager.DbHandler = _mockDb;
            
            var dataTable = spellChecker.SpellCheckTable(TableName, ColumnToSearch, null);
            
            var errors = (from error in dataTable.AsEnumerable()
                          where error.Field<bool>("IsSpelledCorrectly") == false
                          select error).ToList();

            Assert.IsTrue(dataTable.Columns.Count > 0);
            Assert.IsTrue(dataTable.Rows.Count == 3);
            Assert.IsTrue(errors.Count == 1);
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