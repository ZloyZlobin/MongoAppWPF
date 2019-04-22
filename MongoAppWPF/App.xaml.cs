using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using MongoAppWPF.Interfaces.Users.Service;
using MongoAppWPF.ViewModels;
using Ninject;

namespace MongoAppWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow app;
        private IKernel container;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // For catching Global uncaught exception
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);

            ConfigureContainer();

            app = new MainWindow();
            MainViewModel context = new MainViewModel(container.Get<IUserService>());
            app.DataContext = context;
            app.ShowDialog();
        }

        private void ConfigureContainer()
        {
            container = new StandardKernel();
            container.Bind<IConfiguration>()
                .ToMethod(ctx => new ConfigurationBuilder().AddJsonFile("settings.json").Build());

            container.Load($"{Assembly.GetEntryAssembly().GetName().Name}.*.dll");
        }

        static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs args)
        {
            // Show a message before closing application
            var dialogService = new MvvmDialogs.DialogService();
            Exception e = (Exception)args.ExceptionObject;
            dialogService.ShowMessageBox((INotifyPropertyChanged)(app.DataContext),
                e.Message,
                "Unhandled Error",
                MessageBoxButton.OK);
        }
    }
}
