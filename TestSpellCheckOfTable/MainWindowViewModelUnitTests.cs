using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SpellCheckDbTable.ViewModels;

namespace TestSpellCheckOfTable
{
    [TestClass]
    public class MainWindowViewModelUnitTests : BaseUnitTests
    {
        [TestMethod]
        public void MainWindowViewModel_Contstructor()
        {
            var sut = new MainWindowViewModel(Kernel, WindowManager);

            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void MainWindowViewModel_OnSelectionChangedDatabaseName()
        {
            string db = "testDB";
            MockDb.Expect(e => e.GetTableNames()).Return(new List<string> {"table1", "table2"});
            MockDb.Expect(e => e.TableToSearch = db);
            var sut = new MainWindowViewModel(Kernel, WindowManager);

            sut.OnSelectionChangedDatabaseName(db);

            MockDb.VerifyAllExpectations();
            Assert.IsTrue(string.IsNullOrWhiteSpace(sut.ColumnToSpellCheck));
            Assert.IsTrue(sut.IsDatabaseNameSelected);
            Assert.IsFalse(sut.IsTableSelected);
            Assert.IsFalse(sut.IsSpellCheckEnabled);
            Assert.AreEqual(sut.ColumnNames.Count, 0);
        }

        [TestMethod]
        public void MainWindowViewModel_OnSelectionChangedTableName()
        {
            string table = "table1";
            MockDb.Expect(e => e.GetColumnNames(table)).Return(new List<string> { "column1", "column2" });
            var sut = new MainWindowViewModel(Kernel, WindowManager);

            sut.OnSelectionChangedTableName(table);

            MockDb.VerifyAllExpectations();
            Assert.IsTrue(string.IsNullOrWhiteSpace(sut.ColumnToSpellCheck));
            Assert.IsTrue(sut.IsTableSelected);
            Assert.IsFalse(sut.IsSpellCheckEnabled);
            Assert.AreNotEqual(sut.ColumnNames.Count, 0);
        }

        [TestMethod]
        public void MainWindowViewModel_OnSelectionChangedColumnName()
        {
            string column = "column1";
            var sut = new MainWindowViewModel(Kernel, WindowManager) { ColumnToSpellCheck = column };

            sut.OnSelectionChangedColumnName();

            Assert.IsTrue(sut.IsSpellCheckEnabled);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckNothingToSearch()
        {
            var sut = new MainWindowViewModel(Kernel, WindowManager);

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckOnlyDbName()
        {
            var sut = new MainWindowViewModel(Kernel, WindowManager) {DataBaseToSearch = "dbname"};

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckOnlyTableName()
        {
            var sut = new MainWindowViewModel(Kernel, WindowManager) { TableToSearch = "tablename" };

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckOnlyColumnName()
        {
            var sut = new MainWindowViewModel(Kernel, WindowManager) { ColumnToSpellCheck = "columname" };

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CanSpellCheck()
        {
            var sut = new MainWindowViewModel(Kernel, WindowManager)
            {
                ColumnToSpellCheck = "dbname",
                TableToSearch = "tablename",
                DataBaseToSearch = "dbname"
            };

            Assert.IsTrue(sut.CanSpellCheck);
        }
    }
}