using RegViewer.Lib.RegistryCodes;
using System.Configuration;
using System.Data;
using System.Windows;

namespace RegViewer
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Item.BindingParam = new();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Item.BindingParam.Setting.Save();
        }
    }

}
