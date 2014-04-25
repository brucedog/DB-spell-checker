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
    public class SpellCheckerUnitTests : BaseUnitTests
    {
        private const string TableName = "SomeTable";
        private const string ColumnToSearch = "SomeColumn";
        
        [TestInitialize]
        public void TestSetup()
        {
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

            IgnoreDictionaryHelper.Stub(s => s.IgnoreList).Return(new Hashtable());
            MockDb.Stub(s => s.GetRows(Arg<string>.Is.Equal(TableName), Arg<string>.Is.Equal(ColumnToSearch)))
                .Return(table);
        }
        /*
        [TestMethod]
        public void MissSpellsFoundInTable()
        {            
            ISpellChecker spellChecker = new SpellChecker(IgnoreDictionaryHelper);
            DbConnectionManager.ConnectionManager.DbHandler = MockDb;
            
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
        */
    }
}