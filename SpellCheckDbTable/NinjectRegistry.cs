using BaseLibrary;
using Ninject.Modules;
using SpellCheckDbTable.ViewModels;
using Utils;

namespace SpellCheckDbTable
{
    public class NinjectRegistry : NinjectModule
    {
        public override void Load()
        {
            // service 
            Bind<ISpellChecker>().To<SpellChecker>();
            Bind<IIgnoreDictionaryHelper>().To<IgnoreDictionaryHelper>();

            // view models
            Bind<MainWindowViewModel>().To<MainWindowViewModel>().InSingletonScope();
            Bind<EditIgnoreListViewModel>().To<EditIgnoreListViewModel>();
            Bind<DbConnectionViewModel>().To<DbConnectionViewModel>();
        }
    }
}