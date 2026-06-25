using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;

namespace RegViewer.Lib
{
    public class KeyItem : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Path
        {
            get
            {
                return string.IsNullOrEmpty(this.RelatedPath) ?
                    this.RootKey.Name :
                    $"{this.RootKey.Name}\\{this.RelatedPath}";
            }
        }

        public RegistryKey RootKey { get; set; }

        public string RelatedPath { get; set; }

        public KeyItem Parent { get; set; }

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
            this.RelatedPath = "";

            LoadSubKeys();
            foreach (var item in this.SubKeys)
            {
                item.LoadSubKeys();
            }
        }

        public void LoadSubKeys()
        {
            if (SubKeys == null)
            {
                this.SubKeys = new ObservableCollection<KeyItem>();
                try
                {
                    using (var regKey = this.RootKey.OpenSubKey(this.RelatedPath))
                    {
                        if (regKey != null)
                        {
                            foreach (var subKeyName in regKey.GetSubKeyNames())
                            {
                                SubKeys.Add(new KeyItem()
                                {
                                    Name = subKeyName,
                                    RelatedPath = this.RelatedPath == "" ? subKeyName : $"{this.RelatedPath}\\{subKeyName}",
                                    RootKey = this.RootKey,
                                    Parent = this,
                                });
                            }
                        }
                    }
                }
                catch (SecurityException e)
                {
                    Console.WriteLine($"SecurityException: {e.Message}");
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
