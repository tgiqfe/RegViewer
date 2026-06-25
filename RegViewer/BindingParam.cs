using Microsoft.Win32;
using RegViewer.Lib;
using System.Collections.ObjectModel;

namespace RegViewer
{
    internal class BindingParam
    {
        public Setting Setting { get; set; }
        public ObservableCollection<KeyItem> RootKeys { get; set; }
        /*
        public ObservableCollection<KeyItem> Key_HKCU { get; set; }
        public ObservableCollection<KeyItem> Key_HKLM { get; set; }
        public ObservableCollection<KeyItem> Key_HKU { get; set; }
        public ObservableCollection<KeyItem> Key_HKCC { get; set; }
        */

        public BindingParam()
        {
            Setting = Setting.Load();
            RootKeys = new();


            RootKeys.Add(new KeyItem(Registry.ClassesRoot));
            RootKeys.Add(new KeyItem(Registry.CurrentUser));
            RootKeys.Add(new KeyItem(Registry.LocalMachine));
            RootKeys.Add(new KeyItem(Registry.Users));
            RootKeys.Add(new KeyItem(Registry.CurrentConfig));
            /*
            Key_HKCU.Add(new KeyItem(Registry.CurrentUser));
            Key_HKLM.Add(new KeyItem(Registry.LocalMachine));
            Key_HKU.Add(new KeyItem(Registry.Users));
            Key_HKCC.Add(new KeyItem(Registry.CurrentConfig));
            */
        }
    }
}
