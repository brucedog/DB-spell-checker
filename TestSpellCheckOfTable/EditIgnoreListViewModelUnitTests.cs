using System.Collections;
using System.Data;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SpellCheckDbTable.ViewModels;

namespace TestSpellCheckOfTable
{
    [TestClass]
    public class EditIgnoreListViewModelUnitTests : BaseUnitTests
    {
        private const string TableName = "SomeTable";
        private const string ColumnToSearch = "SomeColumn";

        [TestInitialize]
        public void TestSetup()
        {
            IgnoreDictionaryHelper = MockRepository.GenerateStrictMock<IIgnoreDictionaryHelper>();
            IgnoreDictionaryHelper.Stub(s => s.IgnoreList).Return(new Hashtable());
            MockDb = MockRepository.GenerateStrictMock<IDbHandler>();
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

        [TestMethod]
        public void EditIgnoreListViewModel_Contruct()
        {
            IgnoreDictionaryHelper.Expect(e => e.DoesIgnoreFileExist()).Return(false);
            var sut = new EditIgnoreListViewModel(IgnoreDictionaryHelper);

            IgnoreDictionaryHelper.VerifyAllExpectations();
            Assert.IsInstanceOfType(sut, typeof(EditIgnoreListViewModel));
        }

        [TestMethod]
        public void EditIgnoreListViewModel_SaveIgnoreList()
        {
            string save = "save this";
            IgnoreDictionaryHelper.Expect(e => e.DoesIgnoreFileExist()).Return(false);
            IgnoreDictionaryHelper.Expect(e => e.SaveIgnoreList(save));
            var sut = new EditIgnoreListViewModel(IgnoreDictionaryHelper){IgnoreList = save};
            
            sut.SaveIgnoreList();
            
            IgnoreDictionaryHelper.VerifyAllExpectations();
        }
    }
}