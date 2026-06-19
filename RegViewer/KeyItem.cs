using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Win32;

namespace RegViewer
{
    internal class KeyItem : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public RegistryKey RootKey { get; set; }

        public string Path { get; set; }

        private ObservableCollection<KeyItem> _subKeys = null;
        public ObservableCollection<KeyItem> SubKeys
        {
            get => _subKeys;
            set
            {
                if (_subKeys != value)
                {
                    _subKeys = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public KeyItem() { }

        public KeyItem(RegistryKey rootKey)
        {
            this.RootKey = rootKey;
            this.Name = rootKey.Name;
            this.Path = "";

            LoadSubKeys();
        }

        public void LoadSubKeys()
        {
            if (SubKeys == null)
            {
                this.SubKeys = new ObservableCollection<KeyItem>();
                using (var regKey = this.RootKey.OpenSubKey(this.Path))
                {
                    if (regKey != null)
                    {
                        foreach (var subKeyName in regKey.GetSubKeyNames())
                        {
                            SubKeys.Add(new KeyItem()
                            {
                                Name = subKeyName,
                                Path = this.Path == "" ? subKeyName : $"{this.Path}\\{subKeyName}",
                                RootKey = this.RootKey
                            });
                        }
                    }
                }
            }
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
