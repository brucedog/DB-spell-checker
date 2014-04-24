using System.Collections;
using System.Data;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SpellCheckDbTable.ViewModels;

namespace TestSpellCheckOfTable
{
    [TestClass]
    public class EditIgnoreListViewModelUnitTests
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
        public void EditIgnoreListViewModel_Contruct()
        {
            _ignoreDictionaryHelper.Expect(e => e.DoesIgnoreFileExist()).Return(false);
            var sut = new EditIgnoreListViewModel(_ignoreDictionaryHelper);

            _ignoreDictionaryHelper.VerifyAllExpectations();
            Assert.IsInstanceOfType(sut, typeof(EditIgnoreListViewModel));
        }

        [TestMethod]
        public void EditIgnoreListViewModel_SaveIgnoreList()
        {
            string save = "save this";
            _ignoreDictionaryHelper.Expect(e => e.DoesIgnoreFileExist()).Return(false);
            _ignoreDictionaryHelper.Expect(e => e.SaveIgnoreList(save));
            var sut = new EditIgnoreListViewModel(_ignoreDictionaryHelper){IgnoreList = save};
            
            sut.SaveIgnoreList();
            
            _ignoreDictionaryHelper.VerifyAllExpectations();
        }
    }
}