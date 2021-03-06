﻿using System.Windows;

using ANPaX.UI.DesktopUI.ViewModels;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
            //DisplayRootViewFor<ShellViewModel>();
        }

        //protected override object GetInstance(Type service, string key)
        //{
        //    return _container.GetInstance(service, key);
        //}
    }
}
