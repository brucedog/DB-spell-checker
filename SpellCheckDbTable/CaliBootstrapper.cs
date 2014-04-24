using System;
using Caliburn.Micro;
using Ninject;
using SpellCheckDbTable.ViewModels;

namespace SpellCheckDbTable
{
    public class CaliBootstrapper : Bootstrapper<MainWindowViewModel>
    {
        protected override void Configure()
        {
            InitializeNinject();
            Kernel.Get<IEventAggregator>().Subscribe(this);
        }
        
        /// <summary>
        /// Overrides default IoC behavior to use Ninject.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrEmpty(key)
                ? Kernel.Get(service)
                : Kernel.Get(service, key);
        }

        /// <summary>
        /// Initializes the IoC container.
        /// </summary>
        private void InitializeNinject()
        {
            Kernel = new StandardKernel();
            Kernel.Load(new NinjectRegistry());
            Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
        }

        private IKernel Kernel { get; set; }
    }
}