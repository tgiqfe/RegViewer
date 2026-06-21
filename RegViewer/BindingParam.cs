using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RegViewer
{
    internal class BindingParam
    {
        public Setting Setting { get; set; }
        public ObservableCollection<KeyItem> Keys1 { get; set; }
        public ObservableCollection<KeyItem> Keys2 { get; set; }


        public BindingParam()
        {
            Setting = Setting.Load();
            Keys1 = new();
            Keys2 = new();
        }
    }
}
