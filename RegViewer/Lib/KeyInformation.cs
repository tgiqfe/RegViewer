using Microsoft.Win32;
using RegViewer.Lib.RegistryCodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace RegViewer.Lib
{
    internal class KeyInformation : INotifyPropertyChanged
    {
        public string Path { get; set; }
        public string Name { get; set; }

        private string _owner = null;
        public string Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                OnPropertyChanged();
            }
        }

        private bool? _isInherited = null;
        public bool? IsInherited
        {
            get => _isInherited;
            set
            {
                _isInherited = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ACL> _acls;
        public ObservableCollection<ACL> ACLs
        {
            get => _acls;
            set
            {
                _acls = value;
                OnPropertyChanged();
            }
        }

        public class ACL
        {
            public string Account { get; set; }
            public bool IsAllow { get; set; }
            public string RightsText { get; set; }
            public bool IsRecurse { get; set; }
            public bool IsInherited { get; set; }
        }

        public void GetKeyInformation(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileName(path);
            try
            {
                using (var regkey = RegistryHelper.GetRegistryKey(path, isCreate: false, writable: false))
                {
                    var security = regkey.GetAccessControl();
                    this.Owner = security.GetOwner(typeof(NTAccount)).ToString();
                    this.IsInherited = !security.AreAccessRulesProtected;
                    var rules = security.GetAccessRules(true, true, typeof(NTAccount));
                    var acls = new ObservableCollection<ACL>();
                    foreach (RegistryAccessRule rule in rules)
                    {
                        acls.Add(new ACL
                        {
                            Account = rule.IdentityReference.Value,
                            IsAllow = rule.AccessControlType == AccessControlType.Allow,
                            RightsText = rule.RegistryRights.ToString(),
                            IsRecurse =
                                rule.InheritanceFlags.HasFlag(InheritanceFlags.ContainerInherit) ||
                                rule.InheritanceFlags.HasFlag(InheritanceFlags.ObjectInherit),
                            IsInherited = rule.IsInherited,
                        });
                    }
                    this.ACLs = acls;
                }
            }
            catch
            {
                this.Owner = null;
                this.IsInherited = null;
                this.ACLs = null;
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
