using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace RegViewer.Lib
{
    internal class RegistryItem
    {
        public string Name { get; set; }

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
            set
            {
                this.ValueKind = value switch
                {
                    "REG_SZ" => RegistryValueKind.String,
                    "REG_EXPAND_SZ" => RegistryValueKind.ExpandString,
                    "REG_BINARY" => RegistryValueKind.Binary,
                    "REG_DWORD" => RegistryValueKind.DWord,
                    "REG_MULTI_SZ" => RegistryValueKind.MultiString,
                    "REG_QWORD" => RegistryValueKind.QWord,
                    _ => RegistryValueKind.Unknown
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
                    byte[] bytes => BitConverter.ToString(bytes).Replace("-", " "),
                    string[] strings => string.Join(", ", strings),
                    _ => this.Value?.ToString() ?? string.Empty
                };
            }
        }
    }
}
