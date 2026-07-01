using MaterialDesignThemes.Wpf;
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
                OnPropertyChanged(nameof(IsInheritedText));
            }
        }
        public string IsInheritedText
        {
            get
            {
                if (IsInherited == null) return null;
                return IsInherited.Value ? "有効" : "無効";
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
            public RegistryRights Rights { get; set; }
            public bool IsAllow { get; set; }
            public bool IsRecurse { get; set; }
            public bool IsInherited { get; set; }

            public string RightsText
            {
                get
                {
                    return Rights switch
                    {
                        RegistryRights.FullControl => "フルコントロール",
                        RegistryRights.ReadKey => "読み取り",
                        _ => "特殊なアクセス許可",
                    };
                }
            }
            public string AllowType { get => IsAllow ? "許可" : "拒否"; }
            public string RecursiveType { get => IsRecurse ? "このキーとサブキー" : "このキーのみ"; }
            public string IsInheritedMark { get => IsInherited ? "●" : ""; }
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

                    List<ACL> aclList = new();
                    foreach (RegistryAccessRule rule in rules)
                    {
                        aclList.Add(new ACL
                        {
                            Account = rule.IdentityReference.Value,
                            Rights = rule.RegistryRights,
                            IsAllow = rule.AccessControlType == AccessControlType.Allow,
                            IsRecurse =
                                rule.InheritanceFlags.HasFlag(InheritanceFlags.ContainerInherit) ||
                                rule.InheritanceFlags.HasFlag(InheritanceFlags.ObjectInherit),
                            IsInherited = rule.IsInherited,
                        });
                    }
                    this.ACLs = new ObservableCollection<ACL>(aclList);
                }
            }
            catch
            {
                this.Owner = null;
                this.IsInherited = null;
                this.ACLs = null;
            }
        }

        public void UpdateAclList(bool viewInherited)
        {
            if (this.ACLs == null) return;

            using (var regkey = RegistryHelper.GetRegistryKey(this.Path, isCreate: false, writable: false))
            {
                var security = regkey.GetAccessControl();
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));
                List<ACL> aclList = new();
                foreach (RegistryAccessRule rule in rules)
                {
                    if (rule.IsInherited && !viewInherited) continue;
                    aclList.Add(new ACL
                    {
                        Account = rule.IdentityReference.Value,
                        Rights = rule.RegistryRights,
                        IsAllow = rule.AccessControlType == AccessControlType.Allow,
                        IsRecurse =
                            rule.InheritanceFlags.HasFlag(InheritanceFlags.ContainerInherit) ||
                            rule.InheritanceFlags.HasFlag(InheritanceFlags.ObjectInherit),
                        IsInherited = rule.IsInherited,
                    });
                }
                this.ACLs = new ObservableCollection<ACL>(aclList);
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
