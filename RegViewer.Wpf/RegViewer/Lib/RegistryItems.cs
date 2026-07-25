using Microsoft.Win32;
using RegViewer.Lib.RegistryCodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Documents;

namespace RegViewer.Lib
{
    public class RegistryItems : INotifyPropertyChanged
    {
        #region Parameters

        private ObservableCollection<RegistryItem> _items;
        public ObservableCollection<RegistryItem> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
        #region Inner class

        public class RegistryItem
        {
            public string Name { get; set; }
            public string NameText
            {
                get
                {
                    return string.IsNullOrEmpty(this.Name) ? "(既定)" : this.Name;
                }
            }

            public RegistryValueKind ValueKind { get; set; }
            public string ValueKindText
            {
                get
                {
                    return this.ValueKind switch
                    {
                        RegistryValueKind.String => "REG_SZ",
                        RegistryValueKind.ExpandString => "REG_EXPAND_SZ",
                        RegistryValueKind.Binary => "REG_BINARY",
                        RegistryValueKind.DWord => "REG_DWORD",
                        RegistryValueKind.MultiString => "REG_MULTI_SZ",
                        RegistryValueKind.QWord => "REG_QWORD",
                        _ => "REG_UNKNOWN"
                    };
                }
            }

            public object Value { get; set; }
            public string ValueText
            {
                get
                {
                    return this.Value switch
                    {
                        string str => str,
                        int i => i.ToString(),
                        long l => l.ToString(),
                        byte[] bytes => bytes.Length > 1024 ?
                            BitConverter.ToString(bytes, 0, 1024).Replace("-", " ") + " ..." :
                            BitConverter.ToString(bytes).Replace("-", " "),
                        string[] strings => string.Join(", ", strings),
                        null => "(値の設定なし)",
                        _ => this.Value?.ToString() ?? string.Empty
                    };
                }
            }
        }

        #endregion

        public void GetRegistryItems(RegistryKey key)
        {
            List<RegistryItem> list = new();
            if (key != null)
            {
                foreach (var valueName in key.GetValueNames())
                {
                    var valueKind = key.GetValueKind(valueName);
                    var item = new RegistryItem
                    {
                        Name = valueName,
                        ValueKind = valueKind,
                        Value = valueKind == RegistryValueKind.ExpandString ?
                            key.GetValue(valueName, "", RegistryValueOptions.DoNotExpandEnvironmentNames) :
                            key.GetValue(valueName),
                    };
                    list.Add(item);
                }
            }

            //  既定のプロパティが存在しない場合は、空の既定のプロパティを追加する
            if (!list.Any(x => string.IsNullOrEmpty(x.Name)))
            {
                list.Add(new RegistryItem
                {
                    Name = string.Empty,
                    ValueKind = RegistryValueKind.String,
                    Value = null,
                });
            }

            this.Items = new ObservableCollection<RegistryItem>(list.OrderBy(x => x.Name));
        }

        public void Clear()
        {
            this.Items = new ObservableCollection<RegistryItem>();
        }


        #region Inotify change

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
