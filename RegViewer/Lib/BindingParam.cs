using Microsoft.Win32;
using RegViewer.Lib;
using System.Collections.ObjectModel;

namespace RegViewer.Lib
{
    internal class BindingParam
    {
        public Setting Setting { get; set; }
        public ObservableCollection<KeyItem> RootKeys { get; set; }
        public AddressBar AddressBar { get; set; }

        public BindingParam()
        {
            this.Setting = Setting.Load();
            this.AddressBar = new AddressBar();

            this.RootKeys = new();
            this.RootKeys.Add(new KeyItem(Registry.ClassesRoot));
            this.RootKeys.Add(new KeyItem(Registry.CurrentUser));
            this.RootKeys.Add(new KeyItem(Registry.LocalMachine));
            this.RootKeys.Add(new KeyItem(Registry.Users));
            this.RootKeys.Add(new KeyItem(Registry.CurrentConfig));
        }
    }
}
