using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Rhino.Mocks;
using SpellCheckDbTable.ViewModels;

namespace TestSpellCheckOfTable
{
    [TestClass]
    public class MainWindowViewModelUnitTests
    {
        private IWindowManager _windowManager;
        private IKernel _kernel;

        [TestInitialize]
        public void SetUp()
        {
            _windowManager = MockRepository.GenerateMock<IWindowManager>();
            _kernel = MockRepository.GenerateMock<IKernel>();
        }

        [TestMethod]
        public void MainWindowViewModel_Contstructor()
        {
            var sut = new MainWindowViewModel(_kernel, _windowManager);

            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckNothingToSearch()
        {
            var sut = new MainWindowViewModel(_kernel, _windowManager);

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckOnlyDbName()
        {
            var sut = new MainWindowViewModel(_kernel, _windowManager) {DataBaseToSearch = "dbname"};

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckOnlyTableName()
        {
            var sut = new MainWindowViewModel(_kernel, _windowManager) { TableToSearch = "tablename" };

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CannotSpellCheckOnlyColumnName()
        {
            var sut = new MainWindowViewModel(_kernel, _windowManager) { ColumnToSpellCheck = "columname" };

            Assert.IsFalse(sut.CanSpellCheck);
        }

        [TestMethod]
        public void MainWindowViewModel_CanSpellCheck()
        {
            var sut = new MainWindowViewModel(_kernel, _windowManager)
            {
                ColumnToSpellCheck = "dbname",
                TableToSearch = "tablename",
                DataBaseToSearch = "dbname"
            };

            Assert.IsTrue(sut.CanSpellCheck);
        }
    }
}