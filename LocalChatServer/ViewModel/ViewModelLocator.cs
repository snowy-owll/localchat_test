/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:LocalChatServer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using DBLocalChat;
using GalaSoft.MvvmLight.Ioc;
using LocalChatServer.Service;

namespace LocalChatServer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<ISettingsService>(() => SettingsService.Instance);
            var unitOfWork = new UnitOfWork(new DBLocalChatContext("LocalChatServerConnectionString"));
            SimpleIoc.Default.Register<IUnitOfWork>(() => unitOfWork);
            var wcfHostService = new WCFHostService(unitOfWork);
            SimpleIoc.Default.Register<IWCFHostService>(() => wcfHostService);
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ServerViewModel>();
        }

        public SettingsViewModel Settings => SimpleIoc.Default.GetInstanceWithoutCaching<SettingsViewModel>();

        public ServerViewModel Server => SimpleIoc.Default.GetInstanceWithoutCaching<ServerViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}