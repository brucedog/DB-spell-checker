using BaseLibrary;
using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Rhino.Mocks;
using Utils;

namespace TestSpellCheckOfTable
{
    public class BaseUnitTests
    {
        protected IWindowManager WindowManager;
        protected IKernel Kernel;
        protected IDbHandler MockDb;
        protected IIgnoreDictionaryHelper IgnoreDictionaryHelper;

        [TestInitialize]
        public void SetUp()
        {
            WindowManager = MockRepository.GenerateMock<IWindowManager>();
            Kernel = MockRepository.GenerateMock<IKernel>();
            MockDb = MockRepository.GenerateStrictMock<IDbHandler>();
            IgnoreDictionaryHelper = MockRepository.GenerateStrictMock<IIgnoreDictionaryHelper>();

            DbConnectionManager.ConnectionManager.DbHandler = MockDb;
        }
    }
}