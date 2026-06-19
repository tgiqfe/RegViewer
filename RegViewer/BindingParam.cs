using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RegViewer
{
    internal class BindingParam
    {
        public ObservableCollection<KeyItem> Keys1 { get; set; }
        public ObservableCollection<KeyItem> Keys2 { get; set; }

        public BindingParam()
        {
            Keys1 = new();
            Keys2 = new();
        }
    }
}
